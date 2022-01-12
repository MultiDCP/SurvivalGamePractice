using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : CloseWeaponController
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
        isActivate = true;;
    }

    protected void Update()
    {
        if(isActivate){
            TryAttack();
        }
    }
}
