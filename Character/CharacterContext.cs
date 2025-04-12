using System;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class CharacterContext : MonoBehaviour
{

    [Header("Core Components")]
    public PlayerInputHandler playerInputHandler;
    public LockOnSystem lockOnSystem;
    public Animator animator;
    public CharacterController characterController;
   
    public CharacterStateMachine characterStateMachine;

    [Header("Movement")] 
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float rotationSpeed = 10f;
    
    [Header("Lock On Settings")]
    private float lookInputThreshold = 0.7f;
    private float lookCoolDown = 0.4f;
    private float lastLookTime = -1f;


    private void Awake()
    {
        characterStateMachine = new CharacterStateMachine();
        characterController = GetComponent<CharacterController>();
        lockOnSystem = GetComponent<LockOnSystem>();
        

        if (playerInputHandler == null)
        {
            playerInputHandler = GetComponentInParent<PlayerInputHandler>();
        }

    }

    private void Start()
    {
        characterStateMachine.Initialize(new LocomotionState(this, characterStateMachine));
    }
    
    private void Update()
    {
        if (playerInputHandler.IsLockOnPressed)
        {
            lockOnSystem.ToggleLockOn();
            playerInputHandler.ResetLockOn();
        }

        float lookX = playerInputHandler.GetComponentIndex();

        if (lockOnSystem.currentTarget != null && Time.time - lastLookTime < lookCoolDown)
        {
            if (lookX > lookInputThreshold)
            {
                lockOnSystem.CycleTarget(true); // to the right
                lastLookTime = Time.time;
            } else if (lookX < -lookInputThreshold)
            {
                lockOnSystem.CycleTarget(false); // to the left
                lastLookTime = Time.time;
            } 
        }
        characterStateMachine.Update();
    }
    
    // Called from states to move character
    public void MoveCharacter(Vector2 input, float speed)
    {
        if (input == Vector2.zero) return;
        
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = Camera.main.transform.TransformDirection(move);
        move.y = 0;
        
        characterController.Move(move.normalized * speed * Time.deltaTime);
        
        Vector3 localMove = transform.InverseTransformDirection(move.normalized);
        
        animator.SetFloat("Horizontal", localMove.x, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", localMove.z, 0.1f, Time.deltaTime);
        animator.SetFloat("Speed", playerInputHandler.IsRunning ? 1f: 0.5f, 0.1f, Time.deltaTime);

        if (lockOnSystem.currentTarget == null && move != Vector3.zero) 
        {
            Quaternion targetRotation = Quaternion.LookRotation(move.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void FaceTarget(Transform target, float rotationSpeed)
    {
        if (target == null) return;
        if (lockOnSystem.currentTarget != null)
        {
            Vector3 dir = lockOnSystem.currentTarget.position - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * rotationSpeed
                );
            }
        }
    }

    private void FixedUpdate()
    {
        characterStateMachine.FixedUpdate();
    }
}
