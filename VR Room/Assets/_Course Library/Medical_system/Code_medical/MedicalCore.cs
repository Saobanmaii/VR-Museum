using UnityEngine;

namespace CCuChi.Medical
{
    public enum MedicalToolType
    {
        Keo,
        Dao,
        Gap,
        BongGac,
        LoCon
    }

    [CreateAssetMenu(menuName = "CCuChi/MedicalToolData", fileName = "MedicalToolData_")]
    public class MedicalToolData : ScriptableObject
    {
        public MedicalToolType type;
        public string displayNameVN;
        [TextArea] public string description;
        public Sprite icon;
    }
}
