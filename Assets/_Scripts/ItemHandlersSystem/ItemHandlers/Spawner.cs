using _Scripts.Pooling_System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class Spawner : UpGiver
{
    [Serializable]
    struct TerrainTypeRessource
    {
        public TerrainType TerrainType;
        public ItemData Ressource;
    }

    [SerializeField] List<TerrainTypeRessource> m_TerrainTypeRessources = new List<TerrainTypeRessource>();

    private ItemData m_SpawnedItemData = null;
    private float m_LastSpawnTime = 0;

    private Coroutine m_ProduceCoroutine = null;

    public UnityEvent OnSpawn;

    private static int s_SpawnerCount = 0;

    protected override void Awake()
    {
        base.Awake();

        if(OnSpawn == null)
            OnSpawn = new UnityEvent();
    }

    protected override void Start()
    {
        base.Start();

        StartCoroutine(DelayMoneyDecreaseOnStart());

        TerrainType terrainType = MapGenerator.instance.GetTileType(transform.position);
        m_SpawnedItemData = _GetTerrainRessource(terrainType);

        if(m_SpawnedItemData != null && m_ProduceCoroutine == null)
            m_ProduceCoroutine = StartCoroutine(Produce(-1));
    }

    IEnumerator DelayMoneyDecreaseOnStart()
    {
        yield return new WaitForEndOfFrame();
        if(Wallet.instance != null && ItemHandlerManager.instance != null)
            Wallet.instance.Money -= ItemHandlerManager.instance.GetSpawnerPrice();
        s_SpawnerCount++;
        // dirty fix to update price ui
        GridManager.instance.OnGridChanged.Invoke();
    }

    private ItemData _GetTerrainRessource(TerrainType iTerrainType)
    {
        foreach(TerrainTypeRessource terrainTypeRessource in m_TerrainTypeRessources)
        {
            if(terrainTypeRessource.TerrainType == iTerrainType)
                return terrainTypeRessource.Ressource;
        }
        return null;
    }

    public override bool CanReceive(IItemHandler iGiver, Item iItem)
    {
        return false;
    }

    public override bool CanBePlacedOn(TerrainType iTerrainType)
    {
        return (iTerrainType == TerrainType.Oil
            || iTerrainType == TerrainType.CoalOre
            || iTerrainType == TerrainType.CopperOre
            || iTerrainType == TerrainType.SulfurOre
            || iTerrainType == TerrainType.IronOre
            || iTerrainType == TerrainType.Water)
            && Wallet.instance.Money >= ItemHandlerManager.instance.GetSpawnerPrice();
    }

    private void _Spawn()
    {
        m_HandledItem = PoolManager.instance.SpawnObject(m_SpawnedItemData, transform.position);
        m_IsItemFullyReceived = (m_HandledItem != null);
        m_LastSpawnTime = Time.time;
        OnSpawn.Invoke();
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
        if(m_ProduceCoroutine != null)
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

    protected override void OnDestroy()
    {
        s_SpawnerCount--;
        base.OnDestroy();

        if(Wallet.instance != null && ItemHandlerManager.instance != null)
            Wallet.instance.Money += ItemHandlerManager.instance.GetSpawnerPrice();
    }

    public static int GetCount()
    {
        return s_SpawnerCount;
    }
}
