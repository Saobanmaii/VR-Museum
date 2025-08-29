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

    [Header("Anim Settings (DOTween)")]
    [SerializeField] private float fadeTime  = 0.9f;
    [SerializeField] private float scaleFrom = 0.85f;
    [SerializeField] private Vector3 moveOffset = new Vector3(0f, -0.5f, 0f);
    [SerializeField] private Ease easeIn = Ease.OutBack;

    [Header("Floating (optional)")]
    [SerializeField] private bool enableFloating = true;
    [SerializeField] private float floatAmplitude = 0.05f; // 5 cm
    [SerializeField] private float floatPeriod    = 2.0f;  // giây

    private Vector3 baseLocalPos;
    private Tween floatTween;
    private bool hasShown;

    private void Awake()
    {
        if (!welcomeCG)      welcomeCG = GetComponent<CanvasGroup>();
        if (!welcomePanel)   welcomePanel = GetComponentInChildren<RectTransform>(true);

        // Lưu vị trí gốc của panel
        baseLocalPos = welcomePanel ? welcomePanel.localPosition : Vector3.zero;

        // Ẩn UI1 lúc đầu
        HideImmediate_UI1();

        // Đảm bảo UI2 tắt lúc đầu (nếu có gán)
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
            welcomePanel.localPosition = baseLocalPos + moveOffset;
        }
    }

    /// <summary>
    /// Gọi hàm này khi Player bước vào zone (ZoneTrigger).
    /// Hiện UI1 bằng DOTween + phát voice chào mừng.
    /// </summary>
    public void ShowWelcome()
    {
        if (hasShown) return; // một lần cho mỗi session (đơn giản)
        hasShown = true;

        KillTweens();
        if (!welcomeCG || !welcomePanel)
        {
            Debug.LogWarning("[WelcomeUI] Chưa gán CanvasGroup/Panel!");
            return;
        }

        // Bật tương tác và reset trạng thái anim
        welcomeCG.alpha = 0f;
        welcomeCG.blocksRaycasts = true;
        welcomeCG.interactable   = true;
        welcomePanel.localScale    = Vector3.one * scaleFrom;
        welcomePanel.localPosition = baseLocalPos + moveOffset;

        // Sequence xuất hiện
        Sequence seq = DOTween.Sequence();
        seq.Join(welcomeCG.DOFade(1f, fadeTime));
        seq.Join(welcomePanel.DOScale(1f, fadeTime).SetEase(easeIn));
        seq.Join(welcomePanel.DOLocalMove(baseLocalPos, fadeTime).SetEase(Ease.OutCubic));
        seq.OnComplete(() =>
        {
            if (enableFloating) StartFloating();
        });

        // Phát voice
        if (voiceSource && welcomeClip)
        {
            voiceSource.Stop();
            voiceSource.clip = welcomeClip;
            voiceSource.PlayDelayed(0.1f);
        }
    }

    private void StartFloating()
    {
        if (!welcomePanel) return;
        floatTween = welcomePanel.DOLocalMoveY(baseLocalPos.y + floatAmplitude, floatPeriod)
                                   .SetEase(Ease.InOutSine)
                                   .SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// Gắn hàm này vào Button "Tìm hiểu" (OnClick).
    /// Ẩn UI1 ngay và bật UI2 (SetActive(true)), không làm anim cho UI2 ở đây.
    /// </summary>
    public void OnClick_TimHieu()
    {
        // Tắt voice chào mừng nếu còn phát
        if (voiceSource && voiceSource.isPlaying) voiceSource.Stop();

        // Ẩn UI1
        HideImmediate_UI1();
        if (disableWelcomeOnProceed) gameObject.SetActive(false);

        // Bật UI2
        if (mainUI) mainUI.SetActive(true);
        else Debug.LogWarning("[WelcomeUI] Chưa gán Main UI (UI #2)!");
    }
}