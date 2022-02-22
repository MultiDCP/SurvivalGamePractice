using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotController : MonoBehaviour
{
    [SerializeField]
    private Slot[] quickSlots; // 퀵슬롯들
    [SerializeField]
    private Image[] img_Cooltime; // 퀵슬롯 쿨타임 이미지
    [SerializeField]
    private Transform tf_parent; // 퀵슬롯의 부모 객체

    [SerializeField]
    private Transform tf_ItemPos; // 아이템이 위치할 손 끝
    public static GameObject go_HandItem; // 손에 든 아이템 정보 저장

    // 쿨타임 관련
    [SerializeField]
    private float coolTime;
    private float currentCoolTime;
    private bool isCoolTime;

    private int selectedSlot; // 선택된 퀵슬롯

    [SerializeField]
    private GameObject go_SelectedImage; // 선택된 퀵슬롯의 이미지
    [SerializeField]
    private WeaponManager theWeaponManager;
    private Animator anim;

    // 등장 내용
    [SerializeField]
    private float appearTime;
    private float currentAppearTime;
    private bool isAppear;

    private void Start(){
        quickSlots = tf_parent.GetComponentsInChildren<Slot>();
        anim = GetComponent<Animator>();
        selectedSlot = 0; // 0~7
    }

    public void IsActivatedQuickSlot(int _num){
        if(selectedSlot == _num){
            Execute();
            return;
        }
        if(DragSlot.instance != null){
            if(DragSlot.instance.dragSlot != null){
                if(DragSlot.instance.dragSlot.GetQuickSlotNumber() == selectedSlot){
                    Execute();
                    return;
                }
            }
        }
    }

    private void SlotSelect(int _num){
        selectedSlot = _num;
        go_SelectedImage.transform.position = quickSlots[selectedSlot].transform.position;
    }

    IEnumerator HandItemCoroutine(Item _item){
        HandController.isActivate = false;
        yield return new WaitUntil(() => HandController.isActivate);

        if(_item.itemType == Item.ItemType.Kit)
            HandController.currentKit = _item;

        go_HandItem = Instantiate(quickSlots[selectedSlot].item.itemPrefab, tf_ItemPos.position, tf_ItemPos.rotation);
        go_HandItem.GetComponent<Rigidbody>().isKinematic = true;
        go_HandItem.GetComponent<BoxCollider>().enabled = false;
        go_HandItem.tag = "Untagged";
        go_HandItem.layer = 9; // Weapon Layer
        go_HandItem.transform.SetParent(tf_ItemPos);
    }

    private void ChangeHand(Item _item = null){
        StartCoroutine(theWeaponManager.ChangeWeaponCoroutine("HAND", "맨손"));

        if(_item != null){
            StartCoroutine(HandItemCoroutine(_item));// 아이템 손 끝에 생성
        }
    }

    private void CoolTimeReset(){
        currentCoolTime = coolTime;
        isCoolTime = true;
    }

    private void Execute(){
        CoolTimeReset();
        AppearReset();

        if(quickSlots[selectedSlot].item != null){
            if(quickSlots[selectedSlot].item.itemType == Item.ItemType.Equipment){
                StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(
                    quickSlots[selectedSlot].item.weaponType, quickSlots[selectedSlot].item.itemName));
            }
            else if(quickSlots[selectedSlot].item.itemType == Item.ItemType.Used || quickSlots[selectedSlot].item.itemType == Item.ItemType.Kit){
                ChangeHand(quickSlots[selectedSlot].item);
            }
            else{
                ChangeHand();
            }
        }
        else{
            ChangeHand();
        }
    }

    private void ChangeSlot(int _num){
        SlotSelect(_num);

        Execute();
    }

    private void TryInputNumber(){
        if(!isCoolTime){
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
    }

    private void CoolTimeCalc(){
        if(isCoolTime){
            currentCoolTime -= Time.deltaTime;
            for(int i=0; i<img_Cooltime.Length; i++)
                img_Cooltime[i].fillAmount = currentCoolTime / coolTime;

            if(currentCoolTime <= 0)
                isCoolTime = false;
        }
    }

    private void AppearReset(){
        currentAppearTime = appearTime;
        isAppear = true;
        anim.SetBool("Appear", isAppear);
    }
    
    private void AppearCalc(){
        if(Inventory.inventoryActivated){
            AppearReset();
        }
        else{
            if(isAppear){
                currentAppearTime -= Time.deltaTime;
                if(currentAppearTime <= 0){
                    isAppear = false;
                    anim.SetBool("Appear", isAppear);
                }
            }
        }
    }

    private void Update() {
        TryInputNumber();
        CoolTimeCalc();
        AppearCalc();
    }

    public void DecreaseSelectedItem(){
        CoolTimeReset();
        AppearReset();

        quickSlots[selectedSlot].SetSlotCount(-1);

        if(quickSlots[selectedSlot].itemCount <= 0)
            Destroy(go_HandItem);
    }

    public bool GetIsGoolTime(){
        return isCoolTime;
    }

    public Slot GetSelectedSlot(){
        return quickSlots[selectedSlot];
    }
}
