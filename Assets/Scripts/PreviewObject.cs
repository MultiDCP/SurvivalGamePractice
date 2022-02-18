using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    public Building.Type needType;
    private bool needTypeFlag;

    // 충돌한 오브젝트의 콜라이더 저장
    private List<Collider> colliderList = new List<Collider>();

    [SerializeField]
    private int layerGround; // 지상 레이어
    private const int IGNORE_RAYCAST_LAYER = 2; // Ignore Raycast의 레이어 번호가 2임

    [SerializeField]
    private Material green;
    [SerializeField]
    private Material red;

    private void OnTriggerEnter(Collider other) {
        if(other.transform.tag == "Structure"){
            if(other.GetComponent<Building>().type != needType){
                colliderList.Add(other);
            }
            else{
                needTypeFlag = true;
            }
        }
        else{
            if(other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER)
                colliderList.Add(other);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.transform.tag == "Structure"){
            if(other.GetComponent<Building>().type != needType){
                colliderList.Remove(other);
            }
            else{
                needTypeFlag = false;
            }
        }
        else{
            if(other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER)
                colliderList.Remove(other);
        }
    }

    private void SetColor(Material mat){
        foreach(Transform tf_Child in this.transform){
            var newMaterials = new Material[tf_Child.GetComponent<Renderer>().materials.Length];

            for(int i=0; i<newMaterials.Length; i++){
                newMaterials[i] = mat;
            }

            tf_Child.GetComponent<Renderer>().materials = newMaterials;
        }
    }

    private void ChangeColor(){
        if(needType == Building.Type.Normal){
            if(colliderList.Count > 0){
                SetColor(red);
            }
            else {
                SetColor(green);
            }
        }
        else{
            if(colliderList.Count > 0 || !needTypeFlag){
                SetColor(red);
            }
            else{
                SetColor(green);
            }
        }
        
    }

    void Update() {
        ChangeColor();
    }

    public bool isBuildable(){
        if(needType == Building.Type.Normal){
            return colliderList.Count == 0;
        }
        else{
            return colliderList.Count == 0 && needTypeFlag;
        }
    }
}
