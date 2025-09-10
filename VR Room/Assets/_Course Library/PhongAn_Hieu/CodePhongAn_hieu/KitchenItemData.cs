using UnityEngine;

namespace CCuChi.Kitchen
{
    public enum KitchenItemType
    {
        Bowl,     // Bát
        Cassava   // Sắn
    }

    [CreateAssetMenu(menuName = "CCuChi/KitchenItemData", fileName = "KitchenItemData_")]
    public class KitchenItemData : ScriptableObject
    {
        public KitchenItemType type;
        public string displayNameVN;
        [TextArea] public string description;
        public Sprite icon;
    }
}