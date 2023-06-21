using _Scripts.Pooling_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSpawner : UpGiver
{
    [SerializeField] ItemData m_SpawnedItemData;

    protected new void Start()
    {
        base.Start();

        StartCoroutine(Produce());
    }

    public override bool CanBeOverriden()
    {
        return false;
    }

    public override bool CanReceive(IItemHandler iGiver, Item iItem)
    {
        return false;
    }

    IEnumerator Produce()
    {
        while(true)
        {
            if(m_HandledItem == null)
            {
                yield return new WaitForSeconds(ItemHandlerManager.instance.GetSpawnRate());
                m_HandledItem = PoolManager.instance.SpawnObject(m_SpawnedItemData, transform.position);
                m_IsItemFullyReceived = (m_HandledItem != null);
            }
            else
                yield return new WaitForFixedUpdate();
        }
    }
}
