using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMachineSettingsPanel : Panel
{
    [SerializeField]
    private RessourceUI _recipeRessourceUI;
    [SerializeField]
    private RessourceUI[] _resourcesToCraft;
    private Dictionary<ItemData, RessourceUI> _itemToRessource = new Dictionary<ItemData, RessourceUI>();
    private NewMachine _machine;

    public override void ChangeVisibility(bool show, float delay = 0, float? durationOverride = null)
    {
        base.ChangeVisibility(show, delay, durationOverride);
        // clear in case we try to access machine from closed settings
        if(!show)
            _machine = null;
    }

    public NewMachine OpenedMachine
    {
        get => _machine;
        set
        {
            if(_machine != null)
                _machine.OnStockUpdated.RemoveListener(UpdateStock);
            value.OnStockUpdated.AddListener(UpdateStock);
            _machine = value;
            if(_machine == null)
                return;
            Dictionary<ItemData, int> stock = new Dictionary<ItemData, int>(_machine.GetCurrentStock());
            SetItemData(_machine.GetCraftedItemData(), stock);
            UpdateStock(stock);
        }
    }

    private void UpdateStock(Dictionary<ItemData, int> stock)
    {
        foreach(var kvp in stock)
        {
            if(!_itemToRessource.ContainsKey(kvp.Key))
                continue;

            _itemToRessource[kvp.Key].UpdateValue(kvp.Value, null);
        }
    }

    public void SetItemData(ItemData item, Dictionary<ItemData, int> iMachineStock)
    {
        _machine.SetCraftedItem(item);
        _recipeRessourceUI.ItemData = item;
        // instead of displaying the cost we display one, being the nb of items being produced
        _itemToRessource = new Dictionary<ItemData, RessourceUI>();
        var ch = _resourcesToCraft.Length;
        int i = 0;
        if(item != null)
        {
            _recipeRessourceUI.UpdateValue(max: 1);
            foreach(var recipe in item.Recipes)
            {
                RessourceUI child = _resourcesToCraft[i];
                _itemToRessource.Add(recipe.Key, child);
                child.gameObject.SetActive(true);
                child.ItemData = recipe.Key;
                child.UpdateValue(iMachineStock[recipe.Key], recipe.Value);
                i++;
            }
        }

        for(; i < ch; i++)
            _resourcesToCraft[i].gameObject.SetActive(false);
    }
}
