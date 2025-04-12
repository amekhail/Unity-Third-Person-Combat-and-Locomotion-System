using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
  [Header("References")]
    public Transform followTarget;
    public Transform cameraPivot;
    public Transform cameraTransform;
    public LockOnSystem lockOnSystem;
    public PlayerInputHandler input;

    [Header("Settings")]
    public float followSpeed = 10f;
    public float yawSpeed = 120f;
    public float pitchSpeed = 120f;
    public float minPitch = -30f;
    public float maxPitch = 60f;
    
    [Header("FOV Settings")]
    public Camera mainCam;
    public float normalFOV = 60f;
    public float lockOnFOV = 50f;
    public float fovLerpSpeed = 6f;

    [Header("Camera Positioning")]
    public float defaultDistance = 5f;
    public float cameraHeight = 1.5f;
    public float lateralOffset = -1.5f; // negative for left offset
    
    [Header("Lock-On Settings")]
    public float lockBlendSpeed = 7f;
    public float lockVerticalOffset = 1.5f;

    [Header("Collision")]
    public LayerMask collisionMask;
    public float sphereRadius = 0.3f;
    public float minDistance = 1.5f;

    private float yaw;
    private float pitch;
    
    // Cinematic!
    private bool isCinematicLockingOn = false;
    private float cinematicTimer = 0f;
    public float cinematicSweepDuration = 0.5f;
    public float cinematicYawOffset = 40f;

    void LateUpdate()
    {
        if (followTarget == null || input == null) return;

        // Follow player root smoothly
        transform.position = Vector3.Lerp(transform.position, followTarget.position, Time.deltaTime * followSpeed);
        
        bool isLockingOn = lockOnSystem != null && lockOnSystem.currentTarget != null;

        if (isLockingOn && !isCinematicLockingOn)
        {
            isCinematicLockingOn = true;
            cinematicTimer = 0f;
        }
        else if (!isLockingOn)
        {
            isCinematicLockingOn = false;
        }
        
        
        // Yaw + Pitch Input
        if (lockOnSystem != null && lockOnSystem.currentTarget != null)
        {
            HandleLockOnTracking();
        }
        else
        {
            Vector2 look = input.LookInput;
            yaw += look.x * yawSpeed * Time.deltaTime;
            pitch -= look.y * pitchSpeed * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        // Apply rotations
        transform.rotation = Quaternion.Euler(0f, yaw, 0f); // Yaw on holder
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f); // Pitch on pivot

        // Determine camera target position
        Vector3 targetOffset = cameraPivot.right * lateralOffset - cameraPivot.forward * defaultDistance;
        Vector3 targetPosition = cameraPivot.position + targetOffset;

        // Handle collision
        Vector3 collisionPosition = HandleCameraCollision(cameraPivot.position, targetPosition);
        cameraTransform.position = collisionPosition;

        // Look at pivot (or use custom look target)
        cameraTransform.LookAt(cameraPivot.position);
        
        float targetFOV = (lockOnSystem != null && lockOnSystem.currentTarget != null) ? lockOnFOV : normalFOV;
        mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);
    }

    Vector3 HandleCameraCollision(Vector3 from, Vector3 to)
    {
        Vector3 dir = to - from;
        float distance = dir.magnitude;

        if (Physics.SphereCast(from, sphereRadius, dir.normalized, out RaycastHit hit, distance, collisionMask))
        {
            return from + dir.normalized * (hit.distance - 0.1f);
        }

        return to;
    }

    private void HandleLockOnTracking()
    {
        if (lockOnSystem.currentTarget == null) return;

        // Get head-height of player & target
        Vector3 playerHead = followTarget.position + Vector3.up * 1.5f;
        Vector3 targetHead = lockOnSystem.currentTarget.position + Vector3.up * 1.5f;

        // Midpoint for camera to aim at
        Vector3 to = (playerHead + targetHead) / 2f;

        Vector3 dir = to - cameraPivot.position;
        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion lookRotation = Quaternion.LookRotation(dir);

        float targetYaw = lookRotation.eulerAngles.y;
        float targetPitch = 10f; // keep pitch level & slightly angled down (natural combat angle)

        // Wrap yaw correctly
        if (targetYaw - yaw > 180f) targetYaw -= 360f;
        else if (yaw - targetYaw > 180f) targetYaw += 360f;

        if (isCinematicLockingOn && cinematicTimer < cinematicSweepDuration)
        {
            float t = cinematicTimer / cinematicSweepDuration;
            float sweepOffset = Mathf.Lerp(cinematicYawOffset, 0f, t);
            yaw = Mathf.LerpAngle(yaw, targetYaw + sweepOffset, Time.deltaTime * (lockBlendSpeed * 2f));
            cinematicTimer += Time.deltaTime;
        }
        else
        {
            yaw = Mathf.LerpAngle(yaw, targetYaw, Time.deltaTime * lockBlendSpeed);
        }

        // ✅ Don't calculate pitch dynamically — just hold a comfortable default angle
        pitch = Mathf.LerpAngle(pitch, targetPitch, Time.deltaTime * lockBlendSpeed);
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }
  
}
