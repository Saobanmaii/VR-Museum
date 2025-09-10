using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class KitchenItem : MonoBehaviour
{
    public CCuChi.Kitchen.KitchenItemData data;
    public UnityEvent OnPicked;
    public UnityEvent OnDropped;

    public CCuChi.Kitchen.KitchenItemType Type => data != null ? data.type : default;

    public void InvokePicked() => OnPicked?.Invoke();
    public void InvokeDropped() => OnDropped?.Invoke();
}