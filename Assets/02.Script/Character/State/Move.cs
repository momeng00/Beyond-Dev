using UnityEngine;

public class Move : CharacterStateBase
{
    public Move(CharacterAnimation animation)
    {
        machine = animation;

    }
    public override bool canExecute => base.canExecute && machine.characterControl.isGrounded;
    public override void EnterState()
    {
        base.EnterState();
        machine.GetComponent<Animator>().Play("Walk");
        //초기화와 같은 방식
        Debug.Log("Move 상태");
    }
    public override void ExitState()
    {
        base.ExitState();
        //Enter의 역
    }
    public override CharacterStateID OnUpdateState()
    {
        CharacterStateID next = CharacterStateID.Idle;

        if (machine.characterControl.isGrounded)
        {
            if (Mathf.Abs(machine.characterControl._axisX) > 0)
            {
                next = CharacterStateID.Move;
            }
        }
        if (!machine.characterControl.isGrounded)
        {
            if (machine.rb.linearVelocityY > 0.01f)
                return CharacterStateID.Jump;
            if (machine.rb.linearVelocityY < -0.01f)
                return CharacterStateID.Falling;
        }
        return next;
    }
}