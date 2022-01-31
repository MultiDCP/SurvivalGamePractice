using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeController : CloseWeaponController
{
    // 현재 활성화 여부
    public static bool isActivate = true;

    protected override IEnumerator HitCoroutine(){
        while(isSwing){
            if(CheckObject()){
                if(hitInfo.transform.tag == "Rock"){
                    hitInfo.transform.GetComponent<Rock>().Mining();
                }
                else if(hitInfo.transform.tag == "Twig"){
                    hitInfo.transform.GetComponent<Twig>().Damage(this.transform);
                }
                else if(hitInfo.transform.tag == "WeakAnimal"){
                    SoundManager.instance.PlaySE("Animal_Hit");
                    hitInfo.transform.GetComponent<WeakAnimal>().Damage(currentCloseWeapon.damage, transform.position);
                }

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

    private void Start() {
        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;
    }

    protected void Update()
    {
        if(isActivate){
            TryAttack();
        }
    }
}
