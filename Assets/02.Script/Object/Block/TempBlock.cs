using UnityEngine;

public class TempBlock : Block
{
    Vector2 startPos;

    public override void Start()
    {
        InitializeReset();
        GameManager.Instance.RegisterInitAction(ResetAction);
        GameManager.Instance.OnReset += ResetAction;
    }
    override public void InitializeReset()
    {
        startPos = transform.position;
    }

    override public void ResetAction()
    {
        transform.position = startPos;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D otherRb))
        {
            otherRb.linearVelocity = rb.linearVelocity;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        rb.linearVelocity = Vector2.zero;
    }
}