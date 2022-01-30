using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject go_BaseUI;
    [SerializeField] private SaveLoad theSaveLoad;

    private void CallMenu(){
        GameManager.isPause = true;
        go_BaseUI.SetActive(true);
        Time.timeScale = 0f;
    }

    private void CloseMenu(){
        GameManager.isPause = false;
        go_BaseUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ClickSave(){
        Debug.Log("세이브");
        theSaveLoad.SaveData();
    }

    public void ClickLoad(){
        Debug.Log("로드");
        theSaveLoad.LoadData();
    }

    public void ClickExit(){
        Debug.Log("게임 종료");
        Application.Quit();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.P)){
            if(!GameManager.isPause){
                CallMenu();
            }
            else{
                CloseMenu();
            }
        }
    }
}
