using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // 현재 활성화 여부
    public static bool isActivate = false;

    // 현재 장착된 총
    [SerializeField]
    private Gun currentGun;
    
    // 연사 속도 계산
    private float currentFireRate;

    // 상태 변수
    private bool isReload = false;
    [HideInInspector]
    public bool isFineSightMode = false;

    // 본래 포지션 값
    [SerializeField]
    private Vector3 originPos;

    // 레이캐스트 충돌 정보
    private RaycastHit hitInfo;
    [SerializeField]
    private Camera theCam;
    private Crosshair theCrosshair;

    // 피격 이펙트
    [SerializeField]
    private GameObject hit_effect_prefab;

    [SerializeField]
    private string gun_Fire;

    // 재장전 실행
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

    // 무기 교체 시 재장전 취소
    public void CancelReload(){
        if(isReload){
            StopAllCoroutines();
            isReload = false;
        }
    }

    // 재장전 시도
    private void TryReload(){
        if(Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount){
            CancelFineSight();
            StartCoroutine(ReloadCoroutine());
        }
    }

    // 반동 구현
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

    // 피격 실행
    private void Hit(){
        if(Physics.Raycast(theCam.transform.position, theCam.transform.forward + 
            new Vector3(Random.Range(-theCrosshair.GetAccuracy() - currentGun.accuracy,
                        theCrosshair.GetAccuracy() + currentGun.accuracy),
                        Random.Range(-theCrosshair.GetAccuracy() - currentGun.accuracy,
                        theCrosshair.GetAccuracy() + currentGun.accuracy), 0),
                out hitInfo, currentGun.range)){
            GameObject clone = Instantiate(hit_effect_prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(clone, 2f);
        }
    }

    // 실제 발사
    private void Shoot(){
        theCrosshair.FireAnimation();
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate; // 연사 속도 재계산
        SoundManager.instance.PlaySE(gun_Fire);
        currentGun.muzzleFlash.Play();
        Hit();

        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());
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

    // 발사 시도
    private void TryFire(){
        if(Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload){
            Fire();
        }
    }

    // 연사 속도 계산
    private void GunFireRateCalc(){
        if(currentFireRate > 0){
            currentFireRate -= Time.deltaTime;
        }
    }

    // 정조준 시 총 위치 변경
    IEnumerator FineSightActivateCoroutine(){
        while(currentGun.transform.localPosition != currentGun.fineSightOriginPos){
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f);
            yield return null;
        }
    }

    // 정조준 해제 시 총 위치 복구
    IEnumerator FineSightDeactivateCoroutine(){
        while(currentGun.transform.localPosition != originPos){
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return null;
        }
    }

    // 정조준 과정
    private void FineSight(){
        isFineSightMode = !isFineSightMode;
        theCrosshair.FineSightAnimation(isFineSightMode);
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

    // 특수한 경우 정조준 취소
    public void CancelFineSight(){
        if(isFineSightMode){
            FineSight();
        }
    }

    // 정조준 시도
    private void TryFineSight(){
        if(Input.GetButtonDown("Fire2") && !isReload){
            FineSight();
        }
    }

    void Start(){
        originPos = Vector3.zero;
        theCrosshair = FindObjectOfType<Crosshair>();
    }

    void Update()
    {
        if(isActivate){
            GunFireRateCalc();
            TryFire();
            TryReload();
            TryFineSight();
        }
    }

    public Gun GetGun(){
        return currentGun;
    }

    public bool GetFineSightMode(){
        return isFineSightMode;
    }

    public void GunChange(Gun _gun){
        if(WeaponManager.currentWeapon != null){
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }

        currentGun = _gun;
        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentGun.anim;

        currentGun.transform.localPosition = Vector3.zero;
        currentGun.gameObject.SetActive(true);
        isActivate = true;
    }
}
