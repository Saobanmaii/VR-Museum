using UnityEngine;

[DisallowMultipleComponent]
public class BoilFXController : MonoBehaviour
{
    [SerializeField] FirepitController firepit;
    [SerializeField] ParticleSystem boilBubblesFX, steamFX;
    [SerializeField] AudioSource boilLoop;

    public bool IsBoiling { get; private set; }

    void OnEnable() { Apply(false); if (firepit) firepit.OnStateChanged.AddListener(OnFire); }
    void OnDisable() { if (firepit) firepit.OnStateChanged.RemoveListener(OnFire); }

    void OnFire(FirepitController.FireState s)
    {
        bool should = (s == FirepitController.FireState.Optimal || s == FirepitController.FireState.Strong);
        if (should == IsBoiling) return;
        IsBoiling = should; Apply(IsBoiling);
    }

    void Apply(bool on)
    {
        if (on) { Play(boilBubblesFX); Play(steamFX); if (boilLoop && !boilLoop.isPlaying) boilLoop.Play(); }
        else { Stop(boilBubblesFX); Stop(steamFX); if (boilLoop && boilLoop.isPlaying) boilLoop.Stop(); }
    }
    static void Play(ParticleSystem ps) { if (ps) { ps.gameObject.SetActive(true); if (!ps.isPlaying) ps.Play(); } }
    static void Stop(ParticleSystem ps) { if (ps && ps.isPlaying) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); }
}