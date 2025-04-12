using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : CharacterState
{
    public IdleState(CharacterContext context, CharacterStateMachine stateMachine) 
        : base(context, stateMachine)
    {
    }

    public override void Enter()
    {
        
        
    }

    public override void LogicUpdate()
    {
        DampToIdle();
        
        var input = Context.playerInputHandler.MoveInput;
        if (input != Vector2.zero)
        {
            if (Context.playerInputHandler.IsRunning)
            {
                StateMachine.ChangeState(new RunningState(Context, StateMachine));
            }
            else
            {
                StateMachine.ChangeState(new WalkingState(Context, StateMachine));
            }
            
        }
        
    }

    private void DampToIdle()
    {
        Context.animator.SetFloat("Speed", 0, 0.1f ,Time.deltaTime);
        Context.animator.SetFloat("Horizontal", 0, 0.1f ,Time.deltaTime);
        Context.animator.SetFloat("Vertical", 0,0.1f ,Time.deltaTime);
    }
}
