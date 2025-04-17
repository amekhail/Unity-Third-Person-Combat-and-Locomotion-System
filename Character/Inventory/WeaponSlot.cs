using UnityEngine;

public class WeaponSlot : MonoBehaviour
{
    public GameObject currentWeaponModel;
    public SlotType slotType;

    public void UnloadWeaponModel()
    {
        if (currentWeaponModel)
        {
            Destroy(currentWeaponModel);
        }
    }

    public void LoadWeaponModel(GameObject weaponModel)
    {
        currentWeaponModel = weaponModel;
        weaponModel.transform.parent = transform;
        
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localRotation = Quaternion.identity;
    }
    
}

/// <summary>
/// A simple slot enum to determine which slot the weapon will go on
/// </summary>
public enum SlotType
{
    RightHand,  // Primary weapon slot
    LeftHand,   // Possibly for a shield. I doubt that I want to add dual wielding weapons into this system
    BackSlot,   // For shield or for large weapons? Bows?
    HipSlot     // For swords and other weapons
}
