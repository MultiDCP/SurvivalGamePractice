using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : CloseWeaponController
{
    // 현재 활성화 여부
    public static bool isActivate = false;
    public static Item currentKit; // 설치하려는 키트

    private bool isPreview = false;

    private GameObject go_preview; // 설치할 키트 프리뷰
    private Vector3 previewPos; // 설치할 키트 위치
    [SerializeField] private float rangeAdd; // 건축 시 추가 사정거리
    
    [SerializeField]
    private QuickSlotController theQuickSlot;

    protected override IEnumerator HitCoroutine(){
        while(isSwing){
            if(CheckObject()){
                isSwing = false;
                // 충돌 감지
                Debug.Log(hitInfo.transform.name);
            }
            yield return null;
        }
    }

    public override void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        base.CloseWeaponChange(_closeWeapon);
        isActivate = true;;
    }

    private void TryEating(){
        if(Input.GetButtonDown("Fire2") && !theQuickSlot.GetIsGoolTime()){
            currentCloseWeapon.anim.SetTrigger("Eat");
            theQuickSlot.DecreaseSelectedItem();
        }
    }

    private void InstallPreviewKit(){
        isPreview = true;
        go_preview = Instantiate(currentKit.kitPreviewPrefab, transform.position, Quaternion.identity);
    }

    private void PreviewPositionUpdate(){
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range + rangeAdd, layerMask)){
            previewPos = hitInfo.point;
            go_preview.transform.position = previewPos;
        }
    }

    private void BuildKit(){
        if(Input.GetButtonDown("Fire1")){
            if(go_preview.GetComponent<PreviewObject>().isBuildable()){
                theQuickSlot.DecreaseSelectedItem(); // 슬롯 아이템 개수--
                GameObject temp = Instantiate(currentKit.kitPrefab, previewPos, Quaternion.identity);
                temp.name = currentKit.itemName;
                Destroy(go_preview);
                currentKit = null;
                isPreview = false;
            }
        }
    }

    public void CancelKit(){
        Destroy(go_preview);
        currentKit = null;
        isPreview = false;
    }

    protected void Update()
    {
        if(isActivate && !Inventory.inventoryActivated){
            if(currentKit == null){
                if(QuickSlotController.go_HandItem == null)
                    TryAttack();
                else
                    TryEating();
            }
            else{
                if(!isPreview)
                    InstallPreviewKit();
                PreviewPositionUpdate();
                BuildKit();
            }
        }    
    }
}
