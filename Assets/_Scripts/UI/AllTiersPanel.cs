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
        IEnumerable<IGrouping<ItemTier, ItemData>> s = SOs.GroupBy(x => x.Tier).OrderBy(t => t.Key.Level);
        var ch=_layout.transform.childCount;
        for (int i = 0; i < ch; i++)
        {
            DestroyImmediate(_layout.transform.GetChild(0).gameObject);
        }
        foreach (IGrouping<ItemTier, ItemData> tier in s)
        {
            var inst = PrefabUtility.InstantiatePrefab(_tierPrefab, _layout.transform).GetComponent<TierUI>();
            inst.TierName.Value = tier.Key.Name;
            inst.InstantiateRecipes(tier);
        }
    }
}
