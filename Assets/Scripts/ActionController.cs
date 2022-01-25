using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range; // 습득 가능 최대 사거리

    private bool pickUpActivated = false; // 습득 가능할 시 true

    private RaycastHit hitInfo; // 충돌체 정보 저장

    // 아이템 레이어에만 반응하도록 하여 레이어 마스크 설정
    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private Text actionText;
    [SerializeField]
    private Inventory theInventory;

    private void ItemInfoAppear(){
        pickUpActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName +
                          " 획득 " + "<color=yellow>" + "(E)" + "</color>";
    }

    private void ItemInfoDisappear(){
        pickUpActivated = false;
        actionText.gameObject.SetActive(false);
    }

    private void CheckItem(){
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, range, layerMask)){
            if(hitInfo.transform.tag == "Item"){
                ItemInfoAppear();
            }
        }
        else {
            ItemInfoDisappear();
        }
    }

    private void CanPickUp(){
        if(pickUpActivated){
            if(hitInfo.transform != null){
                Debug.Log(hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " 획득");
                theInventory.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
                Destroy(hitInfo.transform.gameObject);
                ItemInfoDisappear();
            }
        }
    }

    private void TryAction(){
        if(Input.GetKeyDown(KeyCode.E)){
            CheckItem();
            CanPickUp();
        }
    }

    void Update()
    {
        CheckItem();
        TryAction();
    }
}
