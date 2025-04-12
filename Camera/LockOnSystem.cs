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

   private void FindTargets()
   {
      _availableTargets.Clear();
      Collider[] hits = Physics.OverlapSphere(transform.position, lockOnRange, targetMask);

      foreach (Collider hit in hits)
      {
         _availableTargets.Add(hit.transform);
      }
      
      _availableTargets.Sort((a,b) => 
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
      if (_availableTargets.Count <= 1) return;
      
      _currentTargetIndex += right ? 1 : -1;

      if (_currentTargetIndex >= _availableTargets.Count)
      {
         _currentTargetIndex = 0;
      } else if (_currentTargetIndex < 0)
      {
         _currentTargetIndex = _availableTargets.Count - 1;
      }
      
      currentTarget = _availableTargets[_currentTargetIndex];
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
      if (currentTarget != null && targetIndicator != null)
      {
         targetIndicator.position = currentTarget.transform.position + Vector3.up * 2f;
      }
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
