using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound{
    public string name; // 곡의 이름
    public AudioClip clip; // 곡
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    #region singleton
    void Awake() {
        if(instance == null){
            instance = this;
        }
        else if(instance != this){
            Destroy(this);
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
    }
    #endregion

    public AudioSource[] audioSourceEffects;
    public AudioSource audioSourceBGM;

    public string[] playSoundName;

    public Sound[] effectSounds;
    public Sound[] bgmSounds;

    public void PlaySE(string _name){
        for(int i=0; i<effectSounds.Length; i++){
            if(_name == effectSounds[i].name){
                for(int j=0; j<audioSourceEffects.Length; j++){
                    if(!audioSourceEffects[j].isPlaying){
                        playSoundName[j] = effectSounds[i].name;
                        audioSourceEffects[j].clip = effectSounds[i].clip;
                        audioSourceEffects[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 가용 AudioSource가 사용 중입니다.");
                return;
            }
        }
        Debug.Log(_name + " 사운드가 SoundManager에 등록되지 않았습니다.");
    }

    public void StopAllSE(){
        for(int i=0; i<audioSourceEffects.Length; i++){
            audioSourceEffects[i].Stop();
        }
    }

    public void StopSE(string _name){
        for(int i=0; i<audioSourceEffects.Length; i++){
            if(playSoundName[i] == _name){
                audioSourceEffects[i].Stop();
                return;
            }
        }
        Debug.Log("재생 중인 " + _name + "사운드가 없습니다.");
    }

    void Start() {
        playSoundName = new string[audioSourceEffects.Length];
    }
}
