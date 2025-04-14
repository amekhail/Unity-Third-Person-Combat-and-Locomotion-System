using UnityEngine;

public class LightAttackState : CharacterState
{
    public LightAttackState(CharacterContext context, CharacterStateMachine stateMachine) : base(context, stateMachine)
    {
    }

    private bool hasAttacked = false;

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Entered Light Attack!");
        hasAttacked = false;
        Context.animator.SetTrigger("LightAttack");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        AnimatorStateInfo stateInfo = Context.animator.GetCurrentAnimatorStateInfo(0);
        
        // Lets prevent Spamming the button
        if (!hasAttacked && stateInfo.IsTag("Attack"))
        {
            hasAttacked = true;
        }
        if (hasAttacked && !stateInfo.IsTag("Attack"))
        {
            StateMachine.ChangeState(new CombatLocomotionState(Context, StateMachine));
        }
        
    }
    
}
