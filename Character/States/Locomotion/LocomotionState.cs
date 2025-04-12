using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionState : CharacterState
{
    public LocomotionState(CharacterContext context, CharacterStateMachine stateMachine) 
        : base(context, stateMachine)
    {
    }

    public override void LogicUpdate()
    {
        var input = Context.playerInputHandler.MoveInput;
        
        if (input == Vector2.zero)
        {
            StateMachine.ChangeState(new IdleState(Context, StateMachine));
        } else if (Context.playerInputHandler.IsRunning)
        {
            StateMachine.ChangeState(new RunningState(Context, StateMachine));
        }
        else
        {
            StateMachine.ChangeState(new WalkingState(Context, StateMachine));
        }
    }
}
