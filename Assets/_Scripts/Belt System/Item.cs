using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : MonoBehaviour
{
    [SerializeField] private GameObject item;
    [SerializeField] private ItemData itemData;

    private void Awake()
    {
        item = gameObject;
    }

    public ItemData GetItemData()
    {
        return itemData;
    }

    public GameObject GetItem()
    {
        return item;
    }

    public void SetItemData(ItemData data)
    {
        itemData = data;
    }

    public void SetItem(GameObject setItem)
    {
        item = setItem;
    }
}
