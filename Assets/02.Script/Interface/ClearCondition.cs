using System;
using UnityEngine;

//List에 IClearCondition을 넣으니 인스펙터 창에서 안나와 만든 추상클래스
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