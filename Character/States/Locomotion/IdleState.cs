using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : CharacterState
{
    private LocomotionState _parent;
    
    public IdleState(CharacterContext context, CharacterStateMachine stateMachine, LocomotionState parent) 
        : base(context, stateMachine)
    {
        _parent = parent;
    }

    public override void Enter()
    {
        Context.animator.SetFloat("Speed", 0f);
        
    }

    public override void LogicUpdate()
    {
        
        DampToIdle();
        // TODO: Implement breathing animations, turn-in-place animations
        // TODO: 
        
    }

    private void DampToIdle()
    {
        Context.animator.SetFloat("Speed", 0, 0.1f ,Time.deltaTime);
        Context.animator.SetFloat("Horizontal", 0, 0.1f ,Time.deltaTime);
        Context.animator.SetFloat("Vertical", 0,0.1f ,Time.deltaTime);
    }
}
