using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingState : CharacterState
{
    public WalkingState(CharacterContext context, CharacterStateMachine stateMachine)
        : base(context, stateMachine)
    {
    }
    
    public override void LogicUpdate()
    {
        Move(Context.walkSpeed);

        var input = Context.playerInputHandler.MoveInput;

        if (input != Vector2.zero) 
        {
            if (Context.playerInputHandler.IsRunning)
            {
                StateMachine.ChangeState(new RunningState(Context, StateMachine));
            }
        }
        else
        {
            StateMachine.ChangeState(new IdleState(Context, StateMachine));
        }

    }

    private void Move(float speed)
    {
        var input = Context.playerInputHandler.MoveInput;
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = Camera.main.transform.TransformDirection(move);
        move.y = 0;
        
        Context.characterController.Move(move.normalized * speed * Time.deltaTime);

        if (move != Vector3.zero)
        {
            Context.transform.forward = Vector3.Slerp(Context.transform.forward, move.normalized, Time.deltaTime * Context.rotationSpeed);
        }
        
        Vector3 localMove = Context.transform.InverseTransformDirection(move.normalized);

        float moveAmount = move.magnitude;
        
        Context.animator.SetFloat("Horizontal", localMove.x, 0.1f, Time.deltaTime);
        Context.animator.SetFloat("Vertical", localMove.z, 0.1f, Time.deltaTime);
        Context.animator.SetFloat("Speed", Context.playerInputHandler.IsRunning ? 1f: 0.5f, 0.1f, Time.deltaTime);
    }
}
