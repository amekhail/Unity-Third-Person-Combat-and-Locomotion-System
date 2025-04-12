
using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerControls _controls;
    
    public Vector2 MoveInput {get; private set;}
    public bool IsRunning {get; private set;}

    private void Awake()
    {
        _controls = new PlayerControls();
        
        _controls.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += _ => MoveInput = Vector2.zero;
        
        _controls.Player.Run.performed += ctx => IsRunning = true;
        _controls.Player.Run.canceled += _ => IsRunning = false;
    }
    
    private void OnDisable() => _controls.Disable();
    private void OnEnable() => _controls.Enable();

}
