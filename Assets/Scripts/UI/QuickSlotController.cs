using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotController : MonoBehaviour
{
    [SerializeField]
    private Slot[] quickSlots; // 퀵슬롯들
    [SerializeField]
    private Transform tf_parent; // 퀵슬롯의 부모 객체

    private int selectedSlot; // 선택된 퀵슬롯

    [SerializeField]
    private GameObject go_SelectedImage; // 선택된 퀵슬롯의 이미지
    [SerializeField]
    private WeaponManager theWeaponManager;

    private void Start(){
        quickSlots = tf_parent.GetComponentsInChildren<Slot>();
        selectedSlot = 0; // 0~7
    }

    private void SlotSelect(int _num){
        selectedSlot = _num;
        go_SelectedImage.transform.position = quickSlots[selectedSlot].transform.position;
    }

    private void Execute(){
        if(quickSlots[selectedSlot].item != null){
            if(quickSlots[selectedSlot].item.itemType == Item.ItemType.Equipment){
                StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(
                    quickSlots[selectedSlot].item.weaponType, quickSlots[selectedSlot].item.itemName));
            }
            else if(quickSlots[selectedSlot].item.itemType == Item.ItemType.Used){
                StartCoroutine(theWeaponManager.ChangeWeaponCoroutine("HAND", "맨손"));
            }
            else{
                StartCoroutine(theWeaponManager.ChangeWeaponCoroutine("HAND", "맨손"));
            }
        }
        else{
            StartCoroutine(theWeaponManager.ChangeWeaponCoroutine("HAND", "맨손"));
        }
    }

    private void ChangeSlot(int _num){
        SlotSelect(_num);

        Execute();
    }

    private void TryInputNumber(){
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            ChangeSlot(0);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2)){
            ChangeSlot(1);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3)){
            ChangeSlot(2);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4)){
            ChangeSlot(3);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha5)){
            ChangeSlot(4);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha6)){
            ChangeSlot(5);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha7)){
            ChangeSlot(6);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha8)){
            ChangeSlot(7);
        }
    }

    private void Update() {
        TryInputNumber();
    }
}
