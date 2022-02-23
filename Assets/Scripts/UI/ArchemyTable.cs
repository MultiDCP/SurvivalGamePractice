using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArchemyItem{
    public string itemName;
    public string itemDesc;
    public Sprite itemImage;

    public GameObject go_ItemPrefab;
}

public class ArchemyTable : MonoBehaviour
{
    private bool isOpen = false;

    [SerializeField] private ArchemyItem[] archemyItems; // 제작할 수 있는 연금 아이템 리스트

    [SerializeField] private Transform tf_BaseUI; // 베이스 UI
    [SerializeField] private Transform tf_PotionAppearPos; // 포션 나올 위치

    private void OpenWindow(){
        isOpen = true;
        GameManager.isOpenArchemyTable = true;
        tf_BaseUI.localScale = new Vector3(1f, 1f, 1f);
    }

    private void CloseWindow(){
        isOpen = false;
        GameManager.isOpenArchemyTable = false;
        tf_BaseUI.localScale = new Vector3(0f, 0f, 0f); // 백그라운드에서 포션 제작이 될 수 있도록 스케일만 줄임
    }

    public void Window(){
        isOpen = !isOpen;
        if(isOpen){
            OpenWindow();
        }
        else
            CloseWindow();
    }

    private void ProductionComplete(int _buttonNum){
        Instantiate(archemyItems[_buttonNum].go_ItemPrefab, tf_PotionAppearPos.position, Quaternion.identity);
    }

    public void ButtonClick(int _buttonNum){
        ProductionComplete(_buttonNum);
    }

    private void Update() {
        if(isOpen){
            if(Input.GetKeyDown(KeyCode.Escape))
                CloseWindow();
        }
    }
}
