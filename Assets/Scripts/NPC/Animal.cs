using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    [SerializeField]
    protected string animalName; // 동물 이름
    [SerializeField]
    protected int hp; // 동물의 체력

    [SerializeField]
    protected float walkSpeed; // 걷기 스피드
    [SerializeField]
    protected float runSpeed; // 뛰기 스피드
    //[SerializeField]
    //protected float turningSpeed; // 회전 스피드  
    //protected float applySpeed;

    //protected Vector3 direction; // 방향
    protected Vector3 destination; // 목적지

    // 상태 변수
    protected bool isAction; // 행동중인지 여부 판별
    protected bool isWalking; // 걷는지 여부 판별
    protected bool isRunning; // 뛰는지 여부 판별
    protected bool isChasing; // 추격중인지 판별
    protected bool isDead;

    [SerializeField]
    protected float walkTime; // 걷기 시간
    [SerializeField]
    protected float waitTime; // 대기 시간
    [SerializeField]
    protected float runTime;
    protected float currentTime;

    [SerializeField]
    protected Animator anim;
    [SerializeField]
    protected Rigidbody rigid;
    [SerializeField]
    protected BoxCollider boxCol;
    protected AudioSource theAudio;
    protected NavMeshAgent nav;
    protected FieldOfViewAngle theViewAngle;

    [SerializeField]
    protected AudioClip[] sound_Normal;
    [SerializeField]
    protected AudioClip sound_Hurt;
    [SerializeField]
    protected AudioClip sound_Dead;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        theViewAngle = GetComponent<FieldOfViewAngle>();
        theAudio = GetComponent<AudioSource>();
        currentTime = waitTime;
        isAction = true;
    }

    protected void TryWalk(){
        isWalking = true;
        anim.SetBool("Walk", isWalking);
        currentTime = walkTime;
        nav.speed = walkSpeed;
        Debug.Log("걷기");
    }

    protected void Dead(){
        PlaySE(sound_Dead);
        isWalking = false;
        isRunning = false;
        isDead = true;
        anim.SetTrigger("Dead");
    }

    public virtual void Damage(int _dmg, Vector3 _targetPos){
        if(!isDead){
            hp -= _dmg;

            if(hp <= 0){
                Dead();
                return;
            }

            PlaySE(sound_Hurt);
            anim.SetTrigger("Hurt");
        }   
    }

    protected virtual void ResetAnimal(){
        isWalking = false;
        isAction = true;
        isRunning = false;

        nav.speed = walkSpeed;
        nav.ResetPath();

        anim.SetBool("Walk", isWalking);
        anim.SetBool("Run", isRunning);
        //direction.Set(0f, Random.Range(0f, 360f), 0f);
        destination.Set(Random.Range(-0.2f, 0.2f), 0f, Random.Range(0.5f, 1f));
    }
    
    

    protected void ElapseTime(){
        if(isAction){
            currentTime -= Time.deltaTime;
            if(currentTime <= 0 && !isChasing){
                ResetAnimal();
            }
        }
    }

    protected void Move(){
        if(isWalking || isRunning){
            //rigid.MovePosition(transform.position + (transform.forward * applySpeed * Time.deltaTime));
            nav.SetDestination(transform.position + destination * 5f);
        }
    }
/*
    protected void Rotation(){
        if(isWalking || isRunning){
            Vector3 _rotation = Vector3.Lerp(transform.eulerAngles, new Vector3(0f, direction.y, 0f), turningSpeed);
            rigid.MoveRotation(Quaternion.Euler(_rotation));
        }
    }
*/
    protected void RandomSound(){
        int _random = Random.Range(0, 3); // 일상 사운드 3개
        PlaySE(sound_Normal[_random]);
    }

    protected void PlaySE(AudioClip _clip){
        theAudio.clip = _clip;
        theAudio.Play();
    }

    protected virtual void Update()
    {
        if(!isDead){
            Move();
            //Rotation();
            ElapseTime();
        }
    }
}
