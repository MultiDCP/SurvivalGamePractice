using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private Gun currentGun;
    
    private float currentFireRate;

    private AudioSource audioSource;

    private void PlaySE(AudioClip _clip){
        audioSource.clip = _clip;
        audioSource.Play();
    }

    // 실제 발사
    private void Shoot(){
        PlaySE(currentGun.fire_Sound);
        currentGun.muzzleFlash.Play();
        Debug.Log("격발됨");
    }

    // 발사 과정
    private void Fire(){
        currentFireRate = currentGun.fireRate;
        Shoot();
    }

    private void TryFire(){
        if(Input.GetButton("Fire1") && currentFireRate <= 0){
            Fire();
        }
    }

    private void GunFireRateCalc(){
        if(currentFireRate > 0){
            currentFireRate -= Time.deltaTime;
        }
    }

    void Start(){
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        GunFireRateCalc();
        TryFire();
    }
}
