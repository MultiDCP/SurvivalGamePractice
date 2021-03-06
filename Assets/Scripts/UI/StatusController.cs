using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    // 체력
    [SerializeField]
    private int hp;
    private int currentHp;

    // 스태미나
    [SerializeField]
    private int sp;
    private int currentSp;

    // 스태미나 증가량
    [SerializeField]
    private int spIncreaseSpeed;

    // 스태미나 재회복 딜레이
    [SerializeField]
    private int spRechargeTime;
    private int currentSpRechargeTime;

    // 스태미나 감소 여부
    private bool spUsed;

    // 방어력
    [SerializeField]
    private int dp;
    private int currentDp;

    // 허기
    [SerializeField]
    private int hungry;
    private int currentHungry;

    // 갈증
    [SerializeField]
    private int thirsty;
    private int currentThirsty;

    // 허기가 줄어드는 속도
    [SerializeField]
    private int hungryDecreaseTime;
    private int currentHungryDecreaseTime;

    // 갈증이 줄어드는 속도
    [SerializeField]
    private int thirstyDecreaseTime;
    private int currentThirstyDecreaseTime;

    // 만족도
    [SerializeField]
    private int satisfy;
    private int currentSatisfy;

    // 필요 이미지
    [SerializeField]
    private Image[] images_Gauge;

    private const int HP = 0, DP = 1, SP = 2, HUNGRY = 3, THIRSTY = 4, SATISFY = 5;

    void Start()
    {
        currentHp = hp;
        currentDp = dp;
        currentSp = sp;
        currentHungry = hungry;
        currentThirsty = thirsty;
        currentSatisfy = satisfy;
    }

    private void Hungry(){
        if(currentHungry > 0){
            if(currentHungryDecreaseTime <= hungryDecreaseTime){
                currentHungryDecreaseTime++;
            }
            else{
                currentHungry--;
                currentHungryDecreaseTime = 0;   
            }
        }
        else{
            Debug.Log("배고픔 수치가 0이 되었습니다.");
        }
    }

    private void Thirsty(){
        if(currentThirsty > 0){
            if(currentThirstyDecreaseTime <= thirstyDecreaseTime){
                currentThirstyDecreaseTime++;
            }
            else{
                currentThirsty--;
                currentThirstyDecreaseTime = 0;   
            }
        }
        else{
            Debug.Log("목마름 수치가 0이 되었습니다.");
        }
    }

    public void IncreaseHP(int _count){
        if(currentHp + _count < hp){
            currentHp += _count;
        }
        else {
            currentHp = hp;
        }
    }

    public void DecreaseHp(int _count){
        if(currentDp > 0){
            DecreaseDp(_count);
            return;
        }
        currentHp -= _count;

        if(currentHp <= 0){
            Debug.Log("캐릭터의 HP가 0이 되었습니다.");
        }
    }

    public void IncreaseSP(int _count){
        if(currentSp + _count < sp){
            currentSp += _count;
        }
        else {
            currentSp = sp;
        }
    }

    public void DecreaseSp(int _count){
        currentSp -= _count;

        if(currentSp <= 0){
            Debug.Log("캐릭터의 SP가 0이 되었습니다.");
        }
    }

    public void IncreaseDP(int _count){
        if(currentDp + _count < dp){
            currentDp += _count;
        }
        else {
            currentDp = dp;
        }
    }

    public void DecreaseDp(int _count){
        currentDp -= _count;

        if(currentDp <= 0){
            Debug.Log("캐릭터의 DP가 0이 되었습니다.");
        }
    }

    public void IncreaseHungry(int _count){
        if(currentHungry + _count < hungry){
            currentHungry += _count;
        }
        else {
            currentHungry = hungry;
        }
    }

    public void DecreaseHungry(int _count){
        if(currentHungry - _count < 0){
            currentHungry = 0;
        }
        else {
            currentHungry -= _count;
        }
    }

    public void IncreaseThirsty(int _count){
        if(currentThirsty + _count < thirsty){
            currentThirsty += _count;
        }
        else {
            currentThirsty = thirsty;
        }
    }

    public void DecreaseThirsty(int _count){
        if(currentThirsty - _count < 0){
            currentThirsty = 0;
        }
        else {
            currentThirsty -= _count;
        }
    }

    public void IncreaseSatisfy(int _count){
        if(currentSatisfy + _count < satisfy){
            currentSatisfy += _count;
        }
        else {
            currentSatisfy = satisfy;
        }
    }

    public void DecreaseSatisfy(int _count){
        if(currentSatisfy - _count < 0){
            currentSatisfy = 0;
        }
        else {
            currentSatisfy -= _count;
        }
    }

    public void DecreaseStamina(int _count){
        spUsed = true;
        currentSpRechargeTime = 0;

        if(currentSp - _count > 0){
            currentSp -= _count;
        }
        else {
            currentSp = 0;
        }
    }

    public int GetCurrentSP(){
        return currentSp;
    }

    private void SPRecover(){
        if(!spUsed && currentSp < sp){
            currentSp += spIncreaseSpeed;
        }
    }

    private void SPRechargeTime(){
        if(spUsed){
            if(currentSpRechargeTime < spRechargeTime){
                currentSpRechargeTime++;
            }
            else{
                spUsed = false;
            }
        }
    }

    private void GaugeUpdate(){
        images_Gauge[HP].fillAmount = (float)currentHp / hp;
        images_Gauge[SP].fillAmount = (float)currentSp / sp;
        images_Gauge[DP].fillAmount = (float)currentDp / dp;
        images_Gauge[HUNGRY].fillAmount = (float)currentHungry / hungry;
        images_Gauge[THIRSTY].fillAmount = (float)currentThirsty / thirsty;
        images_Gauge[SATISFY].fillAmount = (float)currentSatisfy / satisfy;

    }

    void Update()
    {
        Hungry();
        Thirsty();
        SPRechargeTime();
        SPRecover();
        GaugeUpdate();
    }
}
