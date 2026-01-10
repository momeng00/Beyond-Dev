using System.Collections;
using UnityEngine;

public class PushBlock : Block
{
    public LayerMask layerMask;
    private Animator animator;
    private Vector2 startPos;
    private Material material;
    private Coroutine activeCoroution;
    private bool popupFlag=false;
    [SerializeField] private Animator focusAnimation;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        startPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        ToggleEventChildren();
        BlockState = false;
        RunToggleEvent(false);
    }
    public override void Start()
    {
        base.Start();
        //해당 구문은 의도적으로 배치를 늘려서 Matarial이 공용화가 되는 것을 막는것임
        material = this.GetComponent<Renderer>().material;
    }
    
    public override void OnBlockAction()
    {
        base.OnBlockAction();
        focusAnimation.Play("Focus", 0, 0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            animator.SetBool("activate", true);
            BlockState = true;
            if (!popupFlag)
            {
                RunToggleEvent(true);
                foreach (var popup in PopUpList)
                {
                    NormalPopUp sample = popup.GetComponent<NormalPopUp>();
                    if (sample != null)
                    {
                        sample.ToggleEvent(true); //임시조치 (뇌가 아플때 했던거임)
                        sample.EventOnce = true;
                    }
                }
                popupFlag = true;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            animator.SetBool("activate", false);
        }
    }
    
    protected bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((1 << obj.layer) & mask) != 0;
    }
    public override void ResetAction()
    {
        base.ResetAction();
        transform.position = startPos;
    }
    public void UDAnimationPlay(bool value)
    {
        if(activeCoroution !=null)
            StopCoroutine(activeCoroution); gameObject.SetActive(true);

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        activeCoroution = StartCoroutine(ReturnAnimation(value));
    }

    private IEnumerator ReturnAnimation(bool value)
    {
        if (value)
        {
            animator.Play("PushBlock_Download");
        }
        else
        {
            animator.Play("PushBlock_Upload");
            while (true)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("PushBlock_Upload") && stateInfo.normalizedTime >= 1.0f)
                {
                    animator.Play("PushBlock_Off");
                    animator.enabled = false;
                    Color color = spriteRenderer.color;
                    color.a = 1f; // 투명도 1(불투명)로 강제 설정
                    spriteRenderer.color = color;
                    animator.enabled = true;
                    gameObject.SetActive(false);
                    break;
                }
                yield return null;
            }
        }
        yield return null;
    }
}
