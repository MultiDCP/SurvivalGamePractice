using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    // 바위 체력
    [SerializeField]
    private int hp;

    // 파편 제거 시간
    [SerializeField]
    private float destroyTime; 
    
    [SerializeField]
    private SphereCollider col;

    // 일반 바위 오브젝트
    [SerializeField]
    private GameObject go_rock;
    // 깨진 바위 오브젝트
    [SerializeField]
    private GameObject go_debris;
    // 채굴 이펙트
    [SerializeField]
    private GameObject go_effect_debris;
    [SerializeField]
    private GameObject go_rock_item_prefab;

    // 드랍 아이템 개수
    [SerializeField]
    private int count;

    // 필요 사운드
    [SerializeField]
    private string strike_Sound;
    [SerializeField]
    private string destroy_Sound;

    private void Destruction(){
        SoundManager.instance.PlaySE(destroy_Sound);
        col.enabled = false;

        for(int i=0; i<count; i++){
            Instantiate(go_rock_item_prefab, go_rock.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
        Destroy(go_rock);

        go_debris.SetActive(true);
        Destroy(go_debris, destroyTime);
    }

    public void Mining(){
        SoundManager.instance.PlaySE(strike_Sound);
        GameObject clone = Instantiate(go_effect_debris, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);

        hp--;

        if(hp <= 0){
            Destruction();
        }
    }
}
