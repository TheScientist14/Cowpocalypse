using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineSettingsPanel : Panel
{
    [SerializeField]
    private RessourceUI _recipeRessourceUI;
    [SerializeField]
    private RessourceUI[] _resourcesToCraft;
    private Dictionary<ItemData, RessourceUI> _itemToRessource = new Dictionary<ItemData, RessourceUI>();
    private Machine _machine;

    public override void ChangeVisibility(bool show, float delay = 0, float? durationOverride = null)
    {
        base.ChangeVisibility(show, delay, durationOverride);
        // clear in case we try to access machine from closed settings
        if(!show)
            _machine = null;
    }

    public Machine OpenedMachine
    {
        get => _machine;
        set
        {
            if(_machine != null)
                _machine.OnStockUpdated.RemoveListener(UpdateStock);
            _machine = value;
            if(_machine == null)
                return;
            _machine.OnStockUpdated.AddListener(UpdateStock);
            SetItemData(_machine.GetCraftedItemData());
            UpdateStock();
        }
    }

    private void UpdateStock()
    {
        if(OpenedMachine == null)
            return;

        ItemData craftedItemData = OpenedMachine.GetCraftedItemData();
        if(craftedItemData == null)
            return;

        foreach(KeyValuePair<ItemData, int> itemRecipeNeeded in craftedItemData.Recipes)
        {
            if(!_itemToRessource.ContainsKey(itemRecipeNeeded.Key))
                continue;

            _itemToRessource[itemRecipeNeeded.Key].UpdateValue(OpenedMachine.GetItemCountInStock(itemRecipeNeeded.Key), itemRecipeNeeded.Value);
        }
    }

    public void SetItemData(ItemData iItem)
    {
        _machine.SetCraftedItem(iItem);
        _recipeRessourceUI.ItemData = iItem;
        // instead of displaying the cost we display one, being the nb of items being produced
        _itemToRessource = new Dictionary<ItemData, RessourceUI>();
        var ch = _resourcesToCraft.Length;
        int i = 0;
        if(iItem != null)
        {
            _recipeRessourceUI.UpdateValue(max: 1);
            foreach(var recipe in iItem.Recipes)
            {
                RessourceUI child = _resourcesToCraft[i];
                _itemToRessource.Add(recipe.Key, child);
                child.gameObject.SetActive(true);
                child.ItemData = recipe.Key;
                child.UpdateValue(OpenedMachine.GetItemCountInStock(recipe.Key), recipe.Value);
                i++;
            }
        }

        for(; i < ch; i++)
            _resourcesToCraft[i].gameObject.SetActive(false);
    }
}
