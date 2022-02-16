using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] private string fireName; // 불 이름

    [SerializeField] private int damage; // 불 데미지

    [SerializeField] private float damageTime; // 데미지가 들어갈 딜레이 시간
    private float currentDamageTime;

    [SerializeField] private float durationTime; // 불의 지속시간
    private float currentDurationTime;

    [SerializeField]
    private ParticleSystem ps_Flame; // 파티클 시스템

    // 필요 컴포넌트
    private StatusController thePlayerStatus;

    // 상태 변수
    private bool isFire = true;

    private void Start() {
        thePlayerStatus = FindObjectOfType<StatusController>();
        currentDurationTime = durationTime;
    }

    private void FireOff(){
        ps_Flame.Stop();
        isFire = false;
    }

    private void ElapseTime(){
        currentDurationTime -= Time.deltaTime;

        if(currentDamageTime > 0){
            currentDamageTime -= Time.deltaTime;
        }

        if(currentDurationTime <= 0){
            FireOff();// 불 끔
        }
    }
    
    private void OnTriggerStay(Collider other) {
        if(isFire && other.transform.tag == "Player"){
            if(currentDamageTime <= 0){
                other.GetComponent<Burn>().BurningStart();
                thePlayerStatus.DecreaseHp(damage);
                currentDamageTime = damageTime;
            }
        }
    }

    private void Update() {
        if(isFire){
            ElapseTime();
        }
    }

}
