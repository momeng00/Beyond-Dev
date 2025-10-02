using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TagBlock : Block
{
    private List<Animator> childAnimators;
    private Collider2D myCollider;
    public TagBlockController controller;

    [Header("필수")]
    public string groupName;
    public LayerMask detectedLayer;
    private bool isActivated = false; // 자신의 ON/OFF 상태
    private bool isTransitioning = false; // 상태 전환 중인지

    public TagContent content;

    //애니메이션이라면 추가적으로 event용 action을 추가해서 진행. (상태전환의 끝을 알려줄 곳)

    private void Awake()
    {
        childAnimators = GetComponentsInChildren<Animator>(true).ToList();
        myCollider = GetComponent<Collider2D>();
        if (controller == null) { 
            controller = GetComponentInParent<TagBlockController>();
            if (controller == null)
            {
                Debug.LogError("부모 계층에 BlockController가 없습니다!", this.gameObject);
            }
        }
        controller.RegisterBlock(groupName,this);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. 부딪힌 오브젝트의 경계(Bounds)를 가져옵니다.
        Bounds otherBounds = collision.collider.bounds;

        // 2. 이 오브젝트(센서)의 경계를 가져옵니다.
        if(myCollider == null)
        {
            myCollider = GetComponent<Collider2D>();
        }
        Bounds myBounds = myCollider.bounds;


        // 3. 상대방의 '발끝'(가장 낮은 y값)이 나의 '머리끝'(가장 높은 y값)보다 위에 있거나 같은지 확인합니다.
        float otherBottomEdge = otherBounds.center.y - otherBounds.extents.y;
        float myTopEdge = myBounds.center.y + myBounds.extents.y;
        Debug.Log(otherBottomEdge >= myTopEdge);
        if (IsInLayerMask(collision.gameObject, detectedLayer) && otherBottomEdge >= (myTopEdge-0.05f))
        {
            Debug.Log("위로 올라감 진입");
            controller.OnObjectEntered(groupName, collision.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        controller.OnObjectExited(groupName, collision.gameObject);
    }

    public override void OnBlockAction()
    {
        base.OnBlockAction();
        if (!isTransitioning)
        {
            StartCoroutine(ToggleStateCoroutine());
        }
    }
    private void SetAllAnimatorsBool(bool value)
    {
        if (childAnimators == null) return;

        // 리스트에 있는 모든 애니메이터를 순회하며 파라미터 값을 변경
        foreach (var animator in childAnimators)
        {
            animator.SetBool("IsActive", value);
        }
    }
    public override void InitializeReset()
    {
        base.InitializeReset();
    }

    public override void ResetAction()
    {
        base.ResetAction();
    }

    protected bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((1 << obj.layer) & mask) != 0;
    }
    private IEnumerator ToggleStateCoroutine()
    {
        isTransitioning = true;
        isActivated = !isActivated;
        Debug.Log("실행되었습니다.");
        SetAllAnimatorsBool(isActivated);
        //To Do....
        //애니메이션 혹은 컨텐츠를 띄우기 위한 동작이 들어갈 곳.
        //잠금 해제를 위한 기능이 들어갈곳.
        //애니메이션이면 위의 이벤트로 처리.
        isTransitioning = false;
        yield return null;
    }
}
