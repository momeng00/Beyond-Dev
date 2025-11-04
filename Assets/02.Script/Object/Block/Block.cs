using System;
using UnityEngine;

public abstract class Block : MonoBehaviour, IReset
{
    protected Collider2D col;
    protected Rigidbody2D rb;
    public Action<bool> blockEvent;
    public virtual void Start()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void OnBlockAction()
    {

    }
    public virtual void InitializeReset()
    {
        
    }

    public virtual void ResetAction()
    {
        
    }
}
