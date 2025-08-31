using UnityEngine;

public class Jump : CharacterStateBase
{
    public override bool canExecute => base.canExecute;
    public Jump(CharacterAnimation animation)
    {
        machine = animation;
    }
    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Jump ป๓ลย");
        machine.GetComponent<Animator>().Play("Jump");
        machine.characterControl.hasJump = true;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override CharacterStateID OnUpdateState()
    {
        CharacterStateID next = CharacterStateID.Jump;
        if (machine.rb.linearVelocityY < 0)
        {
            next = CharacterStateID.Falling;
        }
        if (machine.characterControl.isGrounded)
        {
            next = CharacterStateID.Idle;
        }

        return next;
    }
}