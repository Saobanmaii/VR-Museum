using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class TabUIController : MonoBehaviour
{
    [Header("Nav Buttons")]
    [SerializeField] Button btnInfo;
    [SerializeField] Button btnGuide;
    [SerializeField] Button btnQuest;

    [Header("Close (X) Buttons")]
    [SerializeField] Button btnCloseInfo;
    [SerializeField] Button btnCloseGuide;
    [SerializeField] Button btnCloseQuest;

    [Header("NavBar (needs CanvasGroup)")]
    [SerializeField] RectTransform navBar;
    [SerializeField] CanvasGroup navBarCg;

    [Header("Intro (needs CanvasGroup)")]
    [SerializeField] RectTransform introRT;
    [SerializeField] CanvasGroup introCg;

    [Header("Pages (each needs CanvasGroup)")]
    [SerializeField] RectTransform pageInfoRT; [SerializeField] CanvasGroup pageInfoCg;
    [SerializeField] RectTransform pageGuideRT; [SerializeField] CanvasGroup pageGuideCg;
    [SerializeField] RectTransform pageQuestRT; [SerializeField] CanvasGroup pageQuestCg;

    [Header("Optional: Nav Highlights")]
    [SerializeField] GameObject hlInfo, hlGuide, hlQuest;

    [Header("Animation")]
    [SerializeField] float slideTime = 0.35f;
    [SerializeField] float fadeTime = 0.20f;
    [SerializeField] float overshoot = 20f;
    [SerializeField] float gap = 40f;
    [SerializeField] Ease easeIn = Ease.OutCubic;
    [SerializeField] Ease easeOut = Ease.InCubic;

    [Header("Voice-over (play once)")]
    [SerializeField] AudioSource voice;
    [SerializeField] AudioClip clipIntro;
    [SerializeField] AudioClip clipInfo;
    [SerializeField] AudioClip clipGuide;
    [SerializeField] AudioClip clipQuest;

    enum Tab { None, Info, Guide, Quest }
    Tab _current = Tab.None;
    bool _firstClickHandled = false;
    RectTransform _curRT; CanvasGroup _curCg;
    readonly HashSet<Tab> _voiced = new();

    void Awake()
    {
        // open tab buttons
        if (btnInfo) btnInfo.onClick.AddListener(() => OnNavClicked(Tab.Info));
        if (btnGuide) btnGuide.onClick.AddListener(() => OnNavClicked(Tab.Guide));
        if (btnQuest) btnQuest.onClick.AddListener(() => OnNavClicked(Tab.Quest));

        // close (X) buttons
        if (btnCloseInfo)  btnCloseInfo.onClick.AddListener(() => OnCloseClicked(Tab.Info));
        if (btnCloseGuide) btnCloseGuide.onClick.AddListener(() => OnCloseClicked(Tab.Guide));
        if (btnCloseQuest) btnCloseQuest.onClick.AddListener(() => OnCloseClicked(Tab.Quest));

        PrepareInitialLayout();
        PlayInitialIntro();
    }

    // ---------- Initial layout & intro ----------
    void PrepareInitialLayout()
    {
        KillAll();

        navBar.anchoredPosition += new Vector2(-overshoot, 0);
        navBarCg.alpha = 0f;

        MoveOffscreenRight(introRT);
        SetVisible(introCg, false, instant: true);

        SetupPageHidden(pageInfoRT, pageInfoCg);
        SetupPageHidden(pageGuideRT, pageGuideCg);
        SetupPageHidden(pageQuestRT, pageQuestCg);

        SetHighlight(null);
    }

    void PlayInitialIntro()
    {
        navBar.DOAnchorPos(navBar.anchoredPosition + new Vector2(overshoot, 0), slideTime).SetEase(easeIn);
        navBarCg.DOFade(1f, fadeTime);

        SlideInFromRight(introRT);
        FadeInteract(introCg, true);

        _current = Tab.None;
        _curRT = introRT; _curCg = introCg;
        PlayVoice(Tab.None);
    }

    // ---------- Nav flow ----------
    void OnNavClicked(Tab target)
    {
        if (!_firstClickHandled)
        {
            _firstClickHandled = true;

            SlideOutToLeft(introRT);
            FadeInteract(introCg, false);

            ShowTab(target, fromRight: true);
            return;
        }

        if (_current == target) return;
        SwitchTab(_current, target);
    }

    void SwitchTab(Tab from, Tab to)
    {
        var (fromRT, fromCg) = GetRTCG(from);
        if (fromRT) SlideOutToLeft(fromRT);
        if (fromCg) FadeInteract(fromCg, false);

        ShowTab(to, fromRight: true);
    }

    void ShowTab(Tab t, bool fromRight)
    {
        var (rt, cg) = GetRTCG(t);
        if (!rt || !cg) return;

        if (fromRight) MoveOffscreenRight(rt);
        SlideInFromRight(rt);
        FadeInteract(cg, true);

        _current = t; _curRT = rt; _curCg = cg;
        SetHighlight(t);
        PlayVoice(t);
    }

    // ---------- Close (X) ----------
    void OnCloseClicked(Tab t)
    {
        // chỉ đóng nếu tab đang mở đúng với nút X
        if (_current != t) return;

        var (rt, cg) = GetRTCG(t);
        if (rt) SlideOutToLeft(rt);
        if (cg) FadeInteract(cg, false);

        // trở lại Intro
        MoveOffscreenRight(introRT);
        SlideInFromRight(introRT);
        FadeInteract(introCg, true);

        _current = Tab.None;
        _curRT = introRT; _curCg = introCg;
        SetHighlight(null);
        PlayVoice(Tab.None);
    }

    // ---------- Voice-over (play once) ----------
    void PlayVoice(Tab t)
    {
        if (_voiced.Contains(t)) return;

        AudioClip clip = t switch
        {
            Tab.None => clipIntro,
            Tab.Info => clipInfo,
            Tab.Guide => clipGuide,
            Tab.Quest => clipQuest,
            _ => null
        };
        if (!voice || !clip) { _voiced.Add(t); return; }

        voice.DOKill();
        voice.DOFade(0f, 0.1f).OnComplete(() =>
        {
            voice.volume = 1f;
            voice.Stop();
            voice.clip = clip;
            voice.Play();
        });

        _voiced.Add(t);
    }

    // ---------- Helpers ----------
    (RectTransform, CanvasGroup) GetRTCG(Tab t)
    {
        return t switch
        {
            Tab.Info => (pageInfoRT, pageInfoCg),
            Tab.Guide => (pageGuideRT, pageGuideCg),
            Tab.Quest => (pageQuestRT, pageQuestCg),
            _ => (introRT, introCg),
        };
    }

    void SetupPageHidden(RectTransform rt, CanvasGroup cg)
    {
        MoveOffscreenRight(rt);
        SetVisible(cg, false, instant: true);
    }

    void SetVisible(CanvasGroup cg, bool show, bool instant = false)
    {
        if (!cg) return;
        cg.alpha = show ? 1f : 0f;
        cg.interactable = show;
        cg.blocksRaycasts = show;
    }

    void MoveOffscreenRight(RectTransform rt)
    {
        var w = rt.rect.width;
        rt.anchoredPosition = new Vector2(w + gap, 0);
    }

    void SlideInFromRight(RectTransform rt)
    {
        rt.DOKill();
        rt.DOAnchorPos(Vector2.zero, slideTime).SetEase(easeIn).SetUpdate(true);
    }

    void SlideOutToLeft(RectTransform rt)
    {
        rt.DOKill();
        var w = rt.rect.width;
        rt.DOAnchorPos(new Vector2(-(w + gap), 0), slideTime).SetEase(easeOut).SetUpdate(true);
    }

    void SetHighlight(Tab? t)
    {
        if (hlInfo)  hlInfo.SetActive(t == Tab.Info);
        if (hlGuide) hlGuide.SetActive(t == Tab.Guide);
        if (hlQuest) hlQuest.SetActive(t == Tab.Quest);
    }

    void KillAll()
    {
        navBar.DOKill(); introRT.DOKill(); pageInfoRT.DOKill(); pageGuideRT.DOKill(); pageQuestRT.DOKill();
        navBarCg.DOKill(); introCg.DOKill(); pageInfoCg.DOKill(); pageGuideCg.DOKill(); pageQuestCg.DOKill();
        if (voice) voice.DOKill();
    }

    void FadeInteract(CanvasGroup cg, bool enable)
    {
        if (!cg) return;
        cg.DOKill();
        cg.DOFade(enable ? 1f : 0f, fadeTime).SetUpdate(true);
        cg.interactable = enable;
        cg.blocksRaycasts = enable;
    }
}
