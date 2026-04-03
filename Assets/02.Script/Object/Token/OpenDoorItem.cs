using System;
using UnityEngine;

public class OpenDoorItem : ClearCondition, IReset
{
    private bool satisfied = false;
    public bool Satisfied
    {
        get 
        {
            OnCheck?.Invoke();
            return satisfied; 
        }
        set
        {
            OnCheck?.Invoke();
            satisfied = value;
        }
    }
    public LayerMask layerMask;
    private void Start()
    {
        GameManager.Instance.OnReset += ResetAction;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    public override bool IsSatisfied()
    {
        return Satisfied;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            Satisfied = true;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
        
    }
    virtual protected bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((1 << obj.layer) & mask) != 0;
    }

    public void InitializeReset()
    {
        throw new System.NotImplementedException();
    }

    public void ResetAction()
    {
        Satisfied = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    public override void ClearAction()
    {
        throw new NotImplementedException();
    }
}