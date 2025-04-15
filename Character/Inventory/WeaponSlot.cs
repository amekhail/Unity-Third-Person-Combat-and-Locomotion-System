using UnityEngine;

public class WeaponSlot : MonoBehaviour
{ 
    public WeaponData equippedWeapon;

    public void EquipWeapon(WeaponData newWeapon)
    {
        equippedWeapon = newWeapon;

        if (newWeapon.weaponPrefab != null)
        {
            Instantiate(newWeapon.weaponPrefab, transform);
        }

        var combat = GetComponent<CombatController>();
        if (combat != null)
        {
            combat.currentWeaponAnimations = newWeapon.animationSet;
        }

    }
        
}
