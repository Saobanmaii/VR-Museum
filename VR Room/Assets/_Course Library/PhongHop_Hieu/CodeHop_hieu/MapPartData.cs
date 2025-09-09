using UnityEngine;

namespace CCuChi.Map
{
    public enum MapPartType
    {
        Part1,
        Part2,
        Part3,
        Part4,
        Part5,
        Part6,
        Part7,
        Part8,
        Part9,
        Part10
    }

    [CreateAssetMenu(menuName = "CCuChi/MapPartData", fileName = "MapPartData_")]
    public class MapPartData : ScriptableObject
    {
        public MapPartType type;
        public string displayNameVN;
        [TextArea] public string description;
        public Sprite icon;
    }
}