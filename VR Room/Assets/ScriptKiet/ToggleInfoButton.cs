using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[DisallowMultipleComponent]
public class ToggleInfoButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    IPointerClickHandler
{
    [Header("Icon / States")]
    [SerializeField] Image icon;                 // Ảnh hiển thị 3 frame
    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite hoverSprite;
    [SerializeField] Sprite pressedSprite;

    [Header("Target Info UI (CanvasGroup)")]
    [SerializeField] CanvasGroup infoPanel;      // Panel cần toggle
    [SerializeField] bool startHidden = true;

    [Header("Anim")]
    [SerializeField] float fadeDuration = 0.25f;
    [SerializeField] float scaleInFrom = 0.95f;  // zoom-in nhẹ khi hiện
    [SerializeField] float scaleOutTo = 0.95f;   // zoom-out nhẹ khi ẩn
    [SerializeField] Ease easeIn = Ease.OutQuad;
    [SerializeField] Ease easeOut = Ease.InQuad;

    Tween panelTween;
    Tween iconPulseTween;
    bool panelVisible;

    void Reset()
    {
        icon = GetComponent<Image>();
    }

    void Awake()
    {
        if (!icon) icon = GetComponent<Image>();
        SetIcon(normalSprite);

        // Chuẩn hóa trạng thái panel ban đầu
        if (infoPanel)
        {
            if (startHidden)
                SetPanelInvisible();
            else
                SetPanelVisibleInstant();
        }
    }

    // ===== Toggle by click =====
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!infoPanel) return;
        if (panelVisible) HidePanel();
        else ShowPanel();
    }

    // ===== Visual states =====
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (icon && !panelVisible) SetIcon(hoverSprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (icon && !panelVisible) SetIcon(normalSprite);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (icon) SetIcon(pressedSprite);
        // nhấn có thể thêm pulse nhẹ nếu thích
        iconPulseTween?.Kill();
        if (icon)
        {
            icon.transform.DOKill();
            iconPulseTween = icon.transform.DOScale(0.97f, 0.08f).SetEase(Ease.OutQuad);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (icon)
        {
            iconPulseTween?.Kill();
            icon.transform.DOKill();
            icon.transform.DOScale(1f, 0.08f).SetEase(Ease.OutQuad);
        }
        // nếu panel chưa mở thì trả về hover/normal tương ứng
        if (!panelVisible)
            SetIcon(hoverSprite); // nếu con trỏ vẫn ở trên sẽ thấy hover; rời ra sẽ về normal nhờ OnPointerExit
    }

    // ===== Panel anim logic =====
    void ShowPanel()
    {
        panelVisible = true;
        SetIcon(pressedSprite);

        panelTween?.Kill();
        infoPanel.gameObject.SetActive(true);
        infoPanel.interactable = false; // bật sau khi anim xong
        infoPanel.blocksRaycasts = false;
        infoPanel.DOKill();

        // chuẩn bị trạng thái bắt đầu
        infoPanel.alpha = 0f;
        infoPanel.transform.localScale = Vector3.one * scaleInFrom;

        panelTween = DOTween.Sequence()
            .Join(infoPanel.DOFade(1f, fadeDuration).SetEase(easeIn))
            .Join(infoPanel.transform.DOScale(1f, fadeDuration).SetEase(easeIn))
            .OnComplete(() =>
            {
                infoPanel.interactable = true;
                infoPanel.blocksRaycasts = true;
                // khi đã mở, icon giữ pressed hoặc bạn đổi sang hover tùy ý:
                SetIcon(pressedSprite);
            });
    }

    void HidePanel()
    {
        panelVisible = false;
        // khi ẩn, icon sẽ chuyển về hover/normal theo vị trí con trỏ (OnPointerExit sẽ xử lý)
        panelTween?.Kill();
        infoPanel.interactable = false;
        infoPanel.blocksRaycasts = false;
        infoPanel.DOKill();

        panelTween = DOTween.Sequence()
            .Join(infoPanel.DOFade(0f, fadeDuration).SetEase(easeOut))
            .Join(infoPanel.transform.DOScale(scaleOutTo, fadeDuration).SetEase(easeOut))
            .OnComplete(() =>
            {
                infoPanel.gameObject.SetActive(false);
                infoPanel.transform.localScale = Vector3.one; // reset
                // sau khi ẩn, icon về normal (nếu con trỏ còn ở trên, OnPointerEnter sẽ đổi sang hover)
                SetIcon(normalSprite);
            });
    }

    // ===== Helpers =====
    void SetIcon(Sprite s)
    {
        if (icon && s) icon.sprite = s;
    }

    void SetPanelInvisible()
    {
        panelVisible = false;
        infoPanel.DOKill();
        infoPanel.alpha = 0f;
        infoPanel.transform.localScale = Vector3.one;
        infoPanel.interactable = false;
        infoPanel.blocksRaycasts = false;
        infoPanel.gameObject.SetActive(false);
    }

    void SetPanelVisibleInstant()
    {
        panelVisible = true;
        infoPanel.DOKill();
        infoPanel.gameObject.SetActive(true);
        infoPanel.alpha = 1f;
        infoPanel.transform.localScale = Vector3.one;
        infoPanel.interactable = true;
        infoPanel.blocksRaycasts = true;
        SetIcon(pressedSprite);
    }
}