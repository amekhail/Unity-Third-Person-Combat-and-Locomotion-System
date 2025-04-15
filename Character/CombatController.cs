using System;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [Header("CombatSettings")] 
    public float combatExitTime = 5f;
    public float combatRadius = 10f;
    public LayerMask enemyLayer;

    public WeaponAnimationSet currentWeaponAnimations;
    private int _currentComboIndex = 0;

    public CharacterContext Context;


    private float _lastCombatActionTime;
    private bool _isInCombat;
    
    public bool IsInCombat => _isInCombat;

    private void Awake()
    {
        Context = GetComponent<CharacterContext>();
    }

    private void Update()
    {
        if (_isInCombat && Time.time - _lastCombatActionTime > combatExitTime)
        {
            ExitCombat();
        }
    }
    public void TryAttack()
    {
        if (currentWeaponAnimations == null) return;

        string animToPlay = currentWeaponAnimations.lightAttackAnimations[_currentComboIndex];
        Context.animator.SetTrigger(animToPlay);
        
        _currentComboIndex = (_currentComboIndex + 1) % currentWeaponAnimations.lightAttackAnimations.Length;
        _lastCombatActionTime = Time.time;

        if (!_isInCombat)
            EnterCombat();
    }

    public void EnterCombat()
    {
        _isInCombat = true;
        Debug.Log("Entered combat.");
    }

    public void ExitCombat()
    {
        _isInCombat = false;
        Debug.Log("Exited combat.");
    }

    public bool AreEnemiesNearby()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, combatRadius, enemyLayer);
        return hits.Length > 0;
    }

}
