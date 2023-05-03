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
        gameObject.name = $"Machine: {BeltID++}";
        CurrentCraftedItem = CraftedItem;
    }

    public override IEnumerator StartBeltMove()
    {
        yield return new WaitForSeconds(CraftedItem.CraftDuration);
    }
}
