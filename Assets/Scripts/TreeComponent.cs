using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeComponent : MonoBehaviour
{
    // 깎일 나무 조각들
    [SerializeField]
    private GameObject[] go_TreePieces;
    [SerializeField]
    private GameObject go_TreeCenter;

    // 통나무 프리팹
    [SerializeField]
    private GameObject go_Log_Prefabs;

    // 쓰러질 때 랜덤으로 가해질 힘의 세기
    [SerializeField]
    private float force;

    // 자식 트리
    [SerializeField]
    private GameObject go_ChildTree;

    // 부모 트리 쓰러질 때 콜라이더 제거용
    [SerializeField]
    private CapsuleCollider parentCol;

    // 자식 트리 쓰러질 때 필요한 컴포넌트 활성화 및 중력 활성화
    [SerializeField]
    private CapsuleCollider childCol;
    [SerializeField]
    private Rigidbody childRigid;

    // 파편 효과
    [SerializeField]
    private GameObject go_hit_effect;

    // 파편 제거 시간
    [SerializeField]
    private float debrisDestroyTime;

    // 나무 제거 시간
    [SerializeField]
    private float destroyTime;

    // 필요 사운드
    [SerializeField]
    private string chop_sound;
    [SerializeField]
    private string falldown_sound;
    [SerializeField]
    private string logChange_sound;

    // 적중 이펙트
    private void Hit(Vector3 _pos){
        SoundManager.instance.PlaySE(chop_sound);

        GameObject clone = Instantiate(go_hit_effect, _pos, Quaternion.Euler(Vector3.zero));
        Destroy(clone, debrisDestroyTime);
        
    }

    private void DestroyPiece(int _num){
        if(go_TreePieces[_num].gameObject != null){
            GameObject clone = Instantiate(go_hit_effect, go_TreePieces[_num].transform.position, Quaternion.Euler(Vector3.zero));
            Destroy(clone, debrisDestroyTime);
            Destroy(go_TreePieces[_num].gameObject);
        }
    }

    private void AngleCalc(float _angleY){
        Debug.Log(_angleY);
        if(0 <= _angleY && _angleY <= 70)
            DestroyPiece(2);
        else if(70 <= _angleY && _angleY <= 140)
            DestroyPiece(3);
        else if(140 <= _angleY && _angleY <= 210)
            DestroyPiece(4);
        else if(210 <= _angleY && _angleY <= 280)
            DestroyPiece(0);
        else if(280 <= _angleY && _angleY <= 360)
            DestroyPiece(1);
    }

    private bool CheckTreePieces(){
        for(int i=0; i<go_TreePieces.Length; i++){
            if(go_TreePieces[i].gameObject != null){
                return true;
            }
        }
        return false;
    }

    IEnumerator LogCoroutine(){
        yield return new WaitForSeconds(destroyTime);

        SoundManager.instance.PlaySE(logChange_sound);

        Instantiate(go_Log_Prefabs, go_ChildTree.transform.position + (go_ChildTree.transform.up * 3f), Quaternion.LookRotation(go_ChildTree.transform.up));
        Instantiate(go_Log_Prefabs, go_ChildTree.transform.position + (go_ChildTree.transform.up * 6f), Quaternion.LookRotation(go_ChildTree.transform.up));
        Instantiate(go_Log_Prefabs, go_ChildTree.transform.position + (go_ChildTree.transform.up * 9f), Quaternion.LookRotation(go_ChildTree.transform.up));

        Destroy(go_ChildTree.gameObject);
    }

    private void FallDownTree(){
        SoundManager.instance.PlaySE(falldown_sound);

        Destroy(go_TreeCenter);

        parentCol.enabled = false;
        childCol.enabled = true;
        childRigid.useGravity = true;

        childRigid.AddForce(Random.Range(-force, force), 0f, Random.Range(-force, force));

        StartCoroutine(LogCoroutine());
    }

    public void Chop(Vector3 _pos, float angleY){
        Hit(_pos);

        AngleCalc(angleY);

        if(!CheckTreePieces()){
            FallDownTree();
        }
    }

    public Vector3 GetTreeCenterPos(){
        return go_TreeCenter.transform.position;
    }
}
