using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class MedicalItem : MonoBehaviour
{
    public CCuChi.Medical.MedicalToolData data;
    public UnityEvent OnPicked;
    public UnityEvent OnDropped;

    public CCuChi.Medical.MedicalToolType Type => data != null ? data.type : default;

    public void InvokePicked() => OnPicked?.Invoke();
    public void InvokeDropped() => OnDropped?.Invoke();
}
