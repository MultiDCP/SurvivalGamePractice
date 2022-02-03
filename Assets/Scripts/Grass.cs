using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    [SerializeField]
    private int hp; // 풀 체력

    [SerializeField]
    private float destroyTime; // 이펙트 제거 시간
    [SerializeField]
    private float force; // 폭발력 세기

    [SerializeField]
    private GameObject go_hit_effect_prefab; // 타격 효과

    private Rigidbody[] rigidbodies;
    private BoxCollider[] boxColliders;

    [SerializeField]
    private string hit_sound;

    private void Start() {
        rigidbodies = this.transform.GetComponentsInChildren<Rigidbody>();
        boxColliders = transform.GetComponentsInChildren<BoxCollider>();
    }

    private void Hit(){
        SoundManager.instance.PlaySE(hit_sound);

        var clone = Instantiate(go_hit_effect_prefab, transform.position + Vector3.up, Quaternion.identity);
        Destroy(clone, destroyTime);
    }

    private void Destruction(){
        for(int i=0; i<rigidbodies.Length; i++){
            rigidbodies[i].useGravity = true;
            rigidbodies[i].AddExplosionForce(force, transform.position, 1f);
            boxColliders[i].enabled = true;
        }

        Destroy(this.gameObject, destroyTime);
    }

    public void Damage(){
        hp--;

        Hit();

        if(hp <= 0){
            Destruction();
        }
    }
}
