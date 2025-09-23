using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Falling : CharacterStateBase
{
    private float time;
    private float startFall;
    public override bool canExecute => base.canExecute;
    public Falling(CharacterAnimation machine)
    {
        this.machine = machine;
    }
    public override void EnterState()
    {
        base.EnterState();
        machine.GetComponent<Animator>().Play("Falling");
        machine.characterControl.jumpTime = -1;
        time = 0f;
        startFall = machine.characterControl.transform.position.y;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override CharacterStateID OnUpdateState()
    {
        CharacterStateID next = CharacterStateID.Falling;
        time += Time.deltaTime;
        if (time > machine.characterControl.coyoteTime)
        {
            machine.characterControl.canJump = false;
        }
        if (machine.rb.linearVelocityY > 0.03f)
        {
            next = CharacterStateID.Jump;
        }
        if (machine.characterControl.isGrounded)
        {
            if (machine.characterControl.landingLimit < startFall - machine.characterControl.transform.position.y)
            {
                return CharacterStateID.Landing;
            }
            if (machine.characterControl.jumpTime >= 0)
            {
                machine.rb.linearVelocity = Vector2.zero;
                machine.rb.AddForce(Vector2.up * machine.characterControl.currentStat.jumpForce, ForceMode2D.Impulse);
                machine.characterControl.jumpTime = -1;
                return CharacterStateID.Jump;
                
            }
            next = CharacterStateID.Idle;
        }
        return next;
    }
}