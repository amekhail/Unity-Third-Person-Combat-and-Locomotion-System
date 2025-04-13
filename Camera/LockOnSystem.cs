using System.Collections.Generic;
using UnityEngine;

public class LockOnSystem : MonoBehaviour
{
    [Header("Lock On Settings")]
    public float lockOnRange = 15f;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public Transform targetIndicator;
    public float checkInterval = 0.25f;

    [Header("Debug")]
    public Transform currentTarget;

    private int _currentTargetIndex = -1;

    private float _checkTimer = 0f;

    public List<Transform> validTargets { get; private set; } = new List<Transform>();

    private void Update()
    {
        if (currentTarget != null)
        {
            _checkTimer += Time.deltaTime;

            if (_checkTimer >= checkInterval)
            {
                _checkTimer = 0f;
                if (!IsTargetValid(currentTarget))
                {
                    Debug.Log($"[LockOn] Auto-unlocked from {currentTarget.name}");
                    UnlockTarget();
                    return;
                }
            }

            RefreshValidTargets();

            if (targetIndicator != null)
            {
                targetIndicator.position = currentTarget.position + Vector3.up * 2f;
            }
        }
    }
    
    private bool IsTargetValid(Transform target)
    {
        if (!target) return false;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > lockOnRange) return false;

        Vector3 origin = transform.position + Vector3.up * 1.7f;
        Vector3 targetPoint = target.position + Vector3.up * 1f;
        Vector3 dir = targetPoint - origin;
        float dist = dir.magnitude;

        // LOS check
        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hitInfo, dist, obstacleMask))
        {
            if (hitInfo.transform.root != target.root)
            {
                return false;
            }
        }

        return true;
    }

    public void ToggleLockOn()
    {
        if (currentTarget != null)
        {
            UnlockTarget();
            return;
        }

        RefreshValidTargets();

        if (validTargets.Count > 0)
        {
            _currentTargetIndex = 0;
            currentTarget = validTargets[_currentTargetIndex];
            if (targetIndicator != null)
            {
                targetIndicator.gameObject.SetActive(true);
            }
        }
    }

    public void CycleTarget(bool right)
    {
        if (validTargets.Count == 0 || currentTarget == null) return;

        int currentIndex = validTargets.IndexOf(currentTarget);
        if (currentIndex == -1) currentIndex = 0;

        int nextIndex = right
            ? (currentIndex + 1) % validTargets.Count
            : (currentIndex - 1 + validTargets.Count) % validTargets.Count;

        currentTarget = validTargets[nextIndex];

        if (targetIndicator != null)
        {
            targetIndicator.gameObject.SetActive(true);
        }
    }

    public void UnlockTarget()
    {
        currentTarget = null;
        if (targetIndicator != null)
        {
            targetIndicator.gameObject.SetActive(false);
        }
    }

    private void RefreshValidTargets()
    {
        validTargets.Clear();
        Collider[] hits = Physics.OverlapSphere(transform.position, lockOnRange, targetMask);

        Vector3 origin = transform.position + Vector3.up * 1.7f;

        foreach (Collider hit in hits)
        {
            Transform potential = hit.transform;
            Vector3 targetPoint = potential.position + Vector3.up * 1f;
            Vector3 dir = targetPoint - origin;
            float dist = dir.magnitude;

            Debug.DrawLine(origin, targetPoint, Color.red, 0.1f);

            // Check if obstacle blocks view
            if (!Physics.Raycast(origin, dir.normalized, dist, obstacleMask))
            {
                // Check if target is directly visible
                if (Physics.Raycast(origin, dir.normalized, out RaycastHit hitInfo, dist, targetMask))
                {
                    if (hitInfo.transform.root == potential.root)
                    {
                        validTargets.Add(potential);
                    }
                }
            }
        }

        // Sort by angle to prioritize in-front targets
        validTargets.Sort((a, b) =>
        {
            Vector3 dirA = (a.position - transform.position).normalized;
            Vector3 dirB = (b.position - transform.position).normalized;
            float angleA = Vector3.Angle(transform.forward, dirA);
            float angleB = Vector3.Angle(transform.forward, dirB);
            return angleA.CompareTo(angleB);
        });

        Debug.Log($"[LockOn] Valid targets after LOS: {validTargets.Count}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lockOnRange);

        if (currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(currentTarget.position, Vector3.one);
        }
    }
}
