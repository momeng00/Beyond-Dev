using System.Collections;
using System.Diagnostics.Tracing;
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
    }
    public override void Start()
    {
        base.Start();
        RunToggleEvent(false);
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
        focusAnimation.Play("Default");
        foreach (var popup in PopUpList)
        {
            NormalPopUp sample = popup.GetComponent<NormalPopUp>();
            if (sample != null)
            {
                sample.EventOnce = false;
                sample.ToggleEvent(false); //임시조치 (뇌가 아플때 했던거임)
            }
        }
        popupFlag = false;
    }
    public void UDAnimationPlay(bool value)
    {
        if(activeCoroution !=null)
            StopCoroutine(activeCoroution); gameObject.SetActive(true);

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        activeCoroution = StartCoroutine(ReturnAnimation(value));
    }
    public void OnDisable()
    {
        // 게임이 종료될 때 꺼지는 건 무시 (안 그러면 종료할 때마다 로그 뜸)
        if (!this.gameObject.scene.isLoaded) return;

        Debug.LogWarning($"[범인 색출] {gameObject.name}의 컴포넌트가 꺼졌습니다!");

        // 누가 껐는지 호출 스택(경로)을 전부 출력합니다.
        Debug.Log(System.Environment.StackTrace);
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
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
