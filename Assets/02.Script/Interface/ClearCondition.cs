using System;
using UnityEngine;

public abstract class ClearCondition : MonoBehaviour, IClearCondition
{
    public Action OnCheck;
    public virtual void ClearAction()
    {
        
    }

    public virtual bool IsSatisfied()
    {
        throw new System.NotImplementedException();
    }
    public virtual void AddOnCheck(Action act)
    {
        OnCheck += act;
    }
}