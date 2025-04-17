using System;
using UnityEngine;

public class PlayerEquiptmentManager : MonoBehaviour
{
    public CharacterContext context;
    public WeaponSlot rightHandSlot; 
    // TODO: Add the rest

    public GameObject rightHandWeaponModel;

    private void Awake()
    {
        context = GetComponent<CharacterContext>();
        InitializeWeaponSlots();
    }

    private void Start()
    {
        LoadAllWeapons();
    }

    private void InitializeWeaponSlots()
    {
        // currently redundant, will update as I get more slots
    }

    public void LoadAllWeapons()
    {
        LoadRightWeapon();
    }

    public void LoadRightWeapon()
    {
        if (context.playerInventoryManager.weaponItem)
        {
            rightHandWeaponModel = Instantiate(context.playerInventoryManager.weaponItem.weaponModel);
            rightHandSlot.LoadWeaponModel(rightHandWeaponModel);
        }
    }
    
    
        
        
}
