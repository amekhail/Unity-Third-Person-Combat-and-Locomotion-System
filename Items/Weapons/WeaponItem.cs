using UnityEngine;

public class WeaponItem : Item
{
    // Animator Controller Override
    [Header("Weapon Model")] 
    public GameObject weaponModel;
    
    [Header("Weapon Base Damage")]
    public int physicalDamage = 0;
    // TODO: Heavy attack modifer, light attack modifer, sprint attack modifer etc.

    [Header("Weapon Base Poise Damage")] 
    public float basePoiseDamage = 10;
    
    // Item Based Actions (left click, right click, Shift+Left click, Shift+Right click)
    
}
