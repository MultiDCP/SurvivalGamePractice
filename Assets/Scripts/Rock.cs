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
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip effect_sound;
    [SerializeField]
    private AudioClip effect_sound2;


    private void Destruction(){
        audioSource.clip = effect_sound2;
        audioSource.Play();

        col.enabled = false;
        Destroy(go_rock);

        go_debris.SetActive(true);
        Destroy(go_debris, destroyTime);
    }

    public void Mining(){
        audioSource.clip = effect_sound;
        audioSource.Play();
        GameObject clone = Instantiate(go_effect_debris, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);

        hp--;

        if(hp <= 0){
            Destruction();
        }
    }
}
