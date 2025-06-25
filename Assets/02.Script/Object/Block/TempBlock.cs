using UnityEngine;

public class TempBlock : Block, IReset
{
    Vector2 startPos;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        InitializeReset();
        GameManager.Instance.RegisterInitAction(ResetAction);
        GameManager.Instance.OnReset += ResetAction;
    }
    public void InitializeReset()
    {
        startPos = transform.position;
    }

    public void ResetAction()
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