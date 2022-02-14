using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigWild : StrongAnimal
{
    protected override void Update()
    {
        base.Update();
        if(theViewAngle.View() && !isDead){
            StopAllCoroutines();
            StartCoroutine(ChaseTargetCoroutine());
        }
    }

    IEnumerator ChaseTargetCoroutine(){
        currentChaseTime = 0;

        while(currentChaseTime < chaseTime){
            Chase(theViewAngle.GetTargetPos());
            yield return new WaitForSeconds(chaseDelayTime);
            currentChaseTime += chaseDelayTime;
        }

        isChasing = false;
        isRunning = false;
        anim.SetBool("Run", isRunning);
        nav.ResetPath();
    }
}
