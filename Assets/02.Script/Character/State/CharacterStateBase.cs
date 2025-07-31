using UnityEngine;

public abstract class CharacterStateBase 
{
    
    public CharacterAnimation machine;
    public virtual bool canExecute => true;
    public virtual void EnterState()
    {

    }
    public virtual void ExitState() 
    {
        
    }
    public virtual CharacterStateID OnUpdateState()
    {
        return CharacterStateID.Idle;
    }
}