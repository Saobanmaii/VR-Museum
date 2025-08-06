using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ActiveAndInActive : MonoBehaviour
{
    public GameObject panelDetail;               // GameObject chứa UI panel
    public float duration = 0.4f;                // Thời gian tween
    public float offsetY = 200f;                 // Trượt lên/xuống theo anchored Y

    private RectTransform rectTransform;         // RectTransform của panel
    private CanvasGroup canvasGroup;             // CanvasGroup để fade
    private Vector2 originalPos;                 // Vị trí gốc
    private bool isVisible = false;              // Trạng thái toggle

    public AudioClip soundArt;
    private AudioSource audioSource;
    void Start()
    {
        // Lấy component từ GameObject
        rectTransform = panelDetail.GetComponent<RectTransform>();
        canvasGroup = panelDetail.GetComponent<CanvasGroup>();
        originalPos = rectTransform.anchoredPosition;
        panelDetail.SetActive(false);
    }

    public void ActiveandInActive()
    {
        isVisible = !isVisible;

        if (isVisible)
        {
            panelDetail.SetActive(true);

            // Đặt vị trí ban đầu trượt xuống trước khi trồi lên
            rectTransform.anchoredPosition = originalPos - new Vector2(0, offsetY);
            canvasGroup.alpha = 0;

            // Trồi lên + hiện dần
            rectTransform.DOAnchorPos(originalPos, duration).SetEase(Ease.OutBack, 0.8f);
            canvasGroup.DOFade(1, duration);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            // Trượt xuống + mờ dần
            rectTransform.DOAnchorPos(originalPos - new Vector2(0, offsetY), duration).SetEase(Ease.InSine);
            canvasGroup.DOFade(0, duration).OnComplete(() => {
                panelDetail.SetActive(false);
            });

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
