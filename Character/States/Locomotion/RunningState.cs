using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningState : CharacterState
{
    private LocomotionState _parent;
    
    public RunningState(CharacterContext context, CharacterStateMachine stateMachine, LocomotionState parent) : base(context, stateMachine)
    {
        _parent = parent;
    }

    public override void LogicUpdate()
    {
        Context.MoveCharacter(Context.playerInputHandler.MoveInput, Context.runSpeed);
    }
    
   
}
