using UnityEngine;

public class ConditionItem : ClearCondition, IReset
{
    //OpenDoorItem이 대신하고 있음
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
            OnCheck?.Invoke();
            satisfied = value;
        }
    }
    virtual protected bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((1 << obj.layer) & mask) != 0;
    }
    public void ResetAction()
    {
        Satisfied = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void InitializeReset()
    {
        throw new System.NotImplementedException();
    }
}