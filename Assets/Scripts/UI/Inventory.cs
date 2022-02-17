using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static bool inventoryActivated = false;

    [SerializeField]
    private GameObject go_InventoryBase;
    [SerializeField]
    private GameObject go_SlotsParent;
    [SerializeField]
    private GameObject go_QuickSlotParent;
    [SerializeField]
    private QuickSlotController theQuickSlot;

    private Slot[] slots; // 인벤토리 슬롯들
    private Slot[] quickslots; // 퀵슬롯들
    private bool isNotPut;
    private int slotNumber;

    public Slot[] GetSlots(){
        return slots;
    }

    [SerializeField]
    private Item[] items;

    public void LoadToInven(int _arrayNum, string _itemName, int _itemNum){
        for(int i=0; i<items.Length; i++){
            if(items[i].itemName == _itemName){
                slots[_arrayNum].AddItem(items[i], _itemNum);
            }
        }
    }

    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
        quickslots = go_QuickSlotParent.GetComponentsInChildren<Slot>();
    }

    private void OpenInventory(){
        GameManager.isOpenInventory = true;
        go_InventoryBase.SetActive(true);
    }

    private void CloseInventory(){
        GameManager.isOpenInventory = false;
        go_InventoryBase.SetActive(false);
    }

    public void AcquireItem(Item _item, int _count = 1){
        PutSlot(quickslots, _item, _count);
        if(!isNotPut){
            theQuickSlot.IsActivatedQuickSlot(slotNumber);
        }

        if(isNotPut){
            PutSlot(slots, _item, _count);
        }

        if(isNotPut){
            Debug.Log("퀵슬롯과 인벤토리가 꽉 찼습니다."); // 나중에 UI 상으로 보이게 수정하면 될듯.
        }
    }

    private void PutSlot(Slot[] _slots, Item _item, int _count){
        if(Item.ItemType.Equipment != _item.itemType){
            for(int i=0; i<_slots.Length; i++){
                if(_slots[i].item != null){
                    if(_slots[i].item.itemName == _item.itemName){
                        slotNumber = i;
                        _slots[i].SetSlotCount(_count);
                        isNotPut = false;
                        return;
                    }
                }
            }
        }

        for(int i=0; i<_slots.Length; i++){
            if(_slots[i].item == null){
                _slots[i].AddItem(_item, _count);
                isNotPut = false;
                return;
            }
        }

        isNotPut = true;
    }

    private void TryOpenInventory(){
        if(Input.GetKeyDown(KeyCode.I)){
            inventoryActivated = !inventoryActivated;

            if(inventoryActivated){
                OpenInventory();
            }
            else{
                CloseInventory();
            }
        }
    }

    void Update() {
        TryOpenInventory();
    }
}
