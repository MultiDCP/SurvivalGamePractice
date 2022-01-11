using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // 필요 컴포넌트
    [SerializeField]
    private GunController theGunController;
    private Gun currentGun;

    // 필요 시 HUD 호출, 필요X 경우 HUD 비활성화
    [SerializeField]
    private GameObject go_BulletHUD;

    // 총알 개수 텍스트
    [SerializeField]
    private Text[] text_Bullet;
    
    private void CheckBullet(){
        currentGun = theGunController.GetGun();
        text_Bullet[0].text = currentGun.carryBulletcount.ToString();
        text_Bullet[1].text = currentGun.reloadBulletCount.ToString();
        text_Bullet[2].text = currentGun.currentBulletCount.ToString();
    }

    void Update()
    {
        CheckBullet();
    }
}
