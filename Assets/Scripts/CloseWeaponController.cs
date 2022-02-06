using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// abstract -> 대충 미완성이라는 뜻
public abstract class CloseWeaponController : MonoBehaviour
{
    // 현재 장착된 Hand형 타입 무기
    [SerializeField]
    protected CloseWeapon currentCloseWeapon;

    // 공격중 유무
    protected bool isAttack = false;
    protected bool isSwing = false;

    protected RaycastHit hitInfo;
    [SerializeField]
    protected LayerMask layerMask;

    [SerializeField]
    private PlayerController thePlayerController;

    protected bool CheckObject(){
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range, layerMask)){
            return true;
        }
        return false;
    }

    protected abstract IEnumerator HitCoroutine();

    protected IEnumerator AttackCoroutine(string swingType, float _delayA, float _delayB, float _delayC){
        isAttack = true;
        currentCloseWeapon.anim.SetTrigger(swingType); // 애니메이션 트리거 작동

        yield return new WaitForSeconds(_delayA);
        isSwing = true;

        // 공격 활성화 시점
        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(_delayB);
        isSwing = false;

        yield return new WaitForSeconds(_delayC - _delayA - _delayB);
        isAttack = false;
    }

    protected void TryAttack(){
        if(!Inventory.inventoryActivated){
            if(Input.GetButton("Fire1")){
                if(!isAttack){
                    if(CheckObject()){
                        if(currentCloseWeapon.isAxe && hitInfo.transform.tag == "Tree"){
                            StartCoroutine(thePlayerController.TreeLookCoroutine(hitInfo.transform.GetComponent<TreeComponent>().GetTreeCenterPos()));
                            StartCoroutine(AttackCoroutine("Chop", currentCloseWeapon.workDelayA, currentCloseWeapon.workDelayB, currentCloseWeapon.workDelay));
                            return;
                        }
                    }

                    StartCoroutine(AttackCoroutine("Attack", currentCloseWeapon.attackDelayA, currentCloseWeapon.attackDelayB, currentCloseWeapon.attackDelay));
                }
            }
        }
    }

    // 가상 함수(완성 상태이지만 추가 편집 가능)
    public virtual void CloseWeaponChange(CloseWeapon _closeWeapon){
        if(WeaponManager.currentWeapon != null){
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }

        currentCloseWeapon = _closeWeapon;
        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;

        currentCloseWeapon.transform.localPosition = Vector3.zero;
        currentCloseWeapon.gameObject.SetActive(true);
    }
}
