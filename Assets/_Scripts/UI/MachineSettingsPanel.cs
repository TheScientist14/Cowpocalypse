using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        //Clear in case we try to access machine from closed settings
        if(!show)
            _machine = null;
    }
    public Machine OpenedMachine
    {
        get => _machine;
        set
        {
            if(_machine != null)
                _machine.stockUpdated.RemoveListener(UpdateStock);
            value.stockUpdated.AddListener(UpdateStock);
            _machine = value;
            if(_machine == null)
                return;
            SetItemData(_machine.GetCraftedItem());
            UpdateStock(_machine.Stock);
        }
    }

    private void UpdateStock(Dictionary<ItemData, int> stock)
    {
        foreach(var kvp in stock)
        {
            if (_itemToRessource.ContainsKey(kvp.Key))
            {
              _itemToRessource[kvp.Key].UpdateValue(kvp.Value, null);  
            }
        }
    }

    public void SetItemData(ItemData item)
    {
        _machine.SetCrafteditem(item);
        _recipeRessourceUI.ItemData = item;
        //Instead of displaying the cost we display one, being the nb of items being produced
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
                child.UpdateValue(_machine.Stock[recipe.Key], recipe.Value);
                i++;
            }
        }

        for(; i < ch; i++)
        {
            _resourcesToCraft[i].gameObject.SetActive(false);
        }
    }
}
