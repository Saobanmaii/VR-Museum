using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GlobalButtonClickSfx : MonoBehaviour
{
    [Header("Âm thanh click")]
    [SerializeField] AudioClip clickSfx;
    [Range(0f, 1f)] public float volume = 1f;

    AudioSource _source;

    void Awake()
    {
        // AudioSource 2D cho UI
        _source = GetComponent<AudioSource>();
        if (_source == null) _source = gameObject.AddComponent<AudioSource>();
        _source.playOnAwake = false;
        _source.loop = false;
        _source.spatialBlend = 0f;

        
        HookAllButtonsInScene();
        SceneManager.sceneLoaded += (_, __) => HookAllButtonsInScene();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= (_, __) => HookAllButtonsInScene();
    }

    void HookAllButtonsInScene()
    {
        var buttons = GameObject.FindObjectsOfType<Button>(true); // include inactive
        foreach (var btn in buttons)
        {
            // Xoá listener cũ của chính script này (tránh add trùng)
            btn.onClick.RemoveListener(Play);
            btn.onClick.AddListener(Play);
        }
    }

    void Play()
    {
        if (clickSfx != null)
            _source.PlayOneShot(clickSfx, volume);
    }
}