using UnityEngine;

public class PushBlock : Block
{
    public LayerMask layerMask;
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
