using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AllTierUi : MonoBehaviour
{
    [SerializeField]
    private LayoutGroup _layout;
    [SerializeField]
    private TierUI _tierPrefab;

    private void Start()
    {
        InstantiateTiers();
    }
    [Button("Instantiate tiers")]
    private void InstantiateTiers()
    {
        var SOs = ItemCreator.LoadAllItemsAtPath<ItemData>("Assets/Scriptable objects/Items/");
        IEnumerable<IGrouping<ItemTier, ItemData>> s = SOs.GroupBy(x => x.Tier).OrderBy(t=>t.Key.Level);
        foreach (IGrouping<ItemTier, ItemData> tier in s)
        {
            var inst = Instantiate(_tierPrefab, _layout.transform);
            inst.TierName.Value = tier.Key.Name;
            inst.InstantiateRecipes(tier);
        }
    }
}
