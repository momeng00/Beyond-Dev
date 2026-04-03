using UnityEngine;

public class ConditionSpot : ClearCondition
{
    public Collider2D col;
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
        Satisfied = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        satisfied = false;
    }
    public override bool IsSatisfied()
    {
        return satisfied;
    }
}