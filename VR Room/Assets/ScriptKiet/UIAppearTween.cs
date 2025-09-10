using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class UIAppearTween : MonoBehaviour
{
    [Header("IN Animation (match WelcomeUIController style)")]
    [SerializeField] float moveDistance = 150f;          // trồi lên từ dưới (px)
    [SerializeField] float duration = 0.6f;              // thời gian vào
    [SerializeField] float scaleFrom = 1.0f;             // 1.0 nếu không muốn scale
    [SerializeField] Ease easeMove = Ease.OutCubic;      // mượt, không nảy
    [SerializeField] Ease easeFade = Ease.OutQuad;

    [Header("Behavior")]
    [SerializeField] bool autoPlayOnEnable = true;       // tự chạy khi SetActive(true)

    RectTransform rect;
    CanvasGroup cg;
    Vector3 baseLocalPos;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        cg   = GetComponent<CanvasGroup>();

        baseLocalPos = rect.localPosition;
        HideImmediate();
    }

    void OnEnable()
    {
        if (autoPlayOnEnable)
            PlayIn();
    }

    void OnDisable()
    {
        KillTweens();
    }

    void KillTweens()
    {
        rect.DOKill();
        cg.DOKill();
    }

    void HideImmediate()
    {
        KillTweens();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable   = false;

        rect.localScale    = Vector3.one * scaleFrom;
        rect.localPosition = baseLocalPos + new Vector3(0f, -moveDistance, 0f);
    }

    /// <summary>
    /// Chạy anim vào: fade-in + trồi từ dưới lên (không overshoot).
    /// </summary>
    public void PlayIn()
    {
        KillTweens();

        // Reset trạng thái ban đầu
        cg.alpha = 0f;
        cg.blocksRaycasts = true;
        cg.interactable   = true;

        rect.localScale    = Vector3.one * scaleFrom;
        rect.localPosition = baseLocalPos + new Vector3(0f, -moveDistance, 0f);

        // Sequence IN
        var seq = DOTween.Sequence();
        seq.Join(rect.DOLocalMove(baseLocalPos, duration).SetEase(easeMove));
        seq.Join(cg.DOFade(1f, duration).SetEase(easeFade));
        // Nếu muốn thêm gì sau khi xuất hiện, thêm .OnComplete(...) tại đây
    }
}
