using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AllTiersPanelBuilder : MonoBehaviour
{
    [SerializeField] MachinePanel m_MachinePanel;

    [SerializeField] LayoutGroup m_Layout;
    [SerializeField] TierUI m_TierUiPrefab;

    void Start()
    {
        InstantiateTiers();
    }

    private void InstantiateTiers()
    {
        int childCount = m_Layout.transform.childCount;
        for(int i = 0; i < childCount; i++)
            Destroy(m_Layout.transform.GetChild(0).gameObject);

        IEnumerable<ItemData> itemsData = ItemCreator.LoadAllResourceAtPath<ItemData>();
        IEnumerable<IGrouping<ItemTier, ItemData>> craftableItemsByTiers =
            itemsData.GroupBy(item => item.Tier).OrderBy(t => t.Key.Level).Where(t => t.Key.Level > 1);

        foreach(IGrouping<ItemTier, ItemData> itemsInTier in craftableItemsByTiers)
        {
            TierUI inst = Instantiate(m_TierUiPrefab, m_Layout.transform);
            inst.SetTierName(itemsInTier.Key.Name);
            inst.InstantiateRecipes(itemsInTier, m_MachinePanel);
        }
    }
}