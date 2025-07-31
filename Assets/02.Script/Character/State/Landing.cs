using UnityEngine;

public class Landing : CharacterStateBase
{
    public override CharacterStateID OnUpdateState()
    {
        if(machine.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            return CharacterStateID.Landing;
        }
        return base.OnUpdateState();
    }
}