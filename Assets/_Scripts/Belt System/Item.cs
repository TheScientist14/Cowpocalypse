using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public GameObject item;
    public ItemData itemData;

    private void Awake()
    {
        item = gameObject;
    }
}
