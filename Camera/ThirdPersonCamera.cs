using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
  [Header("References")]
    public Transform followTarget;               // Target to follow (usually player)
    public Transform cameraPivot;                // Pivot for pitch control
    public Transform cameraTransform;            // Actual camera transform
    public LockOnSystem lockOnSystem;
    public PlayerInputHandler input;

    [Header("Camera Movement")]
    public float followSpeed = 10f;
    public float yawSpeed = 120f;
    public float pitchSpeed = 120f;
    public float minPitch = -30f;
    public float maxPitch = 60f;

    [Header("Positioning")]
    public float defaultDistance = 5f;
    public float cameraHeight = 1.5f;
    public float lateralOffset = -1.5f; // Negative = left of player

    [Header("Collision")]
    public LayerMask collisionMask;
    public float sphereRadius = 0.3f;
    public float minDistance = 1.5f;

    [Header("FOV")]
    public Camera mainCam;
    public float normalFOV = 60f;
    public float lockOnFOV = 50f;
    public float fovLerpSpeed = 6f;

    [Header("Cinematic Lock-On")]
    public float lockBlendSpeed = 7f;
    public float lockVerticalOffset = 1.5f;
    public float cinematicSweepDuration = 0.5f;
    public float cinematicYawOffset = 40f;

    private bool isCinematicLockingOn = false;
    private float cinematicTimer = 0f;

    [Header("Cinematic Unlock")]
    public float unlockSweepDuration = 0.5f;
    public float unlockYawOffset = -30f;
    private bool isCinematicUnlocking = false;
    private float unlockTimer = 0f;

    [Header("Dolly Push-In")]
    public AnimationCurve dollyCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float dollyDuration = 0.4f;
    public float dollyStrength = 0.5f;
    private bool playDolly = false;
    private float dollyTimer = 0f;

    // Internal yaw/pitch values
    private float yaw;
    private float pitch;

    private void Awake()
    {
        // Disable cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (followTarget == null || input == null) return;

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, followTarget.position, Time.deltaTime * followSpeed);

        // Detect lock-on changes
        bool isLockedOn = lockOnSystem != null && lockOnSystem.currentTarget != null;

        if (isLockedOn && !isCinematicLockingOn)
        {
            isCinematicLockingOn = true;
            isCinematicUnlocking = false;
            cinematicTimer = 0f;
            playDolly = true;
            dollyTimer = 0f;
        }
        else if (!isLockedOn && isCinematicLockingOn)
        {
            isCinematicLockingOn = false;
            isCinematicUnlocking = true;
            unlockTimer = 0f;
        }

        // Handle rotation
        if (isLockedOn)
            HandleLockOnTracking();
        else
            HandleFreeLook();

        // Apply yaw & pitch
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // Calculate camera position
        Vector3 targetOffset = cameraPivot.right * lateralOffset - cameraPivot.forward * defaultDistance;
        Vector3 desiredCameraPos = cameraPivot.position + targetOffset;

        // Collision
        Vector3 blockedPosition = HandleCameraCollision(cameraPivot.position, desiredCameraPos);
        cameraTransform.position = blockedPosition;

        // Dolly zoom effect
        if (playDolly)
        {
            dollyTimer += Time.deltaTime;
            float t = dollyTimer / dollyDuration;
            float dollyAmount = dollyCurve.Evaluate(t);
            cameraTransform.position += cameraTransform.forward * dollyAmount * dollyStrength;

            if (t >= 1f)
                playDolly = false;
        }

        // Look at pivot point
        cameraTransform.LookAt(cameraPivot.position);

        // FOV zoom
        float targetFOV = isLockedOn ? lockOnFOV : normalFOV;
        mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);

        // Unlock sweep handling
        if (!isLockedOn && isCinematicUnlocking)
        {
            float targetYaw = followTarget.eulerAngles.y;
            if (unlockTimer < unlockSweepDuration)
            {
                float t = unlockTimer / unlockSweepDuration;
                float sweepOffset = Mathf.Lerp(unlockYawOffset, 0f, t);
                yaw = Mathf.LerpAngle(yaw, targetYaw + sweepOffset, Time.deltaTime * lockBlendSpeed * 2f);
                unlockTimer += Time.deltaTime;
            }
            else
            {
                isCinematicUnlocking = false;
            }
        }
    }

    private void HandleFreeLook()
    {
        Vector2 inputLook = input.LookInput;
        yaw += inputLook.x * yawSpeed * Time.deltaTime;
        pitch -= inputLook.y * pitchSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private void HandleLockOnTracking()
    {
        if (lockOnSystem.currentTarget == null) return;

        Vector3 playerHead = followTarget.position + Vector3.up * 1.5f;
        Vector3 targetHead = lockOnSystem.currentTarget.position + Vector3.up * 1.5f;
        Vector3 midpoint = (playerHead + targetHead) / 2f;

        Vector3 direction = midpoint - cameraPivot.position;
        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        float targetYaw = lookRotation.eulerAngles.y;
        float targetPitch = 10f; // consistent combat pitch

        // Wrap yaw to avoid snapping
        if (targetYaw - yaw > 180f) targetYaw -= 360f;
        else if (yaw - targetYaw > 180f) targetYaw += 360f;

        if (isCinematicLockingOn && cinematicTimer < cinematicSweepDuration)
        {
            float t = cinematicTimer / cinematicSweepDuration;
            float sweepOffset = Mathf.Lerp(cinematicYawOffset, 0f, t);
            yaw = Mathf.LerpAngle(yaw, targetYaw + sweepOffset, Time.deltaTime * lockBlendSpeed * 2f);
            cinematicTimer += Time.deltaTime;
        }
        else
        {
            yaw = Mathf.LerpAngle(yaw, targetYaw, Time.deltaTime * lockBlendSpeed);
        }

        pitch = Mathf.LerpAngle(pitch, targetPitch, Time.deltaTime * lockBlendSpeed);
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private Vector3 HandleCameraCollision(Vector3 from, Vector3 to)
    {
        Vector3 dir = to - from;
        float distance = dir.magnitude;

        if (Physics.SphereCast(from, sphereRadius, dir.normalized, out RaycastHit hit, distance, collisionMask))
        {
            float adjustedDistance = Mathf.Clamp(hit.distance - 0.05f, minDistance, distance);
            return from + dir.normalized * adjustedDistance;
        }

        return to;
    }
  
}
