using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapRoomManager : MonoBehaviour
{
    public List<MapPartSocketValidator> sockets;
    public UnityEvent OnWrongPlacement;
    public UnityEvent<int, int> OnProgressChanged;
    public UnityEvent OnAllCompleted;

    int correctCount;

    void OnEnable()
    {
        foreach (var s in sockets)
        {
            s.OnPlacedCorrect.AddListener(HandleCorrect);
            s.OnPlacedWrong.AddListener(HandleWrong);
            s.OnItemRemoved.AddListener(HandleRemoved);
        }
        Recount();
    }

    void OnDisable()
    {
        foreach (var s in sockets)
        {
            s.OnPlacedCorrect.RemoveListener(HandleCorrect);
            s.OnPlacedWrong.RemoveListener(HandleWrong);
            s.OnItemRemoved.RemoveListener(HandleRemoved);
        }
    }

    void HandleCorrect(MapPartSocketValidator s)
    {
        Recount();
        if (correctCount >= sockets.Count)
            OnAllCompleted?.Invoke();
    }

    void HandleWrong(MapPartSocketValidator s)
    {
        OnWrongPlacement?.Invoke();
    }

    void HandleRemoved(MapPartSocketValidator s)
    {
        Recount();
    }

    void Recount()
    {
        int c = 0;
        foreach (var s in sockets) if (s.IsCorrectlyOccupied()) c++;
        correctCount = c;
        OnProgressChanged?.Invoke(correctCount, sockets.Count);
    }
}