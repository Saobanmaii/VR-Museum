using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSocketInteractor))]
public class WeaponSocketValidator : MonoBehaviour
{
    public CCuChi.Weapons.WeaponType expectedType;
    public UnityEvent<WeaponSocketValidator> OnPlacedCorrect;
    public UnityEvent<WeaponSocketValidator> OnPlacedWrong;
    public UnityEvent<WeaponSocketValidator> OnItemRemoved;

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
        var item = go.GetComponent<WeaponItem>();

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
        var item = go != null ? go.GetComponent<WeaponItem>() : null;
        return item != null && item.Type == expectedType;
    }
}