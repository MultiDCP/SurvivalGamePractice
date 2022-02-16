using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : MonoBehaviour
{
    private bool isBurning = false;

    [SerializeField] private int damage; // 화상 데미지
    
    [SerializeField] private float damageTime;
    private float currentDamageTime;

    [SerializeField] private float durationTime;
    private float currentDurationTime;

    [SerializeField]
    private GameObject flame_Prefab; // 불 붙으면 프리팹 생성
    private GameObject go_tempFlame; // 프리팹 그릇

    public void BurningStart(){
        if(!isBurning){
            go_tempFlame = Instantiate(flame_Prefab, transform.position, Quaternion.Euler(new Vector3 (-90f, 0f, 0f)));
            go_tempFlame.transform.SetParent(transform);
        }
        isBurning = true;
        currentDurationTime = durationTime;
    }

    private void Damage(){
        currentDamageTime = damageTime;
        GetComponent<StatusController>().DecreaseHp(damage);
    }

    private void BurnOff(){
        isBurning = false;
        Destroy(go_tempFlame);
    }

    private void ElapseTime(){
        if(isBurning){
            currentDurationTime -= Time.deltaTime;

            if(currentDamageTime > 0){
                currentDamageTime -= Time.deltaTime;
            }

            if(currentDamageTime <= 0){
                Damage(); // 데미지 입힘
            }

            if(currentDurationTime <= 0){
                BurnOff(); // 불을 끔
            }
        }
    }

    private void Update() {
        if(isBurning){
            ElapseTime();
        }
    }
}
