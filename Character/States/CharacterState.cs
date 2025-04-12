

public abstract class CharacterState
{
    protected CharacterContext Context;
    protected CharacterStateMachine StateMachine;

    public CharacterState(CharacterContext context, CharacterStateMachine stateMachine)
    {
        Context = context;
        StateMachine = stateMachine;
    }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void HandleInput()
    {
    }

    public virtual void LogicUpdate()
    {
    }

    public virtual void PhysicsUpdate()
    {
    }
}
