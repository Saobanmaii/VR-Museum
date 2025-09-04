using UnityEngine;
using DG.Tweening;

[DisallowMultipleComponent]
public class TriggerUI_DOTweenTarget : MonoBehaviour
{
    [Header("Who can trigger")]
    [SerializeField] string playerTag = "Player";

    [Header("Target UI")]
    [SerializeField] GameObject target;         // Kéo UI muốn animate vào đây
    [SerializeField] bool startHidden = true;   // Ẩn khi bắt đầu

    CanvasGroup cg;
    RectTransform rt;

    [Header("Fade")]
    [SerializeField] bool useFade = true;
    [SerializeField] float fadeDuration = 0.35f;

    [Header("Slide (optional)")]
    [SerializeField] bool useSlide = true;
    [SerializeField] Vector2 slideFrom = new Vector2(0, -80);  // UI bay lên từ dưới
    [SerializeField] float slideDuration = 0.45f;
    [SerializeField] Ease slideEase = Ease.OutCubic;

    [Header("Scale (optional)")]
    [SerializeField] bool useScale = false;
    [SerializeField] Vector3 scaleFrom = new Vector3(0.9f, 0.9f, 1f);
    [SerializeField] float scaleDuration = 0.35f;
    [SerializeField] Ease scaleEase = Ease.OutBack;

    Sequence currentSeq;
    Vector2 anchoredStart;   // vị trí ban đầu
    Vector3 scaleStart;      // scale ban đầu

    void Awake()
    {
        if (!target)
        {
            Debug.LogWarning($"[{name}] TriggerUI_DOTweenTarget: Chưa gán Target UI.");
            enabled = false;
            return;
        }

        rt = target.GetComponent<RectTransform>();
        cg = target.GetComponent<CanvasGroup>();
        if (!cg) cg = target.AddComponent<CanvasGroup>();

        if (rt) anchoredStart = rt.anchoredPosition;
        scaleStart = target.transform.localScale;

        // Set trạng thái ban đầu
        if (startHidden) SetImmediateHidden();
        else SetImmediateShown();
    }

    void OnDestroy()
    {
        currentSeq?.Kill();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
            PlayShow();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
            PlayHide();
    }

    // ---- Public API nếu muốn gọi tay từ code khác ----
    public void PlayShow()  => Play(true);
    public void PlayHide()  => Play(false);

    // ---------------- Core ----------------
    void Play(bool show)
    {
        currentSeq?.Kill();
        currentSeq = DOTween.Sequence();

        // chuẩn bị trạng thái "from"
        if (show)
        {
            if (useFade)  { cg.alpha = 0f; }
            if (useSlide && rt) { rt.anchoredPosition = anchoredStart + slideFrom; }
            if (useScale) { target.transform.localScale = Vector3.Scale(scaleStart, scaleFrom); }

            // bật raycast khi đã hiện
            cg.interactable = false;
            cg.blocksRaycasts = false;

            // add tweens
            if (useSlide && rt) currentSeq.Join(rt.DOAnchorPos(anchoredStart,  SlideDur()).SetEase(slideEase));
            if (useScale)        currentSeq.Join(target.transform.DOScale(scaleStart, ScaleDur()).SetEase(scaleEase));
            if (useFade)         currentSeq.Join(cg.DOFade(1f, FadeDur()));

            currentSeq.OnComplete(() => {
                cg.interactable = true;
                cg.blocksRaycasts = true;
            });
        }
        else
        {
            // tắt raycast ngay khi bắt đầu ẩn (tránh click)
            cg.interactable = false;
            cg.blocksRaycasts = false;

            if (useSlide && rt) currentSeq.Join(rt.DOAnchorPos(anchoredStart + slideFrom, SlideDur()).SetEase(slideEase));
            if (useScale)        currentSeq.Join(target.transform.DOScale(Vector3.Scale(scaleStart, scaleFrom), ScaleDur()).SetEase(scaleEase));
            if (useFade)         currentSeq.Join(cg.DOFade(0f, FadeDur()));
        }
    }

    // ---------------- Helpers ----------------
    float FadeDur()  => Mathf.Max(0f, fadeDuration);
    float SlideDur() => Mathf.Max(0f, slideDuration);
    float ScaleDur() => Mathf.Max(0f, scaleDuration);

    void SetImmediateHidden()
    {
        if (useFade) cg.alpha = 0f; else cg.alpha = 0f; // vẫn 0 để UI không bắt raycast
        if (useSlide && rt) rt.anchoredPosition = anchoredStart + slideFrom;
        if (useScale) target.transform.localScale = Vector3.Scale(scaleStart, scaleFrom);
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    void SetImmediateShown()
    {
        cg.alpha = 1f;
        if (rt) rt.anchoredPosition = anchoredStart;
        target.transform.localScale = scaleStart;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }
}