using System.Collections.Generic;
using UnityEngine;

public class MachinePanel : BasicPanel
{
	[SerializeField] RessourceUI m_CraftedItemRessourceUI;
	[SerializeField] List<RessourceUI> m_NeededItemRessourcesUI;

	private Dictionary<ItemData, RessourceUI> m_ItemToRessource = new Dictionary<ItemData, RessourceUI>();
	private Machine m_Machine;

	protected override void OnOpen()
	{
		base.OnOpen();
		InputStateMachine.instance.SetState(new MachineSettingsState());

		if(m_Machine == null)
			Close();
	}

	protected override void OnClose()
	{
		base.OnClose();
		InputStateMachine.instance.SetState(new FreeViewState());
		m_Machine = null;
	}

	public void SetMachine(Machine iMachine)
	{
		if(m_Machine != null)
			m_Machine.OnStockUpdated.RemoveListener(UpdateStock);

		m_Machine = iMachine;
		if(m_Machine == null)
			return;

		m_Machine.OnStockUpdated.AddListener(UpdateStock);
		UpdateItemData();
		UpdateStock();
	}

	public void SetItemData(ItemData iItemData)
	{
		m_Machine.SetCraftedItem(iItemData);
		UpdateItemData();
	}

	private void UpdateStock()
	{
		if(m_Machine == null)
			return;

		ItemData craftedItemData = m_Machine.GetCraftedItemData();
		if(craftedItemData == null)
			return;

		foreach(KeyValuePair<ItemData, int> itemRecipeNeeded in craftedItemData.Recipes)
		{
			if(!m_ItemToRessource.ContainsKey(itemRecipeNeeded.Key))
				continue;

			m_ItemToRessource[itemRecipeNeeded.Key].UpdateValue(m_Machine.GetItemCountInStock(itemRecipeNeeded.Key), itemRecipeNeeded.Value);
		}
	}

	private void UpdateItemData()
	{
		ItemData craftedItem = m_Machine.GetCraftedItemData();

		m_CraftedItemRessourceUI.SetItemData(craftedItem);
		m_ItemToRessource = new Dictionary<ItemData, RessourceUI>();
		var ressourceUiTotalCount = m_NeededItemRessourcesUI.Count;
		int ressourceUiIdx = 0;
		if(craftedItem != null)
		{
			// erratum : trying to display nothing
			// instead of displaying the cost we display one, being the nb of items being produced
			// m_CraftedItemRessourceUI.UpdateValue(iMax: 1);
			foreach(var recipe in craftedItem.Recipes)
			{
				RessourceUI neededItemRessourceUi = m_NeededItemRessourcesUI[ressourceUiIdx];
				m_ItemToRessource.Add(recipe.Key, neededItemRessourceUi);
				neededItemRessourceUi.gameObject.SetActive(true);
				neededItemRessourceUi.SetItemData(recipe.Key);
				neededItemRessourceUi.UpdateValue(m_Machine.GetItemCountInStock(recipe.Key), recipe.Value);
				ressourceUiIdx++;
			}
		}

		for(; ressourceUiIdx < ressourceUiTotalCount; ressourceUiIdx++)
			m_NeededItemRessourcesUI[ressourceUiIdx].gameObject.SetActive(false);
	}
}
