using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    // 현재 장착된 Hand형 타입 무기
    [SerializeField]
    private Hand currentHand;

    // 공격중 유무
    private bool isAttack = false;
    private bool isSwing = false;

    private RaycastHit hitInfo;

    private bool CheckObject(){
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, currentHand.range)){
            return true;
        }
        return false;
    }

    IEnumerator HitCoroutine(){
        while(isSwing){
            if(CheckObject()){
                isSwing = false;
                // 충돌 감지
                Debug.Log(hitInfo.transform.name);
            }
            yield return null;
        }
    }

    IEnumerator AttackCoroutine(){
        isAttack = true;
        currentHand.anim.SetTrigger("Attack"); // 애니메이션 트리거 작동

        yield return new WaitForSeconds(currentHand.attackDelayA);
        isSwing = true;

        // 공격 활성화 시점
        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(currentHand.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentHand.attackDelay - currentHand.attackDelayA - currentHand.attackDelayB);
        isAttack = false;
    }

    private void TryAttack(){
        if(Input.GetButton("Fire1")){
            if(!isAttack){
                StartCoroutine(AttackCoroutine());
            }
        }
    }

    void Update()
    {
        TryAttack();
    }
}
