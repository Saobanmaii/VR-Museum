using UnityEngine;
using System.Collections.Generic;

public class IconStateGroup : MonoBehaviour {
    [SerializeField] List<IconState> items = new(); // hoặc để trống, tự tìm

    void Awake(){
        if (items.Count == 0) items.AddRange(GetComponentsInChildren<IconState>(true));
        foreach (var it in items) it.InitDefault();
    }

    public void Select(IconState selected){
        foreach (var it in items) it.SetClicked(it == selected);
    }
}