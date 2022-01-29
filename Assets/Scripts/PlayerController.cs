using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 스피드 조정 변수
    [SerializeField]
    private float walkSpeed; // 걷기 스피드
    [SerializeField]
    private float runSpeed; // 뛰기 스피드
    [SerializeField]
    private float crouchSpeed; // 앉기 스피드
    private float applySpeed; // 함수 하나에서 스피드 변수를 컨트롤하기 위한 변수

    [SerializeField]
    private float jumpForce;

    // 상태 변수
    private bool isWalk = false;
    private bool isRun = false;
    private bool isCrouch = false; 
    private bool isGround = true;

    // 앉기 상태에 얼마나 앉을지 결정
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    private CapsuleCollider capsuleCollider;

    // 카메라 변수
    [SerializeField]
    private float lookSensitivity; // 카메라 감도
    [SerializeField]
    private float cameraRotationLimit; // 각도 제한
    private float currentCameraRotationX = 0f;

    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRigid;
    private GunController theGunController;
    private Crosshair theCrosshair;
    private StatusController theStatusController;

    // 움직임 체크 변수
    private Vector3 lastPos;

    // 지면 체크
    private void IsGround(){
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
        //0.1f의 경우 경사/계단과 같은 곳에서의 오차를 감안한 여유 거리
        theCrosshair.JumpingAnimation(!isGround);
    }

    // 점프
    private void Jump(){
        // 앉은 상태에서 점프 시도 시 앉기 해제
        if(isCrouch){
            Crouch();
        }
        theStatusController.DecreaseStamina(100);
        myRigid.velocity = transform.up * jumpForce;
    }

    // 키 입력 시 점프 시도
    private void TryJump(){
        if(Input.GetKeyDown(KeyCode.Space) && isGround && theStatusController.GetCurrentSP() > 0){
            Jump();
        }
    }

    // 달리기
    private void Running(){
        if(isCrouch){
            Crouch();
        }

        theGunController.CancelFineSight();

        isRun = true;
        theCrosshair.RunningAnimation(isRun);
        theStatusController.DecreaseStamina(10);
        applySpeed = runSpeed;
    }

    // 달리기 중단
    private void RunningCancel(){
        isRun = false;
        theCrosshair.RunningAnimation(isRun);
        applySpeed = walkSpeed;
    }

    // 키 입력 시 달리기 시도
    private void TryRun(){
        if(Input.GetKey(KeyCode.LeftShift) && theStatusController.GetCurrentSP() > 0){
            Running();
        }
        if(Input.GetKeyUp(KeyCode.LeftShift) || theStatusController.GetCurrentSP() <= 0){
            RunningCancel();
        }
    }

    // 부드러운 앉기 동작
    IEnumerator CrouchCoroutine(){
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;

        while(_posY != applyCrouchPosY){
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f);
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            if (count > 15){
                break;
            }
            yield return null;
        }
        theCamera.transform.localPosition = new Vector3(0f, applyCrouchPosY, 0f);
    }

    // 앉기
    private void Crouch(){
        isCrouch = !isCrouch;
        theCrosshair.CrouchAnimation(isCrouch);

        if(isCrouch){
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        //theCamera.transform.localPosition = new Vector3(theCamera.transform.localPosition.x, applyCrouchPosY, theCamera.transform.localPosition.z);
        StartCoroutine(CrouchCoroutine());
    }

    // 앉기 시도
    private void TryCrouch(){
        if(Input.GetKeyDown(KeyCode.LeftControl)){
            Crouch();
        }
    }

    // 이동
    private float Move(){
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");
        float moveXZAbsSum = Mathf.Abs(_moveDirX) + Mathf.Abs(_moveDirZ);
        // 움직임이 있다면 그 값을 저장. 움직임이 있음에도 0이 되는 것을 막기 위해 절대값 이용.

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;
        /* Vector 값이 1이 나오도록 정규화시켜줄 경우 유니티 자체의 계산도 편리하지만
           개발자 입장에서도 1초에 얼마만큼 이동시킬 것인지 계산하기 편리해짐 */

        myRigid.MovePosition(transform.position + (_velocity * Time.deltaTime));

        return moveXZAbsSum;
    }

    private void MoveCheck(float MoveXZ){
        if(!isRun && !isCrouch && isGround){
            //if(Vector3.Distance(lastPos, transform.position) >= 0.01f)
            // Vector3.Distance가 가끔씩 0이 뜰 때가 있어 Walk 상탱임에도 Idle로 판단하는 경우 발생
            if(MoveXZ != 0){ // 움직임이 있다면 0이 아님
                isWalk = true;
            }
            else {
                isWalk = false;
            }

            theCrosshair.WalkingAnimation(isWalk);
            lastPos = transform.position;
        }
    }

    // 카메라 상하 회전
    private void CameraRotation(){
        float _xRotation = Input.GetAxisRaw("Mouse Y"); // 카메라의 X Rotation은 위아래 방향임
        float _cameraRotationX = _xRotation * lookSensitivity;

        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    // 캐릭터 좌우 회전
    private void CharacterRotation(){
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        theStatusController = GetComponent<StatusController>();
        theGunController = FindObjectOfType<GunController>();
        theCrosshair = FindObjectOfType<Crosshair>();

        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }

    void Update()
    {
        if(GameManager.canPlayerMove){
            IsGround();
            TryJump();
            TryRun();
            TryCrouch();
            float CheckMoveXZ = Move();
            MoveCheck(CheckMoveXZ);
            CameraRotation();
            CharacterRotation();
        }
    }
}
