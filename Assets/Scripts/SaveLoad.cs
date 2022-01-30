using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData{
    public Vector3 playerPos; // 캐릭터의 위치
    public Vector3 playerRotation; // 캐릭터가 바라보는 방향

    public List<int> invenArrayNum = new List<int>();
    public List<string> invenItemName = new List<string>();
    public List<int> invenItemNum = new List<int>();
}

public class SaveLoad : MonoBehaviour
{
    private SaveData saveData = new SaveData();

    private string SAVE_DATA_DIRECTORY;
    private string SAVE_FILENAME = "/SaveFile.txt";

    private PlayerController thePlayer;
    private Inventory theInven;

    private void Start() {
        SAVE_DATA_DIRECTORY = Application.dataPath + "/Saves/";

        if(!Directory.Exists(SAVE_DATA_DIRECTORY)){
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY);
        }
    }

    public void SaveData(){
        thePlayer = FindObjectOfType<PlayerController>();
        theInven = FindObjectOfType<Inventory>();

        saveData.playerPos = thePlayer.transform.position;
        saveData.playerRotation = thePlayer.transform.eulerAngles;

        Slot[] slots = theInven.GetSlots();
        for(int i=0; i<slots.Length; i++){
            if(slots[i].item != null){
                saveData.invenArrayNum.Add(i);
                saveData.invenItemName.Add(slots[i].item.itemName);
                saveData.invenItemNum.Add(slots[i].itemCount);
            }
        }

        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);

        Debug.Log("저장 완료");
        Debug.Log(json);
    }

    public void LoadData(){
        if(File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME)){
            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            thePlayer = FindObjectOfType<PlayerController>();
            theInven = FindObjectOfType<Inventory>();

            thePlayer.transform.position = saveData.playerPos;
            thePlayer.transform.eulerAngles = saveData.playerRotation;

            for(int i=0; i<saveData.invenItemName.Count; i++){
                theInven.LoadToInven(saveData.invenArrayNum[i], saveData.invenItemName[i], saveData.invenItemNum[i]);
            }

            Debug.Log("로드 완료");
        }
        else{
            Debug.Log("세이브 파일이 없습니다.");
        }
    }
}
