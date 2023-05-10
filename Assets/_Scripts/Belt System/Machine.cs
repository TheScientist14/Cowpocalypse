using _Scripts.Pooling_System;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : Belt
{
    [SerializeField] private GameObject ItemObject;
    [Expandable]
    [SerializeField] private ItemData CraftedItem;

    public Dictionary<ItemData, int> Stock { get; private set; }


    private void Start()
    {
        Stock = new Dictionary<ItemData, int>();
        gameObject.name = $"Machine: {BeltID++}";

        SetCafteditem(CraftedItem);
    }

    public override IEnumerator StartBeltMove()
    {
        Stock.TryGetValue(BeltItem.GetItemData(), out int amount);
        Stock[BeltItem.GetItemData()] = Stock[BeltItem.GetItemData()] + 1;
        print(amount);
        PoolManager.instance.DespawnObject(BeltItem);
        isSpaceTaken = false;
        foreach (ItemData item in CraftedItem.Recipes.Keys)
        {
            if (Stock[item] < CraftedItem.Recipes[item])
            {
                yield break;
            }
        }
        foreach (ItemData item in CraftedItem.Recipes.Keys)
        {
            Stock[item] -= CraftedItem.Recipes[item];
        }
        yield return new WaitForSeconds(CraftedItem.CraftDuration);
        Item craftedItem = PoolManager.instance.SpawnObject(CraftedItem, transform.position);
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
}
