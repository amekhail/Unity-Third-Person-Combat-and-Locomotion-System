
public class ParentStateWithChildren : CharacterState
{
        protected CharacterState currentSubState;

        public ParentStateWithChildren(CharacterContext context, CharacterStateMachine stateMachine) 
                : base(context, stateMachine)
        {
        }

        protected void SetSubState(CharacterState subState)
        {
                currentSubState?.Exit();
                currentSubState = subState;
                currentSubState.Enter();
        }

        public override void HandleInput()
        {
                currentSubState?.HandleInput();
        }

        public override void LogicUpdate()
        {
                currentSubState?.LogicUpdate();
        }

        public override void PhysicsUpdate()
        {
                currentSubState?.PhysicsUpdate();
        }

        public override void Exit()
        {
                currentSubState.Exit();
        }
}
