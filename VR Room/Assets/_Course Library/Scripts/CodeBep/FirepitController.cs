using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class FirepitController : MonoBehaviour
{
    [SerializeField] FirepitAreaCounter counter;

    [Header("Particles")]
    [SerializeField] ParticleSystem flameSmall, flameOptimal, flameStrong;

    [Header("Audio")]
    [SerializeField] AudioSource fireLoop;

    public enum FireState { Off, Small, Optimal, Strong }
    public FireState State { get; private set; } = FireState.Off;

    [Header("Events")]
    public UnityEvent<FireState> OnStateChanged;
    public UnityEvent OnWarningStarted; 
    public UnityEvent OnWarningStopped;

    void Awake() { if (counter) counter.OnCountChanged.AddListener(Apply); Apply(counter ? counter.Count : 0); }

    public void Apply(int wood)
    {
        var next = FireState.Off;
        if (wood == 0) next = FireState.Off;
        else if (wood < 5) next = FireState.Small;       
        else if (wood == 5) next = FireState.Optimal;    
        else next = FireState.Strong;                    

        if (next == State) return;
        State = next; OnStateChanged?.Invoke(State);

        // FX g?i ý
        StopAllFX();
        switch (State)
        {
            case FireState.Small: Play(flameSmall); SetAudio(.3f); break;
            case FireState.Optimal: Play(flameOptimal); SetAudio(.6f); break;
            case FireState.Strong: Play(flameStrong); SetAudio(.9f); break;
            default: SetAudio(0f); break;
        }

        // Warning UI
        if (State == FireState.Strong) OnWarningStarted?.Invoke();
        else OnWarningStopped?.Invoke();
    }

    void StopAllFX() { Stop(flameSmall); Stop(flameOptimal); Stop(flameStrong); }

    static void Play(ParticleSystem ps) { if (!ps) return; ps.gameObject.SetActive(true); if (!ps.isPlaying) ps.Play(); }
    static void Stop(ParticleSystem ps) { if (!ps) return; if (ps.isPlaying) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); }

    void SetAudio(float vol)
    {
        if (!fireLoop) return;
        if (vol <= 0f) { if (fireLoop.isPlaying) fireLoop.Stop(); return; }
        if (!fireLoop.isPlaying) fireLoop.Play();
        fireLoop.volume = vol;
    }
}