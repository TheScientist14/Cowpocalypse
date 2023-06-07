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
    public void ChangeTiersDisplayed(int firstTiersToHide, int lastTiersToHide = 100000)
    {
        var tr = _layout.transform;
        int childCount = tr.childCount;
        firstTiersToHide = Mathf.Min(firstTiersToHide, childCount);
        lastTiersToHide = Mathf.Min(lastTiersToHide, childCount);
        int i = 0;
        for (; i < firstTiersToHide; i++)
        {
            tr.GetChild(i).gameObject.SetActive(false);
        }
        for (; i < lastTiersToHide; i++)
        {
            tr.GetChild(i).gameObject.SetActive(true);
        }
        for (; i < childCount; i++)
        {
            tr.GetChild(i).gameObject.SetActive(false);
        }
    }
    
    [SerializeField, Header("Editor helpers")]
    private TierUI _tierPrefab;
    [Button("Instantiate tiers")]
    private void InstantiateTiers()
    {
        var SOs = ItemCreator.LoadAllResourceAtPath<ItemData>();
        IEnumerable<IGrouping<ItemTier, ItemData>> s = SOs.GroupBy(x => x.Tier).OrderBy(t => t.Key.Level);
        var ch = _layout.transform.childCount;
        for (int i = 0; i < ch; i++)
        {
            Destroy(_layout.transform.GetChild(0).gameObject);
        }
        foreach (IGrouping<ItemTier, ItemData> tier in s)
        {
            var inst = Instantiate(_tierPrefab, _layout.transform);
            inst.TierName.Value = tier.Key.Name;
            inst.InstantiateRecipes(tier);
        }
    }
}