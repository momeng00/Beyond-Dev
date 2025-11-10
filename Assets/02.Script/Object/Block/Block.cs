using System;
using UnityEngine;

public abstract class Block : MonoBehaviour, IReset
{
    protected Collider2D col;
    protected Rigidbody2D rb;
    public Action<bool> blockEvent;
    protected SpriteMask mask;

    public virtual void Start()
    {
        mask = GetComponent<SpriteMask>();
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
