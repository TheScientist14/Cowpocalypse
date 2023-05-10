using _Scripts.Pooling_System;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : Belt
{
    [Expandable]
    [SerializeField] private ItemData CraftedItem;

    public Dictionary<ItemData, int> Stock { get; private set; }


    private void Start()
    {
        Stock = new Dictionary<ItemData, int>();
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
        if(CraftedItem != null)
            foreach(ItemData item in CraftedItem.Recipes.Keys)
                Stock.Add(item, 0);
    }

    public ItemData GetCraftedItem()
    {
        return CraftedItem;
    }

    public void AddToStock(Item item)
    {
        print("AddToStock");
        Stock.TryGetValue(item.GetItemData(), out int amount);
        Stock[item.GetItemData()] = Stock[item.GetItemData()] + 1;
        StartCoroutine(MoveQueuedItems(item));
    }
}
