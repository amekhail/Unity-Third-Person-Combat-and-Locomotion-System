using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LockOnSystem : MonoBehaviour
{
   [Header("Lock On Settings")]
   public float lockOnRange = 15f;
   public LayerMask targetMask;
   public Transform targetIndicator;
   
   [Header("Debug")]
   public Transform currentTarget;
   
   private List<Transform> _availableTargets = new List<Transform>();
   private int _currentTargetIndex = -1;

   public List<Transform> validTargets = new List<Transform>();
   
   private void FindTargets()
   {
      _availableTargets.Clear();
      Collider[] hits = Physics.OverlapSphere(transform.position, lockOnRange, targetMask);
      Debug.Log($"[LockOn] Found {hits.Length} potential targets.");

      Vector3 origin = transform.position + Vector3.up * 1.7f;

      foreach (Collider hit in hits)
      {
         Transform potential = hit.transform;
         Vector3 targetPoint = potential.position + Vector3.up * 1f;
         Vector3 dir = targetPoint - origin;
         float dist = dir.magnitude;

         Debug.DrawLine(origin, targetPoint, Color.yellow, 0.1f);

         // Better comparison and use targetMask
         if (Physics.Raycast(origin, dir.normalized, out RaycastHit hitInfo, dist, targetMask))
         {
            if (hitInfo.transform == potential)
            {
               Debug.Log($"[LockOn] {potential.name} is visible and added.");
               _availableTargets.Add(potential);
            }
         }
      }

      _availableTargets.Sort((a, b) =>
         Vector3.Distance(transform.position, a.position)
            .CompareTo(Vector3.Distance(transform.position, b.position))
      );
   } 
   
   public void ToggleLockOn()
   {
      if (currentTarget != null)
      {
         UnlockTarget();
         return;
      }

      FindTargets();

      if (_availableTargets.Count > 0)
      {
         _currentTargetIndex = 0;
         currentTarget = _availableTargets[_currentTargetIndex];
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
      Debug.Log($"[LockOn] Switched to target: {currentTarget.name}");

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

   private void Update()
   {
      if (currentTarget != null)
      {
         UpdateLockOnTargets();

         if (targetIndicator != null)
         {
            targetIndicator.position = currentTarget.transform.position + Vector3.up * 2f;
         }
      }
   }

   private void UpdateLockOnTargets()
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

         if (Physics.Raycast(origin, dir.normalized, out RaycastHit hitInfo, dist, targetMask))
         {
            if (hitInfo.transform == potential)
            {
               validTargets.Add(potential);
            }
         }
      }

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
         Gizmos.DrawWireCube(currentTarget.transform.position, Vector3.one);
      }
   }

}
