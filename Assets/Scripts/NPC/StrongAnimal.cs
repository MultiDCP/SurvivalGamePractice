using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongAnimal : Animal
{
    [SerializeField]
    protected float chaseTime; // 총 추격 시간
    protected float currentChaseTime; // 계산
    [SerializeField]
    protected float chaseDelayTime; // 추격 딜레이

    public void Chase(Vector3 _targetPos){
        //direction = Quaternion.LookRotation(transform.position - _targetPos).eulerAngles;
        isChasing = true;
        destination = _targetPos;
        nav.speed = runSpeed;
        isRunning = true;
        anim.SetBool("Run", isRunning);
        nav.SetDestination(destination);
    }

    public override void Damage(int _dmg, Vector3 _targetPos)
    {
        base.Damage(_dmg, _targetPos);
        if(!isDead){
            Chase(_targetPos);
        }
    }
}
