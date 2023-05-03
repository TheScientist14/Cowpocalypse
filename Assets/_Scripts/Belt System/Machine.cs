using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : Belt
{
    [SerializeField] private ItemData CraftedItem;

    private ItemData CurrentCraftedItem;
    public Dictionary<ItemData, int> Stocks { get; private set; }

    // Start is called before the first frame update

    private void Start()
    {
        Stock = new Dictionary<ItemData, int>();
        gameObject.name = $"Machine: {BeltID++}";

        SetCafteditem(CraftedItem);

    }

    public override IEnumerator StartBeltMove()
    {
        
        yield return new WaitForSeconds(CraftedItem.CraftDuration);
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
        yield return new WaitForSeconds(CraftedItem.CraftDuration);

    }
}
