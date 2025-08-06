using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ActiveXemtt : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject xemTT;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPos;

    void Start()
    {
        rectTransform = xemTT.GetComponent<RectTransform>();
        canvasGroup = xemTT.GetComponent<CanvasGroup>();
        originalPos = rectTransform.anchoredPosition;
        xemTT.SetActive(false);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "LeftHand Controller" )
        {
            xemTT.SetActive(true);
            rectTransform.anchoredPosition = originalPos - new Vector2(0, 200f);
            canvasGroup.alpha = 0;
            rectTransform.DOAnchorPos(originalPos, 0.6f).SetEase(Ease.OutBack);
            canvasGroup.DOFade(1, 0.6f);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "LeftHand Controller")
    {
        rectTransform.DOAnchorPos(originalPos - new Vector2(0, 200f), 0.6f).SetEase(Ease.InSine);
        canvasGroup.DOFade(0, 0.6f).OnComplete(() =>
        {
            xemTT.SetActive(false);
        });
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    }
}
