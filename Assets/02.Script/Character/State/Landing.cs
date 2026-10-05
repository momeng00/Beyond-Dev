using UnityEngine;

public class Landing : CharacterStateBase
{
    public override bool canExecute => base.canExecute;
    public Landing(CharacterAnimation machine)
    {
        this.machine = machine;
    }
    public override void EnterState()
    {
        base.EnterState();
        machine.GetComponent<Animator>().Play("Landing");
    }
    public override CharacterStateID OnUpdateState()
    {
        if(machine.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            return CharacterStateID.Landing;
        }
        return base.OnUpdateState();
    }
}