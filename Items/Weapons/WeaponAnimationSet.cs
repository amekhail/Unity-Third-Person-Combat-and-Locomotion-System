using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Animation Set")]
public class WeaponAnimationSet : ScriptableObject
{
    public string WeaponType; 
    
    [Header("Light Attacks")]
    public string[] lightAttackAnimations;
    [Header("Heavy Attacks")]
    public string[] heavyAttackAnimations;
    [Header("Special Attacks")]
    public string[] specialAttackAnimations;
}
