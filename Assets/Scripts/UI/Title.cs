using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public string sceneName = "Game";

    public static Title instance;

    private SaveLoad theSaveLoad;

    private void Awake() {

        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }   
        else{
            Destroy(this.gameObject);
        } 
    }

    public void ClickStart(){
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator LoadCoroutine(){
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while(!operation.isDone){
            yield return null;
        }

        theSaveLoad = FindObjectOfType<SaveLoad>();
        theSaveLoad.LoadData();

        Destroy(gameObject);
    }

    public void ClickLoad(){
        Debug.Log("로드");

        StartCoroutine(LoadCoroutine());
    }

    public void ClickExit(){
        Debug.Log("게임 종료");
        Application.Quit();
    }
}
