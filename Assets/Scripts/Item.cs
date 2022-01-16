using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject
{
    public string itemName; // 이름
    public ItemType itemType; // 유형
    public Sprite itemImage; // 이미지
    public GameObject itemPrefab; // 프리팹

    public string weaponType; // 무기 유형

    public enum ItemType{
        Equipment,
        Used,
        Ingredient,
        ETC
    }
}
