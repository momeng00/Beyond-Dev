using UnityEngine;

public class ConditionSpot : ClearCondition
{
    public Collider2D col;
    public LayerMask layerMask;
    private bool satisfied = false;
    public bool Satisfied
    {
        get
        {
            return satisfied;
        }
        set
        {
            satisfied = value;
            OnCheck?.Invoke();
        }
    }
    private void Start()
    {
        col = GetComponent<Collider2D>();
        
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogError("Collider°¡ ¾øÀ½");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            Satisfied = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Satisfied = false;
    }
    public override bool IsSatisfied()
    {
        return Satisfied;
    }
    virtual protected bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((1 << obj.layer) & mask) != 0;
    }
}