using _Scripts.Pooling_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Seller : Belt
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = $"Seller: {BeltID++}";
    }

    // Update is called once per frame
    void Update()
    {
        if(BeltItem == null)
            return;

        isSpaceTaken = false;
        Assert.IsTrue(PoolManager.instance.DespawnObject(BeltItem));
        Wallet.instance.Money += BeltItem.GetItemData().Price;
        BeltItem = null;
    }
}
