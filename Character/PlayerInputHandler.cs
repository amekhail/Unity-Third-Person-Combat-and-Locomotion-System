
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerControls _controls;
    
    public Vector2 MoveInput {get; private set;}
    public Vector2 LookInput {get; private set;}
    public bool IsRunning {get; private set;}
    public bool IsLockOnPressed {get; private set;}

    public bool IsLightAttackPressed => _isLightAttackPressed;
    private bool _isLightAttackPressed;

    public bool IsRollPressed => _isRollPressed;
    private bool _isRollPressed;
    
    public bool IsSheathePressed {get; private set;}
    

    private void Awake()
    {
        _controls = new PlayerControls();
        
        // Movement
        _controls.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += _ => MoveInput = Vector2.zero;
        
        // Running
        _controls.Player.Run.performed += ctx => IsRunning = true;
        _controls.Player.Run.canceled += _ => IsRunning = false;
        
        // Rolling
        _controls.Player.Roll.performed += OnRoll;
        
        //Lock-on
        _controls.Player.LockOn.performed += ctx => IsLockOnPressed = true;
        _controls.Player.LockOn.canceled += _ => IsLockOnPressed = false;
        
        // Camera Look
        _controls.Player.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        _controls.Player.Look.canceled += _ => LookInput = Vector2.zero;
        
        // Attack
        _controls.Player.LeftClick.performed += OnLightAttack;
        
        // Sheathing
        _controls.Player.Sheath.performed += OnSheath;



    }

    public void OnLightAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isLightAttackPressed = true;
        }
    }

    public void ResetLightAttack()
    {
        _isLightAttackPressed = false;
    }

    public void OnSheath(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsSheathePressed = true;
        }
    }

    public void ResetSheathe()
    {
        IsSheathePressed = false;
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isRollPressed = true;
        }
    }

    public void ResetRoll()
    {
        _isRollPressed = false;
    }

    private void OnDisable() => _controls.Disable();
    private void OnEnable() => _controls.Enable();
    public void ResetLockOn()
    {
        IsLockOnPressed = false;
    }

}
