using _Scripts.Pooling_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class Seller : SimpleItemHandler
{
    private HashSet<Item> m_NotFullyReceivedItems = new HashSet<Item>();

    public UnityEvent OnSell;

    private static int s_SellerCount = 0;

    protected override void Awake()
    {
        base.Awake();

        if(OnSell == null)
            OnSell = new UnityEvent();
    }

    protected virtual void Start()
    {
        s_SellerCount++;
    }

    public override bool CanReceive(IItemHandler iGiver, Item iItem)
    {
        return true;
    }

    public override bool CanBePlacedOn(TerrainType iTerrainType)
    {
        return base.CanBePlacedOn(iTerrainType) && s_SellerCount < ItemHandlerManager.instance.GetMaxNbShop();
    }

    protected override IEnumerator MoveReceivedItem(Item iItem)
    {
        m_NotFullyReceivedItems.Add(iItem);
        yield return StartCoroutine(base.MoveReceivedItem(iItem));

        Wallet.instance.Money += iItem.GetItemData().Price;
        OnSell.Invoke();
        bool success = PoolManager.instance.DespawnObject(iItem);
        Assert.IsTrue(success);
        if(success)
            m_NotFullyReceivedItems.Remove(iItem);
    }

    protected override void OnDestroy()
    {
        s_SellerCount--;
        base.OnDestroy();

        foreach(Item item in m_NotFullyReceivedItems)
        {
            if(item != null && PoolManager.instance != null)
                PoolManager.instance.DespawnObject(item);
        }
    }

    public IEnumerable<Item> GetItemsInTransfer()
    {
        return m_NotFullyReceivedItems;
    }

    public void AddItemsInTransfer(IEnumerable<Item> iItemsInTransfer)
    {
        foreach(Item item in iItemsInTransfer)
        {
            if(m_NotFullyReceivedItems.Contains(item))
                continue;
            MoveReceivedItem(item);
        }
    }

    public static int GetCount()
    {
        return s_SellerCount;
    }
}