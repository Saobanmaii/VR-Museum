using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class MapPartItem : MonoBehaviour
{
    public CCuChi.Map.MapPartData data;
    public UnityEvent OnPicked;
    public UnityEvent OnDropped;

    public CCuChi.Map.MapPartType Type => data != null ? data.type : default;

    public void InvokePicked() => OnPicked?.Invoke();
    public void InvokeDropped() => OnDropped?.Invoke();
}