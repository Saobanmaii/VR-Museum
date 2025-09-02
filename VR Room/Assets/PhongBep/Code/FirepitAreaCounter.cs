using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[DisallowMultipleComponent, RequireComponent(typeof(Collider))]
public class FirepitAreaCounter : MonoBehaviour
{
    [SerializeField] string firewoodTag = "Firewood";
    public UnityEvent<int> OnCountChanged;

    readonly HashSet<Rigidbody> _inside = new();
    void Reset() { GetComponent<Collider>().isTrigger = true; }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(firewoodTag)) return;
        var rb = other.attachedRigidbody; if (!rb) return;
        if (_inside.Add(rb)) OnCountChanged?.Invoke(_inside.Count);
    }
    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(firewoodTag)) return;
        var rb = other.attachedRigidbody; if (!rb) return;
        if (_inside.Remove(rb)) OnCountChanged?.Invoke(_inside.Count);
    }
    public int Count => _inside.Count;
}