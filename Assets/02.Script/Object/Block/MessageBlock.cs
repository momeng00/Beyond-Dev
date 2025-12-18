using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MessageBlock : Block, ISwitchable
{
    private Animator ani;
    public Switch Switch => throw new System.NotImplementedException();
    public List<Switch> switchList = new List<Switch>();
    override public bool BlockState
    {
        get
        {
            return _blockState;
        }
        set
        {
            blockEvent?.Invoke(value);
            _blockState = value;
            MessagerBlock(value);
            RunToggleEvent(value); 
        }
    }
    private bool _blockState = false;
    private BoxCollider2D boxCollider;
    
    public LayerMask targetLayer;
    private List<Rigidbody2D> rigidbodiesInTrigger = new List<Rigidbody2D>();
    private void Awake()
    {
        ani = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider.size = spriteRenderer.bounds.size;
    }
    public override void Start()
    {
        base.Start();
        foreach (Switch sw in switchList)
        {
            sw.SetSwitch(this);
        }
        ToggleEventChildren();
        BlockState = false;
        
    }
    public bool SwitchOn(bool value)
    {
        BlockState = !BlockState;
        return true;
    }
    public void MessagerBlock(bool on)
    {
        if (on) // 블록이 켜질 때
        {
            MoveObjectsUp();
            ani.Play("In");
            mask.enabled = on;
            matarialAnim.Play();
            //spriteRenderer.enabled = true; // 보이게
            boxCollider.isTrigger = false; // 단단한 발판으로
        }
        else // 블록이 꺼질 때
        {
            ani.Play("Out");
            mask.enabled = on;
            matarialAnim.PlayReturn();
            //spriteRenderer.enabled = false; // 안 보이게
            boxCollider.isTrigger = true; // 감지 모드로
        }
    }
    private bool CanPushHorizontally(Collider2D charCol, float directionX, float distance)
    {
        Vector2 direction = new Vector2(directionX, 0);

        // 결과를 담을 리스트 (Cast 함수는 결과를 여기에 넣어줍니다)
        RaycastHit2D[] results = new RaycastHit2D[1];

        // ContactFilter 설정 (레이어 마스크 등)
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(targetLayer);
        filter.useLayerMask = true;

        // --- 핵심 변경: 내 콜라이더 모양 그대로 쏘기 ---
        // distance + skinWidth 만큼 쏴서 벽이 걸리는지 확인
        // (이 함수는 감지된 개수를 반환합니다)
        int hitCount = charCol.Cast(direction, filter, results, distance + 0.02f);

        // 감지된 게 0개라면 벽이 없다는 뜻 -> 밀릴 수 있음 (true)
        return hitCount == 0;
    }
    private void MoveObjectsUp()
    {
        Debug.Log(rigidbodiesInTrigger.Count);
        // 리스트 복사
        List<Rigidbody2D> rigidbodiesToMove = new List<Rigidbody2D>(rigidbodiesInTrigger);

        // 내(블록) 콜라이더의 경계
        Bounds blockBounds = boxCollider.bounds;

        foreach (Rigidbody2D charRb in rigidbodiesToMove)
        {
            if (charRb == null) continue;

            Collider2D charCol = charRb.GetComponent<Collider2D>();
            Bounds charBounds = charCol.bounds;

            // --- 1. 겹친 깊이 계산 ---

            // 위로 탈출하기 위해 이동해야 할 거리
            // (블록의 상단) - (캐릭터의 발바닥)
            float distToTop = (blockBounds.max.y - charBounds.min.y);

            // 옆으로 탈출하기 위해 이동해야 할 거리 계산
            float distToRight = (blockBounds.max.x - charBounds.min.x); // 오른쪽으로 밀 때 거리
            float distToLeft = (charBounds.max.x - blockBounds.min.x); // 왼쪽으로 밀 때 거리

            // 캐릭터가 블록 중심보다 오른쪽에 있는지 왼쪽에 있는지 판단
            bool isRightSide = charBounds.center.x > blockBounds.center.x;

            // 현재 위치에서 가장 가까운 옆면 탈출 거리와 방향
            float sidePushDist = isRightSide ? distToRight : distToLeft;
            float sideDir = isRightSide ? 1f : -1f;


            // --- 2. 로직 판단 ---

            // 조건 A: 옆으로 나가는 거리가 위로 가는 거리보다 짧은가? (즉, 옆에 걸쳐있는가?)
            // 조건 B: 옆으로 나가는 거리가 너무 멀지 않은가? (캐릭터 폭의 절반 이하일 때만 옆으로 인정 등)
            bool isEdge = sidePushDist < distToTop && sidePushDist < (charBounds.size.x * 0.7f);

            if (isEdge)
            {
                // --- 3. 밀릴 수 있는지 체크 (BoxCast) ---
                if (CanPushHorizontally(charCol, sideDir, sidePushDist))
                {
                    // 벽이 없다 -> 옆으로 밀기
                    float targetX = charRb.position.x + (sideDir * sidePushDist) + (sideDir * 0.02f);
                    charRb.position = new Vector2(targetX, charRb.position.y);
                }
                else
                {
                    // 벽이 있다 -> 위로 올리기 (기존 로직)
                    PushUp(charRb, blockBounds.max.y, charBounds.size.y);
                }
            }
            else
            {
                // 완전히 안쪽에 있거나 위쪽이 더 가까움 -> 위로 올리기 (기존 로직)
                PushUp(charRb, blockBounds.max.y, charBounds.size.y);
            }
        }
    }

    // 코드 재사용을 위해 위로 올리는 부분만 따로 뺌
    private void PushUp(Rigidbody2D charRb, float blockTopY, float charHeight)
    {
        float targetY = blockTopY + (charHeight / 2f) + 0.02f;
        charRb.position = new Vector2(charRb.position.x, targetY);

        // (중요) 위로 올릴 때 Y축 속도를 0으로 만들어야 튀어 오르는 현상 방지
        charRb.linearVelocity = new Vector2(charRb.linearVelocity.x, 0);
    }
    //private void MoveObjectsUp()
    //{
    //    // 리스트를 복사해서 순회 (원본 리스트가 변경될 수 있으므로)
    //    List<Rigidbody2D> rigidbodiesToMove = new List<Rigidbody2D>(rigidbodiesInTrigger);

    //    foreach (Rigidbody2D rb in rigidbodiesToMove)
    //    {
    //        if (rb == null) continue; // 오브젝트가 파괴되었을 경우 대비

    //        float objectHeight = rb.GetComponent<Collider2D>().bounds.size.y;
    //        float boxTopY = transform.position.y + boxCollider.offset.y + (boxCollider.size.y / 2);

    //        Vector2 targetPosition = new Vector2(
    //            rb.position.x,
    //            boxTopY + (objectHeight / 2)
    //        );

    //        rb.position = targetPosition; // 즉시 위치 변경
    //    }
    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 지정된 레이어인지 확인하고 Rigidbody2D가 있는지 확인
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null && !rigidbodiesInTrigger.Contains(rb))
            {
                rigidbodiesInTrigger.Add(rb);
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {

        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null && !rigidbodiesInTrigger.Contains(rb))
            {
                rigidbodiesInTrigger.Add(rb);
            }

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null && rigidbodiesInTrigger.Contains(rb))
            {
                // 안에서 '오브젝트가 SetActive(false)가 되면 오류 확인을 해야합니다'
                rigidbodiesInTrigger.Remove(rb);
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null && rigidbodiesInTrigger.Contains(rb))
            {
                // 안에서 '오브젝트가 SetActive(false)가 되면 오류 확인을 해야합니다'
                rigidbodiesInTrigger.Remove(rb);
            }
        }
    }

    public override void ResetAction()
    {
        base.ResetAction();
        BlockState = false;
    }
}