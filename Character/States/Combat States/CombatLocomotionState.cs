using UnityEngine;

public class CombatLocomotionState : CharacterState
{
    public CombatLocomotionState(CharacterContext context, CharacterStateMachine stateMachine) : base(context,
        stateMachine)
    {
    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Vector2 input = Context.playerInputHandler.MoveInput;
        bool isRunning = Context.playerInputHandler.IsRunning;

        if (input == Vector2.zero)
        {
            Context.animator.SetFloat("Vertical", 0f, 0.2f, Time.deltaTime);
            Context.animator.SetFloat("Horizontal", 0f, 0.2f, Time.deltaTime);
            return;
        }

        Vector3 moveDir = new Vector3(input.x, 0, input.y);
        moveDir = Quaternion.Euler(0, Context.transform.eulerAngles.y, 0) * moveDir;

        float speed = isRunning ? Context.runSpeed : Context.walkSpeed;
        Context.characterController.Move(moveDir.normalized * speed * Time.deltaTime);

        Context.animator.SetFloat("Vertical", input.y, 0.2f, Time.deltaTime);
        Context.animator.SetFloat("Horizontal", input.x, 0.2f, Time.deltaTime);
    }
    
}
