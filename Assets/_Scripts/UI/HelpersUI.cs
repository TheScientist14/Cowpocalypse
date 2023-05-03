using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TierUI : MonoBehaviour
{
    [SerializeField]
    private TextUI _tierName;
    [SerializeField]
    private LayoutGroup _layout;
    [SerializeField]
    private GameObject _prefab;

    public TextUI TierName { get => _tierName; }
    public void InstantiateRecipes(IEnumerable<ItemData> itemsInTier)
    {
        //To-Do Fetch resources of the tier
        foreach (ItemData item in itemsInTier)
        {
            var obj=Instantiate(_prefab, _layout.transform);
            //PoolManager.instance.SpawnObject(item);
            obj.name = item.name;
        }
    }
}
