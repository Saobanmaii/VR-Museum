using UnityEngine;

namespace CCuChi.Weapons
{
    public enum WeaponType
    {
        Mine,
        RPG2_B40,
        AK47,
        K50M,
        Bomb
    }

    [CreateAssetMenu(menuName = "CCuChi/WeaponData", fileName = "WeaponData_")]
    public class WeaponData : ScriptableObject
    {
        public WeaponType type;
        public string displayNameVN;
        [TextArea] public string description;
        public Sprite icon;
    }
}