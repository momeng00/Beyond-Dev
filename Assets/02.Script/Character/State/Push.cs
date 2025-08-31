using UnityEngine;

public class Push : CharacterStateBase
{
    private float speed;
    public Push(CharacterAnimation animation)
    {
        machine = animation;

    }
    public override void EnterState()
    {
        base.EnterState();
        machine.GetComponent<Animator>().Play("Pull");
        speed = machine.characterControl.currentStat.moveSpeed;
        machine.characterControl.currentStat.moveSpeed = machine.characterControl.currentStat.moveSpeed * 0.36f;
    }
    public override void ExitState()
    {
        base.ExitState();
        machine.characterControl.currentStat.moveSpeed = speed;
    }
    public override CharacterStateID OnUpdateState()
    {
        CharacterStateID next = CharacterStateID.Push;
        if(Mathf.Abs(machine.characterControl._axisX) <= 0.03f)
        {
            next=CharacterStateID.Idle;
        }
        if(machine.rb.linearVelocityY >= 0.03f)
        {
            next=CharacterStateID.Jump;
        }
        return next;
    }
}