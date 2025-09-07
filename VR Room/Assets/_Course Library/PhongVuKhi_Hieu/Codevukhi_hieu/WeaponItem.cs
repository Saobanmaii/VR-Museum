using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class WeaponItem : MonoBehaviour
{
    public CCuChi.Weapons.WeaponData data;
    public UnityEvent OnPicked;
    public UnityEvent OnDropped;

    public CCuChi.Weapons.WeaponType Type => data != null ? data.type : default;

    public void InvokePicked() => OnPicked?.Invoke();
    public void InvokeDropped() => OnDropped?.Invoke();
}