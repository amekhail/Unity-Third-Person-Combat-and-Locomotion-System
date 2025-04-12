using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
  [Header("References")] 
  public Transform followTarget;
  public LockOnSystem lockOnSystem;
  public PlayerInputHandler playerInput;

  [Header("General")] 
  public float followDistance = 6f;
  public float heightOffset = 2f;

  [Header("Free Look")] 
  public float lookSpeed = 120f;
  public float pitchMin = -30f;
  public float pitchMax = 60f;
  
  [Header("Lock On")]
  public float lockSmoothTime = 0.1f;

  private float _yaw;
  private float _pitch;
  private Vector3 _currentVelocity;

  private void Start()
  {
    // Hide cursor
    
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    
    Vector3 angles = transform.eulerAngles;
    _yaw = angles.y;
    _pitch = angles.x;

    if (playerInput == null && followTarget != null)
    {
      playerInput = followTarget.GetComponent<PlayerInputHandler>();
    }
    
  }

  private void LateUpdate()
  {
    if (playerInput != null && lockOnSystem.currentTarget != null)
    {
      HandleLockOn();
    }
    else
    {
      HandleFreeLook();
    }
  }

  private void HandleFreeLook()
  {
    if (playerInput == null || followTarget == null) return;
    
    Vector2 input = playerInput.LookInput;
    
    _yaw += input.x * lookSpeed * Time.deltaTime;
    _pitch -= input.y * lookSpeed * Time.deltaTime;
    _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);
    
    Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    Vector3 desiredPostion = followTarget.position - (rotation * Vector3.forward * followDistance) + Vector3.up * heightOffset;
    
    transform.position = Vector3.SmoothDamp(transform.position, desiredPostion, ref _currentVelocity, 0.05f);
    transform.LookAt(followTarget.position, Vector3.up * 1.5f);
    
  }

  private void HandleLockOn()
  {
    Transform target = lockOnSystem.currentTarget;
    if (!target || !followTarget)
    {
      return;
    }
    
    Vector3 lockPoint = (followTarget.position + target.position) / 2;
    Vector3 direction = (followTarget.position - target.position).normalized;
    
    Vector3 desiredPostion = lockPoint + direction * followDistance + Vector3.up * heightOffset;
    
    transform.position = Vector3.Lerp(transform.position, desiredPostion, lockSmoothTime);
    transform.LookAt(lockPoint);
    
    
  }
  
}
