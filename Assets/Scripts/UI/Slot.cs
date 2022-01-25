using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private Vector3 originPos;

    public Item item; // 획득한 아이템
    public int itemCount; // 획득한 아이템 개수
    public Image itemImage; // 아이템 이미지

    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private GameObject go_CountImage;

    private WeaponManager theWeaponManager;
    
    void Start() {
        originPos = transform.position;
        theWeaponManager = FindObjectOfType<WeaponManager>();
    }

    // 이미지 투명도 조절
    private void SetColor(float _alpha){
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 슬롯 초기화
    private void ClearSlot(){
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        go_CountImage.SetActive(false);
        text_Count.text = "0";
    }

    // 아이템 개수 조정
    public void SetSlotCount(int _count){
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if(itemCount <= 0){
            ClearSlot();
        }
    }

    // 아이템 획득
    public void AddItem(Item _item, int _count = 1){
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        if(item.itemType != Item.ItemType.Equipment){
            text_Count.text = itemCount.ToString();
            go_CountImage.SetActive(true);
        }
        else {
            text_Count.text = "0";
            go_CountImage.SetActive(false);
        }

        SetColor(1);
    }

    public void OnPointerClick(PointerEventData eventData){
        if(eventData.button == PointerEventData.InputButton.Right){
            if(item != null){
                if(item.itemType == Item.ItemType.Equipment){ // 장착
                    StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(item.weaponType, item.itemName));
                }
                else{ // 소모
                    Debug.Log(item.itemName + " 을 사용했습니다.");
                    SetSlotCount(-1);
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData){
        if(item != null){
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);

            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData){
        if(item != null){
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData){
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragSlot = null;
    }

    private void ChangeSlot(){
        Item _tempItem = item;
        int _tempItemCount = itemCount;

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

        if(_tempItem != null){
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
        }
        else{
            DragSlot.instance.dragSlot.ClearSlot();
        }
    }

    public void OnDrop(PointerEventData eventData){
        if(DragSlot.instance.dragSlot != null){
            ChangeSlot();
        }
    }
}