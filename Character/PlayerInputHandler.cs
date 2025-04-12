
using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerControls _controls;
    
    public Vector2 MoveInput {get; private set;}
    public Vector2 LookInput {get; private set;}
    public bool IsRunning {get; private set;}
    public bool IsLockOnPressed {get; private set;}
    

    private void Awake()
    {
        _controls = new PlayerControls();
        
        // Movement
        _controls.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += _ => MoveInput = Vector2.zero;
        
        // Running
        _controls.Player.Run.performed += ctx => IsRunning = true;
        _controls.Player.Run.canceled += _ => IsRunning = false;
        
        //Lock-on
        _controls.Player.LockOn.performed += ctx => IsLockOnPressed = true;
        _controls.Player.LockOn.canceled += _ => IsLockOnPressed = false;
        
        // Camera Look
        _controls.Player.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        _controls.Player.Look.canceled += _ => LookInput = Vector2.zero;
    }

    private void OnDisable() => _controls.Disable();
    private void OnEnable() => _controls.Enable();

    public void ResetLockOn()
    {
        IsLockOnPressed = false;
    }

}
