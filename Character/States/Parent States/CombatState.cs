using UnityEngine;

public class CombatState : CharacterState
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
      StateMachine.ChangeState(new CombatLocomotionState(Context, StateMachine));
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
         StateMachine.ChangeState(new LocomotionState(Context, StateMachine));
         // TODO: Add auto sheathing weapon
      }
   }

   private bool IsNearEnemies()
   {
      Collider[] hits = Physics.OverlapSphere(Context.transform.position, enemyCheckRadius, Context.enemyLayerMask);
      return hits.Length > 0;
   }

}
