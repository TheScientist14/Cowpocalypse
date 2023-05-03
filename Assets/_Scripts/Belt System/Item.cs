using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
