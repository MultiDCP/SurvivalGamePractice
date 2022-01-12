using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeController : CloseWeaponController
{
    // 현재 활성화 여부
    public static bool isActivate = false;

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
        isActivate = true;
    }

    void Start() {
        //WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        //WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;
    }

    void Update()
    {
        if(isActivate){
            TryAttack();
        }
    }
}