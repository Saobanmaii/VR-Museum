using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSocketInteractor))]
public class KitchenSocketValidator : MonoBehaviour
{
    public CCuChi.Kitchen.KitchenItemType expectedType;
    public UnityEvent<KitchenSocketValidator> OnPlacedCorrect;
    public UnityEvent<KitchenSocketValidator> OnPlacedWrong;
    public UnityEvent<KitchenSocketValidator> OnItemRemoved;

    [Header("Wrong Placement Sound (optional)")]
    public AudioSource audioSource;
    public AudioClip wrongClip;
    [Range(0f, 1f)] public float wrongVolume = 1f;

    XRSocketInteractor socket;
    IXRSelectInteractable current;

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(HandleSelectEntered);
        socket.selectExited.AddListener(HandleSelectExited);
    }

    void OnDestroy()
    {
        if (socket != null)
        {
            socket.selectEntered.RemoveListener(HandleSelectEntered);
            socket.selectExited.RemoveListener(HandleSelectExited);
        }
    }

    void HandleSelectEntered(SelectEnterEventArgs args)
    {
        current = args.interactableObject;

        var tr = args.interactableObject != null ? args.interactableObject.transform : null;
        var go = tr != null ? tr.gameObject : null;
        var item = go != null ? go.GetComponent<KitchenItem>() : null;

        if (item != null && item.Type == expectedType)
        {
            OnPlacedCorrect?.Invoke(this);
        }
        else
        {
            OnPlacedWrong?.Invoke(this);
            if (wrongClip)
            {
                if (audioSource)
                    audioSource.PlayOneShot(wrongClip, wrongVolume);
                else
                    AudioSource.PlayClipAtPoint(wrongClip, transform.position, wrongVolume);
            }
        }
    }

    void HandleSelectExited(SelectExitEventArgs args)
    {
        if (current == args.interactableObject)
        {
            current = null;
            OnItemRemoved?.Invoke(this);
        }
    }

    public bool IsCorrectlyOccupied()
    {
        if (socket == null) socket = GetComponent<XRSocketInteractor>();
        if (socket == null) return false;                 // không có socket

        if (!socket.hasSelection) return false;

        var first = socket.firstInteractableSelected;
        if (first == null) return false;

        var tr = first.transform;
        if (tr == null) return false;

        var go = tr.gameObject;
        if (go == null) return false;

        var item = go.GetComponent<KitchenItem>();
        return item != null && item.Type == expectedType;
    }
}
