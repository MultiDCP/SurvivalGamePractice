using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FieldOfViewAngle : MonoBehaviour
{
    [SerializeField]
    private float viewAngle; // 시야각(120도)
    [SerializeField]
    private float viewDistance; // 시야 거리(10m 등)
    [SerializeField]
    private LayerMask targetMask; // 타겟 마스크(플레이어)

    private PlayerController thePlayer;
    private NavMeshAgent nav;

    void Start(){
        thePlayer = FindObjectOfType<PlayerController>();
        nav = GetComponent<NavMeshAgent>();
    }

    public Vector3 GetTargetPos(){
        return thePlayer.transform.position;
    }
    /*
    private Vector3 BoundaryAngle(float _angle){
        _angle += transform.eulerAngles.y; // 자기 자신의 오일러 angle의 y값을 더해줌
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
        /* 
           반지름이 1인 원이 있다 가정하였을 때
           자기 자신(0, 0)에서 Ray를 쏘기 위해서는 나아가야 할 방향의 좌표가 필요할 것
           원과 해당 직선이 만나는 정점을 P(x, z)라고 가정한다.
           x축으로 수선의 발을 내려 생기는 직각삼각형의 밑변을 w, 높이를 h라고 하자.
           이 직각삼각형의 빗변과 x축이 만드는 각도를 세타라고 했을 때,
           당연하게 P의 좌표는 (cos 세타, sin 세타)가 됐을 것이다.
           하지만 우리가 필요한 것은 해당 직각삼각형의 세타값을 각도로 가지는 좌표가 아니라
           viewAngle(여기선 _angle), 즉 (90' - 세타) 값이다. 이는 직각삼각형의 빗변과 z축이 만드는 각도와 같다.
           이를 바탕으로 계산해야 우리가 원하는 라디안 값과 좌표를 얻을 수 있으므로,
           P의 좌표는 sin(_angle), cos(_angle)이 될 것이다.
           다만 angle은 지금 라디안 값이 아니라 float 값의 각도이므로
           Mathf.Sin과 MathF.Cos 메소드로 계산하려면 이를 라디안값으로 변환을 해주어야 한다.
           따라서 이를 모두 변환한 뒤 계산하면 우리가 원하는 Vector3 좌표를 얻을 수 있게 된다.
        
    }*/

    private float CalcPathLength(Vector3 _targetPos){
        NavMeshPath _path = new NavMeshPath();
        nav.CalculatePath(_targetPos, _path);

        Vector3[] _wayPoint = new Vector3[_path.corners.Length + 2]; // 자기 자신과 플레이어의 위치를 기억시키기 위해 2 더함

        _wayPoint[0] = transform.position;
        _wayPoint[_path.corners.Length + 1] = _targetPos;

        float _pathLength = 0;
        for(int i=0; i<_path.corners.Length; i++){
            _wayPoint[i+1] = _path.corners[i]; // wayPoint에 경로를 넣음. i+1인 이유는 0은 자기 자신이기 때문
            _pathLength += Vector3.Distance(_wayPoint[i], _wayPoint[i+1]); // 경로 길이 계산
        }

        return _pathLength;
    }

    public bool View(){
        /*
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f); // 왼쪽으로 치우쳐야 해서 마이너스
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f);

        Debug.DrawRay(transform.position + transform.up, _leftBoundary * 10, Color.red);
        Debug.DrawRay(transform.position + transform.up, _rightBoundary * 10, Color.red);
        */
        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        for(int i=0; i<_target.Length; i++){
            Transform _targetTf = _target[i].transform;
            if(_targetTf.name == "Player"){ // 눈 앞에 타겟이 플레이어일 경우
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward);

                if(_angle < viewAngle * 0.5f){
                    RaycastHit _hit;
                    if(Physics.Raycast(transform.position + transform.up, _direction, out _hit, viewDistance)){
                        if(_hit.transform.name == "Player"){
                            //Debug.Log("플레이어가 돼지 시야 내에 있습니다.");
                            Debug.DrawRay(transform.position + transform.up, _direction, Color.blue);
                            
                            return true;
                        }
                    }
                }
            }

            if(thePlayer.GetRun()){
                if(CalcPathLength(thePlayer.transform.position) <= viewDistance){
                    Debug.Log("돼지가 주변에서 뛰고 있는 플레이어의 움직임을 파악했습니다.");
                    return true;
                }
            }
        }
        return false;
    }
}
