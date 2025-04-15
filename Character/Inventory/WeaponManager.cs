using UnityEngine;

public class WeaponManager : MonoBehaviour
{
        [Header("References")] 
        public Transform weaponSocket;
        public CombatController combatController;

        [Header("Equiped Weapon")] 
        public WeaponData equipedWeapon;
        private GameObject _currentWeaponObject;

        public void EquipWeapon(WeaponData newWeapon)
        {
                if (!newWeapon) return;
                
                // Destroy reference to previous weapon
                if (_currentWeaponObject)
                {
                        Destroy(_currentWeaponObject);
                }
                
                // Save new weapon reference
                equipedWeapon = newWeapon;

                if (newWeapon.weaponPrefab && weaponSocket)
                {
                        _currentWeaponObject = Instantiate(newWeapon.weaponPrefab, weaponSocket);
                }

                if (combatController)
                {
                        combatController.currentWeaponAnimations = newWeapon.animationSet;
                }
                
                Debug.Log($"Equipped weapon: {newWeapon.weaponType}");
        }
}
