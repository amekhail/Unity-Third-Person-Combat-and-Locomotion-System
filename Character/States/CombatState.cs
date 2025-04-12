using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatState : CharacterState
{
    public bool isAttacking;

    public CombatState(CharacterContext context, CharacterStateMachine stateMachine)
        : base(context, stateMachine)
    {
    }

    public override void HandleInput()
    {
        
    }

    public void OnAttackComplete()
    {
        isAttacking = false;
    }

    public override void LogicUpdate()
    {
        // TODO: Add transition out of combat when not in combat for a certain amount of time
    }

}
