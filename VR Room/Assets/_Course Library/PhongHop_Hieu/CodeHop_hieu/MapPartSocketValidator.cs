using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSocketInteractor))]
public class MapPartSocketValidator : MonoBehaviour
{
    public CCuChi.Map.MapPartType expectedType;
    public UnityEvent<MapPartSocketValidator> OnPlacedCorrect;
    public UnityEvent<MapPartSocketValidator> OnPlacedWrong;
    public UnityEvent<MapPartSocketValidator> OnItemRemoved;

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
        var go = args.interactableObject.transform.gameObject;
        var item = go.GetComponent<MapPartItem>();

        if (item != null && item.Type == expectedType)
        {
            OnPlacedCorrect?.Invoke(this);
        }
        else
        {
            OnPlacedWrong?.Invoke(this);
            if (wrongClip)
            {
                if (audioSource) audioSource.PlayOneShot(wrongClip, wrongVolume);
                else AudioSource.PlayClipAtPoint(wrongClip, transform.position, wrongVolume);
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
        if (socket == null) return false;

        if (!socket.hasSelection) return false;

        var go = socket.firstInteractableSelected?.transform?.gameObject;
        var item = go ? go.GetComponent<MapPartItem>() : null;
        return item != null && item.Type == expectedType;
    }
}