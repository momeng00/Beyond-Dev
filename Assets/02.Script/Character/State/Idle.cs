using Unity.VisualScripting;
using UnityEngine;

public class Idle: CharacterStateBase
{
    public Idle(CharacterAnimation animation)
    {
        machine = animation;
    }
    public override bool canExecute => base.canExecute;
    public override void EnterState()
    {
        base.EnterState();
        machine.characterControl.hasJump = false;
        machine.characterControl.canJump = true;
        //초기화와 같은 방식
        Debug.Log("Idle 상태");
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
        else
        {
            if (machine.rb.linearVelocityY > 0f)
            {
                next = CharacterStateID.Jump;
            }
            else if (machine.rb.linearVelocityY < 0f)
            {
                next = CharacterStateID.Falling;
            }
        }
        return next;
    }
    
}