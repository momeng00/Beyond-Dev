using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MultiBlock : Block, ISwitchable
{
    private Animator ani;
    public Switch Switch => throw new System.NotImplementedException();
    public List<Switch> switchList = new List<Switch>();
    public bool BlockState
    {
        get
        {
            return _blockState;
        }
        set 
        {
            _blockState = value;
        }
    }
    private bool _blockState = false;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    public LayerMask targetLayer;
    private List<Rigidbody2D> rigidbodiesInTrigger = new List<Rigidbody2D>();
    private void Awake()
    {
        ani = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public override void Start()
    {
        base.Start();
        boxCollider.size = spriteRenderer.bounds.size;
        foreach (Switch sw in switchList)
        {
            sw.SetSwitch(this);
        }
        MessagerBlock(BlockState);
    }
    public void SwitchOn(bool value)
    {
        BlockState = !BlockState;
        MessagerBlock(BlockState);
    }
    public void MessagerBlock(bool on)
    {
        //ani.SetBool("BlockState",blockState);
        if (on) // 블록이 켜질 때
        {
            // 리스트에 있는 모든 오브젝트를 위로 올림
            MoveObjectsUp();
            ani.Play("In");
            //spriteRenderer.enabled = true; // 보이게
            boxCollider.isTrigger = false; // 단단한 발판으로
        }
        else // 블록이 꺼질 때
        {
            ani.Play("Out");
            //spriteRenderer.enabled = false; // 안 보이게
            boxCollider.isTrigger = true; // 감지 모드로
        }
    }
    private void MoveObjectsUp()
    {
        // 리스트를 복사해서 순회 (원본 리스트가 변경될 수 있으므로)
        List<Rigidbody2D> rigidbodiesToMove = new List<Rigidbody2D>(rigidbodiesInTrigger);

        foreach (Rigidbody2D rb in rigidbodiesToMove)
        {
            if (rb == null) continue; // 오브젝트가 파괴되었을 경우 대비

            float objectHeight = rb.GetComponent<Collider2D>().bounds.size.y;
            float boxTopY = transform.position.y + boxCollider.offset.y + (boxCollider.size.y / 2);

            Vector2 targetPosition = new Vector2(
                rb.position.x,
                boxTopY + (objectHeight / 2)
            );

            rb.position = targetPosition; // 즉시 위치 변경
        }
    }
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
    public override void InitializeReset()
    {
        base.InitializeReset();
    }
    public override void OnBlockAction()
    {
        base.OnBlockAction();
    }
    public override void ResetAction()
    {
        base.ResetAction();
    }

}