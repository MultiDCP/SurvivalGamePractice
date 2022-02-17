using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodOnFire : MonoBehaviour
{
    [SerializeField]
    private float time; // 익히거나 타는 데에 걸리는 시간
    private float currentTime;

    private bool isDone; // 끝났다면 더 이상 불에 있어도 계산할 수 있게 함

    [SerializeField]
    private GameObject go_CookedItemPrefab; // 익혀진 혹은 탄 아이템 교체

    private void OnTriggerStay(Collider other) {
        if(other.transform.tag == "Fire" && !isDone){
            currentTime += Time.deltaTime;

            if(currentTime >= time){
                isDone = true;
                Instantiate(go_CookedItemPrefab, transform.position, Quaternion.Euler(transform.eulerAngles));
                Destroy(gameObject);
            }
        }
    } 
}
