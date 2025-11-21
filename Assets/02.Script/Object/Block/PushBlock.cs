using UnityEngine;

public class PushBlock : Block
{
    public LayerMask layerMask;
    private Animator animator;
    private Vector2 startPos;
    private Material material;
    [SerializeField]private Animator focusAnimation;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        startPos = transform.position;
    }
    public override void Start()
    {
        base.Start();
        //해당 구문은 의도적으로 배치를 늘려서 Matarial이 공용화가 되는 것을 막는것임
        material = this.GetComponent<Renderer>().material; 
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnBlockAction()
    {
        base.OnBlockAction();
        focusAnimation.Play("Focus",0,0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            animator.SetBool("activate",true);
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
}
