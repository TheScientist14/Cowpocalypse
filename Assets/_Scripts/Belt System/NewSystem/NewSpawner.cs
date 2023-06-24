using _Scripts.Pooling_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class NewSpawner : UpGiver
{
    [SerializeField] ItemData m_SpawnedItemData;
    private float m_LastSpawnTime = 0;

    private Coroutine m_ProduceCoroutine = null;

    protected new void Start()
    {
        base.Start();

        m_ProduceCoroutine = StartCoroutine(Produce(-1));
    }

    public override bool CanBeOverriden()
    {
        return false;
    }

    public override bool CanReceive(IItemHandler iGiver, Item iItem)
    {
        return false;
    }

    private void _Spawn()
    {
        m_HandledItem = PoolManager.instance.SpawnObject(m_SpawnedItemData, transform.position);
        m_IsItemFullyReceived = (m_HandledItem != null);
        m_LastSpawnTime = Time.time;
    }

    IEnumerator Produce(float iInitTime)
    {
        if(iInitTime >= 0)
        {
            yield return new WaitForSeconds(iInitTime);
            _Spawn();
        }
        while(true)
        {
            if(m_HandledItem == null)
            {
                yield return new WaitForSeconds(ItemHandlerManager.instance.GetSpawnRate());
                _Spawn();
            }
            else
                yield return new WaitForFixedUpdate();
        }
    }

    public void InitSpawnTime(float iTimeForNextSpawn)
    {
        StopCoroutine(m_ProduceCoroutine);
        m_LastSpawnTime = Time.time - (ItemHandlerManager.instance.GetSpawnRate() - iTimeForNextSpawn);
        Assert.IsNull(m_HandledItem);
        m_ProduceCoroutine = StartCoroutine(Produce(iTimeForNextSpawn));
    }

    public ItemData GetItemDataToSpawn()
    {
        return m_SpawnedItemData;
    }

    public float GetTimeForNextSpawn()
    {
        if(m_HandledItem != null)
            return -1; // no next spawn until the previous is sent

        return Mathf.Clamp(m_LastSpawnTime + ItemHandlerManager.instance.GetSpawnRate() - Time.time,
            0, ItemHandlerManager.instance.GetSpawnRate());
    }
}
