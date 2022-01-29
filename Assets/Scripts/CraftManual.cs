using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Craft{
    public string craftName; // 이름
    public GameObject go_Prefab; // 실제 설치될 프리팹
    public GameObject go_PreviewPrefab; // 미리보기 프리팹
}

public class CraftManual : MonoBehaviour
{
    // 상태 변수
    private bool isActivated = false;
    private bool isPreviewActivated = false;

    [SerializeField]
    private GameObject go_BaseUI; // 기본 베이스 UI

    [SerializeField]
    private GameObject[] go_Slot; // 탭과 연결된 슬롯의 게임오브젝트(0-불, 1-트랩)

    [SerializeField]
    private Craft[] craft_fire; // 모닥불용 탭
    [SerializeField]
    private Craft[] craft_trap; // 덫 전용 탭

    private GameObject go_Preview; // 미리보기 프리팹 담을 변수
    private GameObject go_Prefab; // 실제 생성될 프리팹 담을 변수

    [SerializeField]
    private Transform tf_Player;

    private RaycastHit hitInfo;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float range;

    private void TapClick(int _tapIndex){
        for(int i=0; i<go_Slot.Length; i++){
            if(i == _tapIndex){
                go_Slot[i].SetActive(true);
                continue;
            }
            go_Slot[i].SetActive(false);
        }
    }

    public void FireTabClick(){
        TapClick(0);
    }

    public void TrapTabClick(){
        TapClick(1);
    }

    private void SlotClick(int _craftIndex, int _slotNumber){
        switch(_craftIndex){
            case 0:
                go_Preview = Instantiate(craft_fire[_slotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
                go_Prefab = craft_fire[_slotNumber].go_Prefab;
                break;
            case 1:
                go_Preview = Instantiate(craft_trap[_slotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
                go_Prefab = craft_trap[_slotNumber].go_Prefab;
                break;
        }

        isPreviewActivated = true;
        GameManager.isOpenCraftManual = false;
        go_BaseUI.SetActive(false);
    }

    public void FireSlotClick(int _slotNumber){
        SlotClick(0, _slotNumber);
    }

    public void TrapSlotClick(int _slotNumber){
        SlotClick(1, _slotNumber);
    }

    private void OpenWindow(){
        isActivated = true;
        GameManager.isOpenCraftManual = true;
        go_BaseUI.SetActive(true);
    }

    private void CloseWindow(){
        isActivated = false;
        GameManager.isOpenCraftManual = false;
        go_BaseUI.SetActive(false);
    }

    private void Window(){
        if(!isActivated){
            OpenWindow();
        }
        else{
            CloseWindow();
        }
    }

    private void Cancel(){
        if(isPreviewActivated){
            Destroy(go_Preview);
        }

        isActivated = false;
        isPreviewActivated = false;
        go_Preview = null;
        go_Prefab = null;

        go_BaseUI.SetActive(false);
    }

    private void PreviewPositionUpdate(){
        if(Physics.Raycast(tf_Player.position, tf_Player.forward, out hitInfo, range, layerMask)){
            if(hitInfo.transform != null){
                Vector3 _location = hitInfo.point;
                go_Preview.transform.position = _location;
            }
        }
    }

    private void Build(){
        if(isPreviewActivated && go_Preview.GetComponent<PreviewObject>().isBuildable()){
            Instantiate(go_Prefab, hitInfo.point, Quaternion.identity);
            Destroy(go_Preview);
            isActivated = false;
            isPreviewActivated = false;
            go_Preview = null;
            go_Prefab = null;
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated){
            Window();
        }

        if(isPreviewActivated){
            PreviewPositionUpdate();
        }
        
        if(Input.GetButtonDown("Fire1")){
            Build();
        }

        if(Input.GetKeyDown(KeyCode.Escape)){
            Cancel();
        }
    }
}
