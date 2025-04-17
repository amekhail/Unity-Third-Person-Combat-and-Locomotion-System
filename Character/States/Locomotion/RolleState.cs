using UnityEngine;

public class RolleState : CharacterState
{
    private Camera _mainCamera;
    
    public RolleState(CharacterContext context, CharacterStateMachine stateMachine) : base(context, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        _mainCamera ??= Camera.main;
        
        Vector2 input = Context.playerInputHandler.MoveInput;

        if (input.sqrMagnitude < 0.1f)
        {
            Context.animator.CrossFade("Roll Forward", 0.2f);
            return;
        }

        // Convert input to world space direction based on camera
        Vector3 camForward = _mainCamera.transform.forward;
        Vector3 camRight = _mainCamera.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * input.y + camRight * input.x).normalized;

        // Get angle between moveDir and character forward
        float angle = Vector3.SignedAngle(Context.transform.forward, moveDir, Vector3.up);

        // Map angle to roll direction
        if (angle >= -45f && angle <= 45f)
        {
            Context.animator.CrossFade("Roll Forward", 0.2f);
        }
        else if (angle > 45f && angle <= 135f)
        {
            Context.animator.CrossFade("Roll Right", 0.2f);
        }
        else if (angle < -45f && angle >= -135f)
        {
            Context.animator.CrossFade("Roll Left", 0.2f);
        }
        else
        {
            Context.animator.CrossFade("Roll Backward", 0.2f);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        AnimatorStateInfo actionLayerInfo = Context.animator.GetCurrentAnimatorStateInfo(1);

        if (!actionLayerInfo.IsTag("Roll"))
        {
            StateMachine.ChangeState(new LocomotionState(Context, StateMachine));
        }
    }
}
