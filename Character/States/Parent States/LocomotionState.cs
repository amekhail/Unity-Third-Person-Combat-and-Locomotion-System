using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionState : ParentStateWithChildren
{
    public LocomotionState(CharacterContext context, CharacterStateMachine stateMachine) 
        : base(context, stateMachine)
    {
    }

    public override void Enter()
    {
        DetermineSubState();
    }

    private void DetermineSubState()
    {
        if (Context.playerInputHandler.MoveInput == Vector2.zero)
        {
            SetSubState(new IdleState(Context, StateMachine, this));
        } else if (Context.playerInputHandler.IsRunning)
            SetSubState(new RunningState(Context, StateMachine, this));
        else
            SetSubState(new WalkingState(Context, StateMachine, this));
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        HandleLockRotation();
        UpdateSubStateIfNeeded();
    }

    private void UpdateSubStateIfNeeded()
    {
        var input = Context.playerInputHandler.MoveInput;

        if (currentSubState is IdleState && input != Vector2.zero)
        {
            DetermineSubState();
        } else if (currentSubState is WalkingState && Context.playerInputHandler.IsRunning)
        {
            DetermineSubState();
        } else if (currentSubState is RunningState && !Context.playerInputHandler.IsRunning)
        {
            DetermineSubState();
        } else if (input == Vector2.zero && currentSubState is not IdleState)
        {
            DetermineSubState();
        }
    }

    private void HandleLockRotation()
    {
        Context.FaceTarget(Context.lockOnSystem.currentTarget, Context.rotationSpeed);
    }
}
