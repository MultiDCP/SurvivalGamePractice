using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range; // 습득 가능 최대 사거리

    private bool pickUpActivated = false; // 아이템 습득 가능할 시 true
    private bool dissolveActivated = false; // 고기 해체 가능할 시
    private bool isDissolving = false; // 고기 해체 중에는 true
    private bool fireLookActivated = false; // 불 근접해서 바라볼 시 true
    private bool lookComputer = false; // 컴퓨터 바라볼 시 true
    private bool lookArchemyTable = false; // 연금 테이블 바라볼 시 true
    private bool lookActivatedTrap = false; // 가동된 함정 바라볼 시 true

    private RaycastHit hitInfo; // 충돌체 정보 저장

    // 아이템 레이어에만 반응하도록 하여 레이어 마스크 설정
    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private Text actionText;
    [SerializeField]
    private Inventory theInventory;
    [SerializeField]
    private WeaponManager theWeaponManager;
    [SerializeField]
    private QuickSlotController theQuickSlot;
    [SerializeField]
    private Transform tf_MeatDissolveTool; // 고기 해체 툴
    [SerializeField]
    private ComputerKit theComputer;

    [SerializeField]
    private string sound_meat;

    private void ResetInfo(){
        pickUpActivated = false;
        dissolveActivated = false;
        fireLookActivated = false;
    }

    private void ItemInfoAppear(){
        ResetInfo();
        pickUpActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName +
                          " 획득 " + "<color=yellow>" + "(E)" + "</color>";
    }

    private void MeatInfoAppear(){
        ResetInfo();
        if(hitInfo.transform.GetComponent<Animal>().GetDead()){
            dissolveActivated = true;
            actionText.gameObject.SetActive(true);
            actionText.text = hitInfo.transform.GetComponent<Animal>().GetAnimalName() +
                            " 해체하기 " + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    private void FireInfoAppear(){
        ResetInfo();
        if(hitInfo.transform.GetComponent<Fire>().GetIsFire()){
            fireLookActivated = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "선택된 아이템 불에 넣기" + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    private void ComputerInfoAppear(){
        if(!GameManager.isPowerOn){
            ResetInfo();
            lookComputer = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "컴퓨터 가동 " + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    private void ArchemyInfoAppear(){
        if(!GameManager.isOpenArchemyTable){
            ResetInfo();
            lookArchemyTable = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "연금 테이블 조작 " + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    private void TrapInfoAppear(){
        if(hitInfo.transform.GetComponent<DeadTrap>().GetIsActivated()){
            ResetInfo();
            lookActivatedTrap = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "함정 재설치 " + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    private void InfoDisappear(){
        pickUpActivated = false;
        dissolveActivated = false;
        fireLookActivated = false;
        lookComputer = false;
        lookArchemyTable = false;
        lookActivatedTrap = false;
        actionText.gameObject.SetActive(false);
    }

    private void CheckAction(){
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, range, layerMask)){
            Debug.Log(hitInfo.transform.tag);
            if(hitInfo.transform.tag == "Item"){
                ItemInfoAppear();
            }
            else if(hitInfo.transform.tag == "WeakAnimal" || hitInfo.transform.tag == "StrongAnimal"){
                MeatInfoAppear();
            }
            else if(hitInfo.transform.tag == "Fire"){
                Debug.Log("인식은 했다");
                FireInfoAppear();
            }
            else if(hitInfo.transform.tag == "Computer"){
                ComputerInfoAppear();
            }
            else if(hitInfo.transform.tag == "ArchemyTable"){
                ArchemyInfoAppear();
            }
            else if(hitInfo.transform.tag == "Trap"){
                TrapInfoAppear();
            }
            else{
                InfoDisappear();
            }
        }
        else {
            InfoDisappear();
        }
    }

    private void CanPickUp(){
        if(pickUpActivated){
            if(hitInfo.transform != null){
                Debug.Log(hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " 획득");
                theInventory.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
                Destroy(hitInfo.transform.gameObject);
                InfoDisappear();
            }
        }
    }

    IEnumerator MeatCoroutine(){
        RaycastHit tempHitInfo = hitInfo;
        WeaponManager.isChangeWeapon = true;
        WeaponManager.currentWeaponAnim.SetTrigger("Weapon_Out");
        PlayerController.isActivated = false;
        WeaponSway.isActivated = false;
        yield return new WaitForSeconds(0.2f);

        WeaponManager.currentWeapon.gameObject.SetActive(false);
        tf_MeatDissolveTool.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        SoundManager.instance.PlaySE(sound_meat);
        yield return new WaitForSeconds(1.8f);

        theInventory.AcquireItem(tempHitInfo.transform.GetComponent<Animal>().GetItem(), tempHitInfo.transform.GetComponent<Animal>().GetItemNumber());

        WeaponManager.currentWeapon.gameObject.SetActive(true);
        tf_MeatDissolveTool.gameObject.SetActive(false);

        PlayerController.isActivated = true;
        WeaponSway.isActivated = true;
        WeaponManager.isChangeWeapon = false;
        isDissolving = false;
    }

    private void CanMeat(){
        if(dissolveActivated){
            if((hitInfo.transform.tag == "WeakAnimal" || hitInfo.transform.tag == "StrongAnimal") 
                && hitInfo.transform.GetComponent<Animal>().GetDead() && !isDissolving){
                    isDissolving = true;
                    InfoDisappear();
                    StartCoroutine(MeatCoroutine());
            }
        }
    }

    private void DropItem(Slot _selectedSlot){
        switch(_selectedSlot.item.itemType){
            case Item.ItemType.Used:
                if(_selectedSlot.item.itemName.Contains("고기")){
                    Instantiate(_selectedSlot.item.itemPrefab, hitInfo.transform.position + Vector3.up, Quaternion.identity);
                    theQuickSlot.DecreaseSelectedItem();
                }
                break;
            case Item.ItemType.Ingredient:
                break;
        }
    }

    private void CanDropFire(){
        if(fireLookActivated){
            if(hitInfo.transform.tag == "Fire" && hitInfo.transform.GetComponent<Fire>().GetIsFire()){
                Slot _selectedSlot = theQuickSlot.GetSelectedSlot(); // 손에 들고 있는 아이템을 불에 넣음 == 선택된 퀵슬롯의 아이템
                if(_selectedSlot.item != null){
                    DropItem(_selectedSlot);
                }
            }
        }
    }

    private void CanComputerPowerOn(){
        if(lookComputer){
            if(hitInfo.transform != null){
                if(!GameManager.isPowerOn)
                    hitInfo.transform.GetComponent<ComputerKit>().PowerOn();
                    InfoDisappear();
            }
        }
    }

    private void CanArchemyTableOpen(){
        if(lookArchemyTable){
            if(hitInfo.transform != null){
                    hitInfo.transform.GetComponent<ArchemyTable>().Window();
                    InfoDisappear();
            }
        }
    }

    private void CanReInstallTrap(){
        if(lookActivatedTrap){
            if(hitInfo.transform != null){
                    hitInfo.transform.GetComponent<DeadTrap>().ReInstall();
                    InfoDisappear();
            }
        }
    }

    private void TryAction(){
        if(Input.GetKeyDown(KeyCode.E)){
            CheckAction();
            CanPickUp();
            CanMeat();
            CanDropFire();
            CanComputerPowerOn();
            CanArchemyTableOpen();
            CanReInstallTrap();
        }
    }

    void Update()
    {
        CheckAction();
        TryAction();
    }
}
