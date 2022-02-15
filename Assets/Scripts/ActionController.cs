using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range; // 습득 가능 최대 사거리

    private bool pickUpActivated = false; // 습득 가능할 시 true
    private bool dissolveActivated = false; // 고기 해체 가능할 시
    private bool isDissolving = false; // 고기 해체 중에는 true

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
    private Transform tf_MeatDissolveTool; // 고기 해체 툴

    [SerializeField]
    private string sound_meat;

    private void ItemInfoAppear(){
        pickUpActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName +
                          " 획득 " + "<color=yellow>" + "(E)" + "</color>";
    }

    private void MeatInfoAppear(){
        if(hitInfo.transform.GetComponent<Animal>().GetDead()){
            dissolveActivated = true;
            actionText.gameObject.SetActive(true);
            actionText.text = hitInfo.transform.GetComponent<Animal>().GetAnimalName() +
                            " 해체하기 " + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    private void InfoDisappear(){
        pickUpActivated = false;
        dissolveActivated = false;
        actionText.gameObject.SetActive(false);
    }

    private void CheckAction(){
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, range, layerMask)){
            if(hitInfo.transform.tag == "Item"){
                ItemInfoAppear();
            }
            else if(hitInfo.transform.tag == "WeakAnimal" || hitInfo.transform.tag == "StrongAnimal"){
                MeatInfoAppear();
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

        theInventory.AcquireItem(hitInfo.transform.GetComponent<Animal>().GetItem(), hitInfo.transform.GetComponent<Animal>().GetItemNumber());

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

    private void TryAction(){
        if(Input.GetKeyDown(KeyCode.E)){
            CheckAction();
            CanPickUp();
            CanMeat();
        }
    }

    void Update()
    {
        CheckAction();
        TryAction();
    }
}
