using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : Belt
{
    [SerializeField] private GameObject ItemObject;
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
        Destroy(BeltItem);
        isSpaceTaken = false;
        foreach (ItemData item in CraftedItem.Recipes.Keys)
        {
            if (Stock[item] < CraftedItem.Recipes[item])
            {
                StopCoroutine(StartBeltMove());
            }
        }
        foreach (ItemData item in CraftedItem.Recipes.Keys)
        {
            Stock[item] -= CraftedItem.Recipes[item];
        }
        yield return new WaitForSeconds(CraftedItem.CraftDuration);
        GameObject craftedObject = Instantiate(ItemObject);
        Item craftedItem = craftedObject.GetComponent<Item>();
        craftedItem.SetItemData(CraftedItem);
    }

    public void SetCafteditem(ItemData craftedItemData)
    {
        CraftedItem = craftedItemData;
        Stock.Clear();
        foreach(ItemData item in CraftedItem.Recipes.Keys)
            Stock.Add(item, 0);
    }

    public ItemData GetCraftedItem()
    {
        return CraftedItem;
    }
}
