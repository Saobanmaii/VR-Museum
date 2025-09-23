using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSocketInteractor))]
public class MedicalSocketValidator : MonoBehaviour
{
    public CCuChi.Medical.MedicalToolType expectedType;
    public UnityEvent<MedicalSocketValidator> OnPlacedCorrect;
    public UnityEvent<MedicalSocketValidator> OnPlacedWrong;
    public UnityEvent<MedicalSocketValidator> OnItemRemoved;

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
        socket.selectEntered.RemoveListener(HandleSelectEntered);
        socket.selectExited.RemoveListener(HandleSelectExited);
    }

    void HandleSelectEntered(SelectEnterEventArgs args)
    {
        current = args.interactableObject;
        var go = args.interactableObject.transform.gameObject;
        var item = go.GetComponent<MedicalItem>();

        if (item != null && item.Type == expectedType)
        {
            OnPlacedCorrect?.Invoke(this);
        }
        else
        {
            OnPlacedWrong?.Invoke(this);
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
        if (!socket.hasSelection) return false;
        var go = socket.firstInteractableSelected?.transform.gameObject;
        var item = go != null ? go.GetComponent<MedicalItem>() : null;
        return item != null && item.Type == expectedType;
    }
}
