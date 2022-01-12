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

    protected bool CheckObject(){
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range)){
            return true;
        }
        return false;
    }

    protected abstract IEnumerator HitCoroutine();

    protected IEnumerator AttackCoroutine(){
        isAttack = true;
        currentCloseWeapon.anim.SetTrigger("Attack"); // 애니메이션 트리거 작동

        yield return new WaitForSeconds(currentCloseWeapon.attackDelayA);
        isSwing = true;

        // 공격 활성화 시점
        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(currentCloseWeapon.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentCloseWeapon.attackDelay - currentCloseWeapon.attackDelayA - currentCloseWeapon.attackDelayB);
        isAttack = false;
    }

    protected void TryAttack(){
        if(Input.GetButton("Fire1")){
            if(!isAttack){
                StartCoroutine(AttackCoroutine());
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
