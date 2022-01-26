using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemEffect{
    public string itemName; // 아이템의 이름(키값)
    [Tooltip("HP, SP, DP, HUNGRY, THIRSTY, SATISFY만 가능합니다.")]
    public string[] part; // 부위
    public int[] num; // 수치

}
public class ItemEffectDatabase : MonoBehaviour
{
    [SerializeField]
    private ItemEffect[] itemEffects;

    [SerializeField]
    private StatusController thePlayerStatus;
    [SerializeField]
    private WeaponManager theWeaponManager;
    [SerializeField]
    private SlotToolTip theSlotToolTip;

    private const string HP = "HP", SP = "SP", DP = "DP", HUNGRY = "HUNGRY", THIRSTY = "THIRSTY", SATISFY = "SATISFY";

    public void ShowToolTip(Item _item, Vector3 _pos){
        theSlotToolTip.ShowToolTip(_item, _pos);
    }

    public void HideToolTip(){
        theSlotToolTip.HideToolTip();
    }

    public void UseItem(Item _item){
        if(_item.itemType == Item.ItemType.Equipment){ // 장착
                    StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(_item.weaponType, _item.itemName));
        }

        else if(_item.itemType == Item.ItemType.Used){
            for(int i=0; i<itemEffects.Length; i++){
                if(itemEffects[i].itemName == _item.itemName){
                    for(int j=0; j<itemEffects.Length; j++){
                        switch(itemEffects[i].part[j]){
                            case HP:
                                thePlayerStatus.IncreaseHP(itemEffects[i].num[j]);
                                break;
                            case SP:
                                thePlayerStatus.IncreaseSP(itemEffects[i].num[j]);
                                break;
                            case DP:
                                thePlayerStatus.IncreaseDP(itemEffects[i].num[j]);
                                break;
                            case HUNGRY:
                                thePlayerStatus.IncreaseHungry(itemEffects[i].num[j]);
                                break;
                            case THIRSTY:
                                thePlayerStatus.IncreaseThirsty(itemEffects[i].num[j]);
                                break;
                            case SATISFY:
                                thePlayerStatus.IncreaseSatisfy(itemEffects[i].num[j]);
                                break;
                            default:
                                Debug.Log("잘못된 Status 부위. HP, SP, DP, HUNGRY, THIRSTY, SATISFY만 가능합니다.");
                                break;
                        }
                        Debug.Log(_item.itemName + " 을 사용했습니다.");
                    }
                    return;
                }
            }
            Debug.Log("ItemEffectDatabase에 일치하는 itemName이 없습니다.");
        }
    }
}
