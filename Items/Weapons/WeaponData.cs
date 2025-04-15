using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Combat/Weapon Data")]
public class WeaponData :ScriptableObject
{
    [FormerlySerializedAs("WeaponType")] public WeaponType weaponType;
    public WeaponAnimationSet animationSet;

    [Header("Weapon Info")] 
    public string weaponName;
    public string weaponDescription;

    [Header("Stats")] 
    public float baseDamage;
    [Tooltip("Time between attacks or animation speed multiplyer")]
    public float attackSpeed;
    
    [Header("Effects")]
    public bool canStagger = true;

    [Header("Visuals")] 
    public GameObject weaponPrefab;

}


public enum WeaponType
{
    Sword,
    Axe,
    GreatSword,
    GreatAxe
}