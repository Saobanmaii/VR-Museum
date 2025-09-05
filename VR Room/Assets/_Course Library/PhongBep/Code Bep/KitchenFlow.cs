using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class KitchenFlow : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] FirepitController firepit;
    [SerializeField] PotReceiver pot;
    [SerializeField] BoilFXController boilFX;

    [Header("Rules")]
    [SerializeField] int cassavaToComplete = 3;

    [Header("UI Events")]
    public UnityEvent ShowOverheatWarning;
    public UnityEvent HideOverheatWarning;
    public UnityEvent ShowCompleted;

    bool done;

    void OnEnable()
    {
        if (firepit)
        {
            firepit.OnWarningStarted.AddListener(OnWarningStartedHandler);
            firepit.OnWarningStopped.AddListener(OnWarningStoppedHandler);
            firepit.OnStateChanged.AddListener(OnFireStateChangedHandler);
        }

        if (pot)
            pot.OnCountChanged.AddListener(CheckCompletion);

        if (boilFX)
            CheckCompletion(0);
    }

    void OnDisable()
    {
        if (firepit)
        {
            firepit.OnWarningStarted.RemoveListener(OnWarningStartedHandler);
            firepit.OnWarningStopped.RemoveListener(OnWarningStoppedHandler);
            firepit.OnStateChanged.RemoveListener(OnFireStateChangedHandler);
        }

        if (pot)
            pot.OnCountChanged.RemoveListener(CheckCompletion);
    }

    void OnWarningStartedHandler() { ShowOverheatWarning?.Invoke(); }
    void OnWarningStoppedHandler() { HideOverheatWarning?.Invoke(); }
    void OnFireStateChangedHandler(FirepitController.FireState _) { CheckCompletion(0); }

    void CheckCompletion(int _)
    {
        if (done) return;

        bool boiling = boilFX && boilFX.IsBoiling;
        bool enoughCassava = pot && pot.HasMin(cassavaToComplete);

        if (boiling && enoughCassava)
        {
            done = true;
            HideOverheatWarning?.Invoke();
            ShowCompleted?.Invoke();
        }
    }

    public void ResetFlow()
    {
        done = false;
        CheckCompletion(0);
    }

#if UNITY_EDITOR
    void Reset()
    {
        if (!firepit) firepit = FindObjectOfType<FirepitController>();
        if (!pot) pot = FindObjectOfType<PotReceiver>();
        if (!boilFX) boilFX = FindObjectOfType<BoilFXController>();
    }
#endif
}