using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[DisallowMultipleComponent, RequireComponent(typeof(Collider))]
public class PotReceiver : MonoBehaviour
{
    [SerializeField] string acceptTag = "Cassava";
    [SerializeField] AudioSource sfxDrop;
    public UnityEvent<int> OnCountChanged;
    public UnityEvent OnFirstReceived;

    readonly HashSet<Rigidbody> _inPot = new();
    bool _first;

    void Reset() { GetComponent<Collider>().isTrigger = true; }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(acceptTag)) return;
        var rb = other.attachedRigidbody; if (!rb) return;
        if (!_inPot.Add(rb)) return;

        if (sfxDrop) sfxDrop.Play();
        OnCountChanged?.Invoke(_inPot.Count);
        if (!_first) { _first = true; OnFirstReceived?.Invoke(); }
    }
    public int Count => _inPot.Count;
    public bool HasMin(int n) => _inPot.Count >= n;
}