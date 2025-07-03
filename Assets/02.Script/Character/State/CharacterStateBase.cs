using UnityEngine;

public abstract class CharacterStateBase : MonoBehaviour
{
    public virtual bool canExecute => true;
    public virtual void EnterState()
    {

    }
    public virtual void ExitState() 
    {
        
    }
    public virtual void OnUpdateState()
    {

    }
}