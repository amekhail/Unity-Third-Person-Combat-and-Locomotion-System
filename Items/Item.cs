using UnityEngine;

public class Item : ScriptableObject
{
        [Header("Item Information")]
        public string itemName;
        [TextArea]public string itemDescription; 
        public Sprite itemIcon;
        public int itemID;

}