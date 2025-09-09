using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IconState : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    [SerializeField] Image icon;
    [SerializeField] Sprite defaultSprite;  // ảnh 1
    [SerializeField] Sprite hoverSprite;    // ảnh 2
    [SerializeField] Sprite clickedSprite;  // ảnh 3
    [SerializeField] Button resetButton;    // nút X để reset

    IconStateGroup group;
    bool isClicked;

    void Awake(){
        group = GetComponentInParent<IconStateGroup>(); // lấy group ở cha

        if (resetButton){
            resetButton.onClick.AddListener(InitDefault); // gán sự kiện cho nút X
        }
    }

    public void InitDefault(){
        isClicked = false;
        if (icon) icon.sprite = defaultSprite;
    }

    public void SetClicked(bool clicked){
        isClicked = clicked;
        if (icon) icon.sprite = clicked ? clickedSprite : defaultSprite;
    }

    public void OnPointerEnter(PointerEventData _){
        if (!isClicked && icon) icon.sprite = hoverSprite;
    }
    public void OnPointerExit(PointerEventData _){
        if (!isClicked && icon) icon.sprite = defaultSprite;
    }
    public void OnPointerClick(PointerEventData _){
        if (group) group.Select(this);
    }
}