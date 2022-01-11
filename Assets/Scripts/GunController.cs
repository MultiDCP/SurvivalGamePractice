using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private Gun currentGun;
    
    private float currentFireRate;

    private bool isReload = false;
    private bool isFineSightMode = false;

    // 본래 포지션 값
    [SerializeField]
    private Vector3 originPos;

    private AudioSource audioSource;

    private void PlaySE(AudioClip _clip){
        audioSource.clip = _clip;
        audioSource.Play();
    }

    IEnumerator ReloadCoroutine(){
        if(currentGun.carryBulletcount > 0){
            isReload = true;
            currentGun.anim.SetTrigger("Reload");

            currentGun.carryBulletcount += currentGun.currentBulletCount;
            currentGun.currentBulletCount = 0;

            yield return new WaitForSeconds(currentGun.reloadTime);

            if(currentGun.carryBulletcount >= currentGun.reloadBulletCount){
                currentGun.currentBulletCount += currentGun.reloadBulletCount;
                currentGun.carryBulletcount -= currentGun.reloadBulletCount;
            }
            else {
                currentGun.currentBulletCount = currentGun.carryBulletcount;
                currentGun.carryBulletcount = 0;
            }

            isReload = false;
        }
        else {
            Debug.Log("소유한 총알이 없습니다.");
        }
    }

    private void TryReload(){
        if(Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount){
            CancelFineSight();
            StartCoroutine(ReloadCoroutine());
        }
    }

    IEnumerator RetroActionCoroutine(){
        Vector3 recoilBack = new Vector3(currentGun.retroActionForce, originPos.y, originPos.z);
        Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z);

        if(!isFineSightMode){
            currentGun.transform.localPosition = originPos;
            // 반동 시작
            while(currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f){
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }

            // 원위치
            while(currentGun.transform.localPosition != originPos){
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        }
        else {
            currentGun.transform.localPosition = currentGun.fineSightOriginPos;
            // 반동 시작
            while(currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f){
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }

            // 원위치
            while(currentGun.transform.localPosition != currentGun.fineSightOriginPos){
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                yield return null;
            }
        }
    }

    // 실제 발사
    private void Shoot(){
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate; // 연사 속도 재계산
        PlaySE(currentGun.fire_Sound);
        currentGun.muzzleFlash.Play();

        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());

        Debug.Log("격발됨");
    }

    // 발사 과정
    private void Fire(){
        if(!isReload){
            if(currentGun.currentBulletCount > 0){
                Shoot();
            }
            else {
                CancelFineSight();
                StartCoroutine(ReloadCoroutine());
            }
        }
    }

    private void TryFire(){
        if(Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload){
            Fire();
        }
    }

    private void GunFireRateCalc(){
        if(currentFireRate > 0){
            currentFireRate -= Time.deltaTime;
        }
    }

    IEnumerator FineSightActivateCoroutine(){
        while(currentGun.transform.localPosition != currentGun.fineSightOriginPos){
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f);
            yield return null;
        }
    }

    IEnumerator FineSightDeactivateCoroutine(){
        while(currentGun.transform.localPosition != originPos){
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return null;
        }
    }

    private void FineSight(){
        isFineSightMode = !isFineSightMode;
        currentGun.anim.SetBool("FineSightMode", isFineSightMode);

        if(isFineSightMode){
            StopAllCoroutines();
            StartCoroutine(FineSightActivateCoroutine());
        }
        else {
            StopAllCoroutines();
            StartCoroutine(FineSightDeactivateCoroutine());
        }
    }

    public void CancelFineSight(){
        if(isFineSightMode){
            FineSight();
        }
    }

    private void TryFineSight(){
        if(Input.GetButtonDown("Fire2") && !isReload){
            FineSight();
        }
    }

    void Start(){
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        GunFireRateCalc();
        TryFire();
        TryReload();
        TryFineSight();
    }
}
