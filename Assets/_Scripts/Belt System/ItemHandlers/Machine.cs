using _Scripts.Pooling_System;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Machine : UpGiver
{
    [Expandable]
    [SerializeField] ItemData m_CraftedItemData;
    private bool m_IsTryingToCraft = false;
    private bool m_IsCrafting = false;
    private float m_StartCraftTime = 0;
    private Item m_CraftedItem;

    private Dictionary<ItemData, int> m_Stock = new Dictionary<ItemData, int>();

    public UnityEvent OnStockUpdated;

    private HashSet<Item> m_NotFullyReceivedItems = new HashSet<Item>();

    private static int s_MachineCount = 0;

    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();

        Wallet.instance.Money -= ItemHandlerManager.instance.GetMachinePrice();
        s_MachineCount++;

        if(OnStockUpdated == null)
            OnStockUpdated = new UnityEvent();
        OnStockUpdated.Invoke();
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
            if(m_Stock.GetValueOrDefault(requiredItemCount.Key, 0) < requiredItemCount.Value)
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
        StartCoroutine(_CraftItemAfterCooldown(_GetCraftLength()));

        return true;
    }

    private float _GetCraftLength()
    {
        return m_CraftedItemData.CraftDuration * ItemHandlerManager.instance.GetCraftingSpeedMultiplier();
    }

    private IEnumerator _CraftItemAfterCooldown(float iCooldown)
    {
        yield return new WaitForSeconds(iCooldown);

        // if crafted item data changed while crafting, we reset it
        if(!m_IsCrafting)
            yield break;

        m_CraftedItem = PoolManager.instance.SpawnObject(m_CraftedItemData, transform.position);
        m_IsItemFullyReceived = true;
        m_IsCrafting = false;
    }

    private IEnumerator RepeatTryCraftUntilSuccess()
    {
        m_IsTryingToCraft = true;
        while(!CraftItem())
            yield return new WaitForSecondsRealtime(.1f);
        m_IsTryingToCraft = false;
    }

    public void SetCraftedItem(ItemData iCraftedItemData)
    {
        if(m_CraftedItemData == iCraftedItemData)
            return;

        m_CraftedItemData = iCraftedItemData;
        ResetStock();
        ClearReceivingItems();
        m_IsCrafting = false;
    }

    private void ResetStock()
    {
        m_Stock.Clear();
        if(m_CraftedItemData != null)
            foreach(ItemData item in m_CraftedItemData.Recipes.Keys)
                m_Stock.Add(item, 0);

        OnStockUpdated?.Invoke();
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

        return m_Stock.ContainsKey(itemData) && m_Stock[itemData] < m_CraftedItemData.Recipes[itemData];
    }

    public override bool Receive(IItemHandler iGiver, Item iItem)
    {
        bool received = base.Receive(iGiver, iItem);
        if(received)
        {
            m_Stock[iItem.GetItemData()] = m_Stock.GetValueOrDefault(iItem.GetItemData(), 0) + 1;
            m_NotFullyReceivedItems.Add(iItem);
            OnStockUpdated?.Invoke();
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

    public override bool CanBePlacedOn(TerrainType iTerrainType)
    {
        return base.CanBePlacedOn(iTerrainType) && Wallet.instance.Money >= ItemHandlerManager.instance.GetMachinePrice();
    }

    protected new void OnDestroy()
    {
        s_MachineCount--;
        base.OnDestroy();

        ClearReceivingItems();

        if(m_CraftedItem != null && PoolManager.instance != null)
            PoolManager.instance.DespawnObject(m_CraftedItem);

        if(ItemHandlerManager.instance == null || Wallet.instance == null)
            return;

        Wallet.instance.Money += ItemHandlerManager.instance.GetMachinePrice();
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
            m_NotFullyReceivedItems.Add(item);
            MoveReceivedItem(item);
        }
    }

    public float GetTimeLeftForCurrentCraft()
    {
        if(!m_IsCrafting)
            return -1;

        return Mathf.Clamp(m_StartCraftTime + _GetCraftLength() - Time.time, 0, _GetCraftLength());
    }

    public void SetTimeLeftForCurrentCraft(float iTimeLeft)
    {
        if(Time.time < 0)
            return;

        _CraftItemAfterCooldown(iTimeLeft);
    }

    public IEnumerable<KeyValuePair<ItemData, int>> GetCurrentStock()
    {
        return m_Stock;
    }

    public int GetItemCountInStock(ItemData iItemData)
    {
        return m_Stock.GetValueOrDefault(iItemData, 0);
    }

    public void SetCurrentStock(IEnumerable<KeyValuePair<ItemData, int>> iStock)
    {
        m_Stock = new Dictionary<ItemData, int>(iStock);
        OnStockUpdated?.Invoke();
    }

    public Item GetCraftedItem()
    {
        return m_CraftedItem;
    }

    public void SetAlreadyCraftedItem(Item iCraftedItem)
    {
        m_CraftedItem = iCraftedItem;
    }

    public static int GetCount()
    {
        return s_MachineCount;
    }
}
