using _Scripts.Pooling_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class NewSeller : SimpleItemHandler
{
    private HashSet<Item> m_NotFullyReceivedItems;

    protected void Start()
    {
        m_NotFullyReceivedItems = new HashSet<Item>();
    }

    public override bool CanReceive(IItemHandler iGiver, Item iItem)
    {
        return true;
    }

    protected override IEnumerator MoveReceivedItem(Item iItem)
    {
        m_NotFullyReceivedItems.Add(iItem);
        yield return StartCoroutine(base.MoveReceivedItem(iItem));

        Wallet.instance.Money += iItem.GetItemData().Price;
        bool success = PoolManager.instance.DespawnObject(iItem);
        Assert.IsTrue(success);
        if(success)
            m_NotFullyReceivedItems.Remove(iItem);
    }

    protected new void OnDestroy()
    {
        base.OnDestroy();

        foreach(Item item in m_NotFullyReceivedItems)
            PoolManager.instance.DespawnObject(item);

        ItemHandlerManager.instance.RemoveOneShop();
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
}