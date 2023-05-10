using UnityEngine;
using UnityEngine.UI;

public class MachineSettingsPanel : Panel
{
    [SerializeField]
    private RessourceUI _recipeRessourceUI;
    [SerializeField]
    private RessourceUI[] _resourcesToCraft;
    private Machine _machine;

    public Machine OpenedMachine
    {
        get => _machine;
        set
        {
            _machine = value;
            SetItemData(_machine.GetCraftedItem());
        }
    }

    public void SetItemData(ItemData item)
    {
        _machine.SetCafteditem(item);
        _recipeRessourceUI.ItemData = item;
        //Instead of displaying the cost we display one, being the nb of items being produced
        _recipeRessourceUI.Number = 1;
        var ch = _resourcesToCraft.Length;
        int i = 0;
        foreach (var recipe in item.Recipes)
        {
            var child = _resourcesToCraft[i];
            child.gameObject.SetActive(true);
            child.ItemData = recipe.Key;
            child.Number = recipe.Value;
            i++;
        }
        for (; i < ch; i++)
        {
            _resourcesToCraft[i].gameObject.SetActive(false);
        }
    }
}
