using _Scripts.Pooling_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class NewSeller : SimpleItemHandler
{
    // Update is called once per frame
    protected void Update()
    {
        if(m_HandledItem != null && m_IsItemFullyReceived)
        {
            bool success = PoolManager.instance.DespawnObject(m_HandledItem);
            Assert.IsTrue(success);
            if(!success)
                return;

            Wallet.instance.Money += m_HandledItem.GetItemData().Price;
            m_HandledItem = null;
            m_IsItemFullyReceived = false;
        }
    }
}
