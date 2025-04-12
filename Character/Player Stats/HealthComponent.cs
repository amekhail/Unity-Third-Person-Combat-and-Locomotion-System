using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
   public int maxHealth = 100;
   private int _currentHealth;

   private void Awake() => _currentHealth = maxHealth;

   public void ApplyDamage(int amount)
   {
      _currentHealth -= amount;
      if (_currentHealth <= 0)
      {
         _currentHealth = 0;
         Die();
      }
      
   }

   private void Die()
   {
      // TODO: Disable input handler and play death animation
   }
   
}
