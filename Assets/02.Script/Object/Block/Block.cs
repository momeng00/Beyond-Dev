using UnityEngine;

public abstract class Block : MonoBehaviour
{
    protected Collider2D col;
    protected Rigidbody2D rb;
    public virtual void OnBlockAction()
    {

    }
}
