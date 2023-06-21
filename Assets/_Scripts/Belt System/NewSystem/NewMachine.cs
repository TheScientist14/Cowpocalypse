using _Scripts.Pooling_System;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class NewMachine : UpGiver
{
    [Expandable]
    [SerializeField] ItemData m_CraftedItemData;
    private bool m_IsTryingToCraft = false;
    private bool m_IsCrafting = false;
    private Item m_CraftedItem;

    public Dictionary<ItemData, int> Stock { get; set; }
    public StockUpdateEvent OnStockUpdated;
    private HashSet<Item> m_NotFullyReceivedItems;
    public class StockUpdateEvent : UnityEvent<Dictionary<ItemData, int>> { }

    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();

        m_NotFullyReceivedItems = new HashSet<Item>();

        if(OnStockUpdated == null)
            OnStockUpdated = new StockUpdateEvent();

        Stock = new Dictionary<ItemData, int>();
        OnStockUpdated.Invoke(Stock);
    }

    public ItemData GetCraftedItemData()
    {
        return m_CraftedItemData;
    }

    private bool HasEnoughRessourcesToCraft()
    {
        if(m_CraftedItemData == null)
            return false;

        foreach(KeyValuePair<ItemData, int> requiredItemCount in m_CraftedItemData.Recipes)
        {
            if(Stock.GetValueOrDefault(requiredItemCount.Key, 0) < requiredItemCount.Value)
                return false;
        }

        return true;
    }

    private bool CanCraft()
    {
        if(m_IsCrafting || m_CraftedItem != null)
            return false;

        // this condition will change if we decide to support stock being
        // bigger than what is needed to craft
        if(m_NotFullyReceivedItems.Count > 0)
            return false;

        if(!HasEnoughRessourcesToCraft())
            return false;

        return true;
    }

    private bool CraftItem()
    {
        if(!CanCraft())
            return false;

        m_IsCrafting = true;
        ResetStock();
        StartCoroutine(_CraftItemAfterCooldown());

        return true;
    }

    private IEnumerator _CraftItemAfterCooldown()
    {
        yield return new WaitForSeconds(m_CraftedItemData.CraftDuration * ItemHandlerManager.instance.GetCraftingSpeedMultiplier());

        // if crafted item data changed while crafting, we reset it
        if(!m_IsCrafting)
            yield return new WaitForEndOfFrame();

        m_CraftedItem = PoolManager.instance.SpawnObject(m_CraftedItemData, transform.position);
        m_IsCrafting = false;
    }

    private IEnumerator RepeatTryCraftUntilSuccess()
    {
        m_IsTryingToCraft = true;
        while(!CraftItem())
            yield return new WaitForSecondsRealtime(.1f);
        m_IsTryingToCraft = false;
    }

    public void SetCrafteditem(ItemData iCraftedItemData)
    {
        m_CraftedItemData = iCraftedItemData;
        ResetStock();
        ClearReceivingItems();
        m_IsCrafting = false;
    }

    private void ResetStock()
    {
        Stock.Clear();
        if(m_CraftedItemData != null)
            foreach(ItemData item in m_CraftedItemData.Recipes.Keys)
                Stock.Add(item, 0);

        OnStockUpdated.Invoke(Stock);
    }

    private void ClearReceivingItems()
    {
        foreach(Item item in m_NotFullyReceivedItems)
        {
            if(item == null)
                continue;
            PoolManager.instance.DespawnObject(item);
        }
        m_NotFullyReceivedItems.Clear();
    }

    protected override ref Item GetItemToSend()
    {
        if(m_IsCrafting)
        {
            m_NullItem = null;
            return ref m_NullItem;
        }

        return ref m_CraftedItem;
    }

    public override bool CanReceive(IItemHandler iGiver, Item iItem)
    {
        if(iItem == null || iGiver == null)
            return false;
        ItemData itemData = iItem.GetItemData();
        if(m_CraftedItemData == null || itemData == null)
            return false;

        return Stock.ContainsKey(itemData) && Stock[itemData] < m_CraftedItemData.Recipes[itemData];
    }

    public override bool Receive(IItemHandler iGiver, Item iItem)
    {
        bool received = base.Receive(iGiver, iItem);
        if(received)
        {
            Stock[iItem.GetItemData()] = Stock.GetValueOrDefault(iItem.GetItemData(), 0) + 1;
            m_NotFullyReceivedItems.Add(iItem);
        }
        return received;
    }

    protected override IEnumerator MoveReceivedItem(Item iItem)
    {
        yield return StartCoroutine(base.MoveReceivedItem(iItem));
        PoolManager.instance.DespawnObject(iItem);
        m_NotFullyReceivedItems.Remove(iItem);
        if(HasEnoughRessourcesToCraft() && !m_IsTryingToCraft)
            StartCoroutine(RepeatTryCraftUntilSuccess());
    }

    protected new void OnDestroy()
    {
        base.OnDestroy();

        ClearReceivingItems();

        if(m_CraftedItem != null)
            PoolManager.instance.DespawnObject(m_CraftedItem);

        ItemHandlerManager.instance.RemoveOneMachine();
    }
}
