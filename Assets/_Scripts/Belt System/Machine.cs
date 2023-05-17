using _Scripts.Pooling_System;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Machine : Belt
{
    [Expandable]
    [SerializeField] private ItemData CraftedItem;

    public Dictionary<ItemData, int> Stock { get; set; }
    public StockUpdateEvent stockUpdated { get; private set; } = new StockUpdateEvent();
    public class StockUpdateEvent : UnityEvent<Dictionary<ItemData, int>> { }
    private void Start()
    {
        Stock = new Dictionary<ItemData, int>();
        stockUpdated.Invoke(Stock);
        gameObject.name = $"Machine: {BeltID++}";

        SetCafteditem(CraftedItem);
    }

    public IEnumerator MoveQueuedItems(Item queuedItem)
    {
        print("StartBeltMove");

        Vector3 toPosition = transform.position;
        float step = BeltManager.instance.speed * Time.fixedDeltaTime;

        while (queuedItem.GetItem().transform.position != toPosition)
        {
            queuedItem.GetItem().transform.position = Vector3.MoveTowards(queuedItem.transform.position, toPosition, step);
            yield return null;
        }

        PoolManager.instance.DespawnObject(queuedItem);

        foreach (ItemData item in CraftedItem.Recipes.Keys)
        {
            if (Stock[item] < CraftedItem.Recipes[item])
                yield break;
        }

        foreach (ItemData item in CraftedItem.Recipes.Keys)
            Stock[item] -= CraftedItem.Recipes[item];
        stockUpdated.Invoke(Stock);
        yield return new WaitForSeconds(CraftedItem.CraftDuration);
        Item craftedItem = PoolManager.instance.SpawnObject(CraftedItem, transform.position);
        StartCoroutine(Output(craftedItem));
    }

    private IEnumerator Output(Item item)
    {
        if (BeltInSequence != null && BeltInSequence.isSpaceTaken == false)
        {
            if (BeltInSequence.GetComponent<Machine>())
            {
                Machine machine = BeltInSequence.GetComponent<Machine>();
                if (machine.GetCraftedItem() != null)
                    isMachineBlocking = !machine.GetCraftedItem().Recipes.ContainsKey(item.GetItemData());
                else
                    isMachineBlocking = true;
            }
            if (!isMachineBlocking)
            {
                Vector3 toPosition = BeltInSequence.transform.position;
                BeltInSequence.isSpaceTaken = true;
                float step = BeltManager.instance.speed * Time.fixedDeltaTime;

                while (item.GetItem().transform.position != toPosition)
                {
                    item.GetItem().transform.position = Vector3.MoveTowards(item.transform.position, toPosition, step);
                    yield return null;
                }

                isSpaceTaken = false;
                BeltInSequence.BeltItem = item;
            }
        }
    }

    public void SetCafteditem(ItemData craftedItemData)
    {
        CraftedItem = craftedItemData;
        Stock.Clear();
        stockUpdated.Invoke(Stock);
        if (CraftedItem != null)
            foreach (ItemData item in CraftedItem.Recipes.Keys)
                Stock.Add(item, 0);
    }

    public ItemData GetCraftedItem()
    {
        return CraftedItem;
    }

    public void AddToStock(Item item)
    {
        Stock.TryGetValue(item.GetItemData(), out int amount);
        Stock[item.GetItemData()] = Stock[item.GetItemData()] + 1;
        print("AddToStock : " + Stock[item.GetItemData()] + " of type : " + item.GetItemData().name);
        stockUpdated.Invoke(Stock);
        StartCoroutine(MoveQueuedItems(item));
    }
}
