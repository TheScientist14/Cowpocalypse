using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AllTiersPanel : Panel
{
    [SerializeField]
    private LayoutGroup _layout;

    private void Start()
    {
        InstantiateTiers();
    }

    [SerializeField, Header("Editor helpers")]
    private TierUI _tierPrefab;
    [Button("Instantiate tiers")]
    private void InstantiateTiers()
    {
        var SOs = ItemCreator.LoadAllResourceAtPath<ItemData>();
        IEnumerable<IGrouping<ItemTier, ItemData>> s = SOs.GroupBy(x => x.Tier).OrderBy(t => t.Key.Level).Where(t => t.Key.Level > 1);
        var ch = _layout.transform.childCount;
        for(int i = 0; i < ch; i++)
        {
            Destroy(_layout.transform.GetChild(0).gameObject);
        }
        foreach(IGrouping<ItemTier, ItemData> tier in s)
        {
            var inst = Instantiate(_tierPrefab, _layout.transform);
            inst.TierName.Value = tier.Key.Name;
            inst.InstantiateRecipes(tier);
        }
    }
}