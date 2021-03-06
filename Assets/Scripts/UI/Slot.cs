using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private Vector3 originPos;

    public Item item; // 획득한 아이템
    public int itemCount; // 획득한 아이템 개수
    public Image itemImage; // 아이템 이미지

    [SerializeField]
    private bool isQuickSlot; // 퀵슬롯 여부 판단
    [SerializeField]
    private int quickSlotNumber; // 퀵슬롯 번호

    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private GameObject go_CountImage;

    private ItemEffectDatabase theItemEffectDatabase;
    [SerializeField]
    private RectTransform baseRect; // 인벤토리 영역
    [SerializeField]
    private RectTransform quickSlotBaseRect; // 퀵슬롯의 영역
    private InputNumber theInputNumber;
    
    void Start() {
        theItemEffectDatabase = FindObjectOfType<ItemEffectDatabase>();
        theInputNumber = FindObjectOfType<InputNumber>();
        originPos = transform.position;
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

    public int GetQuickSlotNumber(){
        return quickSlotNumber;
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

    public void OnPointerEnter(PointerEventData eventData){
        if(item != null){
            theItemEffectDatabase.ShowToolTip(item, transform.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData){
        theItemEffectDatabase.HideToolTip();
    }

    public void OnPointerClick(PointerEventData eventData){
        if(eventData.button == PointerEventData.InputButton.Right){
            if(item != null){
                theItemEffectDatabase.UseItem(item);
                if(item.itemType == Item.ItemType.Used){
                    SetSlotCount(-1);
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData){
        if(item != null && Inventory.inventoryActivated){
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
        if(!(
            (DragSlot.instance.transform.localPosition.x > baseRect.rect.xMin && DragSlot.instance.transform.localPosition.x < baseRect.rect.xMax
            && DragSlot.instance.transform.localPosition.y > baseRect.rect.yMin && DragSlot.instance.transform.localPosition.y < baseRect.rect.yMax)
            ||
            (DragSlot.instance.transform.localPosition.x > quickSlotBaseRect.rect.xMin && DragSlot.instance.transform.localPosition.x < quickSlotBaseRect.rect.xMax
            && DragSlot.instance.transform.localPosition.y < /*quickSlotBaseRect.transform.localPosition.y - quickSlotBaseRect.rect.yMax*/-440
            && DragSlot.instance.transform.localPosition.y > /*quickSlotBaseRect.transform.localPosition.y - quickSlotBaseRect.rect.yMin*/-500)
            )
            ){
                if(DragSlot.instance.dragSlot != null){
                    Debug.Log(DragSlot.instance.transform.localPosition.y);
                    Debug.Log(quickSlotBaseRect.transform.localPosition.y - quickSlotBaseRect.rect.yMax);
                    Debug.Log(quickSlotBaseRect.transform.localPosition.y - quickSlotBaseRect.rect.yMin);


                    theInputNumber.Call();
                }
        }
        else{
            DragSlot.instance.SetColor(0);
            DragSlot.instance.dragSlot = null;
        }
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

            if(isQuickSlot){
                theItemEffectDatabase.IsActivatedQuickSlot(quickSlotNumber); // 활성화된 퀵슬롯인지 여부 판단. 맞으면 교체
            }
            else{
                if(DragSlot.instance.dragSlot.isQuickSlot){ // 출발이 퀵슬롯인 경우
                    theItemEffectDatabase.IsActivatedQuickSlot(DragSlot.instance.dragSlot.quickSlotNumber); // 활성화된 퀵슬롯인지 여부 판단.
                }
            }
        }
    }
}
