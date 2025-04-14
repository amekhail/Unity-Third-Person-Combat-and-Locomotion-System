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

        if (Context.playerInputHandler.IsLightAttackPressed)
        {
            Context.playerInputHandler.ResetLightAttack();
            StateMachine.ChangeState(new LightAttackState(Context, StateMachine));
            return;
        }

        if (Context.playerInputHandler.IsSheathePressed)
        {
            Context.playerInputHandler.ResetSheathe();
            Context.animator.SetTrigger("Sheathe");
            StateMachine.ChangeState(new LocomotionState(Context, StateMachine));
            return;
        }

        if (input == Vector2.zero)
        {
            Context.animator.SetFloat("Vertical", 0f, 0.2f, Time.deltaTime);
            Context.animator.SetFloat("Horizontal", 0f, 0.2f, Time.deltaTime);
            return;
        }

        // Camera-relative movement
        Transform cameraTransform = Camera.main.transform;
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * input.y + camRight * input.x;

        float speed = isRunning ? Context.runSpeed : Context.walkSpeed;
        Context.characterController.Move(moveDir.normalized * speed * Time.deltaTime);

        // Apply animation movement values
        Context.animator.SetFloat("Vertical", input.y, 0.2f, Time.deltaTime);
        Context.animator.SetFloat("Horizontal", input.x, 0.2f, Time.deltaTime);

        // Handle rotation
        if (Context.lockOnSystem != null && Context.lockOnSystem.currentTarget != null)
        {
            Vector3 targetDir = Context.lockOnSystem.currentTarget.position - Context.transform.position;
            targetDir.y = 0f;

            if (targetDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDir);
                Context.transform.rotation = Quaternion.Slerp(
                    Context.transform.rotation,
                    targetRotation,
                    Time.deltaTime * Context.rotationSpeed
                );
            }
        }
        else if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            Context.transform.rotation = Quaternion.Slerp(
                Context.transform.rotation,
                targetRotation,
                Time.deltaTime * Context.rotationSpeed
            );
        }
    }
}
