using UnityEngine;
using DG.Tweening;

public class WelcomeUIController : MonoBehaviour
{
    [Header("WELCOME UI (UI #1)")]
    [SerializeField] private CanvasGroup welcomeCG;
    [SerializeField] private RectTransform welcomePanel; // panel của UI #1
    [SerializeField] private AudioSource voiceSource;
    [SerializeField] private AudioClip welcomeClip;

    [Header("UI #2 (Bật khi bấm 'Tìm hiểu')")]
    [SerializeField] private GameObject mainUI;          // kéo UI2 vào đây
    [SerializeField] private bool disableWelcomeOnProceed = true; // tắt hẳn UI1 sau khi bấm

    [Header("Anim IN (DOTween)")]
    [SerializeField] private float fadeTime  = 0.9f;
    [SerializeField] private float scaleFrom = 0.85f;
    [SerializeField] private Vector3 moveOffsetIn = new Vector3(0f, -0.5f, 0f);
    [SerializeField] private Ease easeIn = Ease.OutBack;

    [Header("Anim OUT (DOTween)")]
    [SerializeField] private float fadeOutTime = 0.5f;
    [SerializeField] private Vector3 moveOffsetOut = new Vector3(0f, 0.35f, 0f);
    [SerializeField] private Ease easeOut = Ease.InCubic;
    [SerializeField] private float voiceFadeOut = 0.25f;

    [Header("Floating (optional)")]
    [SerializeField] private bool enableFloating = true;
    [SerializeField] private float floatAmplitude = 0.05f; // 5 cm
    [SerializeField] private float floatPeriod    = 2.0f;  // giây

    private Vector3 baseLocalPos;
    private Tween floatTween;
    private bool hasShown;
    private bool isClosing;

    private void Awake()
    {
        if (!welcomeCG)      welcomeCG = GetComponent<CanvasGroup>();
        if (!welcomePanel)   welcomePanel = GetComponentInChildren<RectTransform>(true);

        baseLocalPos = welcomePanel ? welcomePanel.localPosition : Vector3.zero;

        HideImmediate_UI1();

        if (mainUI) mainUI.SetActive(false);
    }

    private void OnDisable()
    {
        KillTweens();
    }

    private void KillTweens()
    {
        welcomeCG?.DOKill();
        welcomePanel?.DOKill();
        floatTween?.Kill();
        floatTween = null;
    }

    private void HideImmediate_UI1()
    {
        KillTweens();

        if (welcomeCG)
        {
            welcomeCG.alpha = 0f;
            welcomeCG.blocksRaycasts = false;
            welcomeCG.interactable   = false;
        }

        if (welcomePanel)
        {
            welcomePanel.localScale    = Vector3.one * scaleFrom;
            welcomePanel.localPosition = baseLocalPos + moveOffsetIn;
        }
    }

    /// <summary>
    /// Gọi khi Player bước vào zone.
    /// </summary>
    public void ShowWelcome()
    {
        if (hasShown) return; // một lần cho mỗi session
        hasShown = true;

        KillTweens();
        if (!welcomeCG || !welcomePanel)
        {
            Debug.LogWarning("[WelcomeUI] Chưa gán CanvasGroup/Panel!");
            return;
        }

        welcomeCG.alpha = 0f;
        welcomeCG.blocksRaycasts = true;
        welcomeCG.interactable   = true;
        welcomePanel.localScale    = Vector3.one * scaleFrom;
        welcomePanel.localPosition = baseLocalPos + moveOffsetIn;

        // IN sequence
        Sequence seq = DOTween.Sequence();
        seq.Join(welcomeCG.DOFade(1f, fadeTime));
        seq.Join(welcomePanel.DOScale(1f, fadeTime).SetEase(easeIn));
        seq.Join(welcomePanel.DOLocalMove(baseLocalPos, fadeTime).SetEase(Ease.OutCubic));
        seq.OnComplete(() =>
        {
            if (enableFloating) StartFloating();
        });

        // Voice
        if (voiceSource && welcomeClip)
        {
            voiceSource.volume = 1f;
            voiceSource.Stop();
            voiceSource.clip = welcomeClip;
            voiceSource.PlayDelayed(0.1f);
        }
    }

    private void StartFloating()
    {
        if (!welcomePanel) return;
        floatTween = welcomePanel
            .DOLocalMoveY(baseLocalPos.y + floatAmplitude, floatPeriod)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// Gắn vào Button "Tìm hiểu".
    /// Làm anim OUT rồi mới bật UI #2.
    /// </summary>
    public void OnClick_TimHieu()
    {
        HideWithAnimThenSwitchUI2();
    }

    private void HideWithAnimThenSwitchUI2()
    {
        if (isClosing) return; // chặn spam click
        isClosing = true;

        KillTweens();

        // Fade out voice
        if (voiceSource && voiceSource.isPlaying)
        {
            voiceSource.DOFade(0f, voiceFadeOut)
                       .OnComplete(() => { voiceSource.Stop(); voiceSource.volume = 1f; });
        }

        if (!welcomeCG || !welcomePanel)
        {
            // Fallback: tắt ngay nếu thiếu reference
            HideImmediate_UI1();
            if (mainUI) mainUI.SetActive(true);
            if (disableWelcomeOnProceed) gameObject.SetActive(false);
            return;
        }

        // Tắt tương tác ngay khi out
        welcomeCG.blocksRaycasts = false;
        welcomeCG.interactable   = false;

        // OUT sequence
        Sequence seq = DOTween.Sequence();
        seq.Join(welcomeCG.DOFade(0f, fadeOutTime));
        seq.Join(welcomePanel.DOScale(scaleFrom, fadeOutTime).SetEase(easeOut));
        seq.Join(welcomePanel.DOLocalMove(baseLocalPos + moveOffsetOut, fadeOutTime).SetEase(easeOut));
        seq.OnComplete(() =>
        {
            if (mainUI) mainUI.SetActive(true);
            if (disableWelcomeOnProceed) gameObject.SetActive(false);
            isClosing = false;
        });
    }
}