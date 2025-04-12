using System;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class CharacterContext : MonoBehaviour
{

    [Header("Core Components")]
    public PlayerInputHandler playerInputHandler;
    public Animator animator;
    public CharacterController characterController;
   
    public CharacterStateMachine characterStateMachine;

    [Header("Movement")] 
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float rotationSpeed = 10f;


    private void Awake()
    {
        characterStateMachine = new CharacterStateMachine();
        characterController = GetComponent<CharacterController>();
        

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
        characterStateMachine.Update();
    }

    private void FixedUpdate()
    {
        characterStateMachine.FixedUpdate();
    }
}
