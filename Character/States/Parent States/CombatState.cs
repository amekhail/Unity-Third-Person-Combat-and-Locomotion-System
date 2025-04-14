using UnityEngine;

public class CombatState : ParentStateWithChildren
{
   private float _combatTimer = 0f;
   private const float COMBAT_TIMEOUT = 5f;
   private float enemyCheckRadius = 10f;

   public CombatState(CharacterContext context, CharacterStateMachine stateMachine) : base(context, stateMachine)
   {
   }

   public override void Enter()
   {
      base.Enter();
      Debug.Log("Enter CombatState");
      Context.animator.SetTrigger("Unsheathe");
      
      SetSubState(new CombatLocomotionState(Context, StateMachine));
   }

   public override void LogicUpdate()
   {
      base.LogicUpdate();
      _combatTimer += Time.deltaTime;

      if (IsNearEnemies())
      {
         _combatTimer = 0f;
      }

      if (_combatTimer > COMBAT_TIMEOUT)
      {
         Debug.Log("Exiting CombatState");
         Context.animator.SetTrigger("Sheathe");
         StateMachine.ChangeState(new LocomotionState(Context, StateMachine));
      }
      
      if (Context.playerInputHandler.IsLightAttackPressed)
      {
         StateMachine.ChangeState(new LightAttackState(Context, StateMachine));
      }
   }

   private bool IsNearEnemies()
   {
      Collider[] hits = Physics.OverlapSphere(Context.transform.position, enemyCheckRadius, Context.enemyLayerMask);
      return hits.Length > 0;
   }

}
