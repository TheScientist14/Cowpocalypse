using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item")]
public class ItemData : ScriptableObject, ISerializationCallbackReceiver
{
    [field: SerializeField] public int Price { get; set; }
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public float CraftDuration { get; set; }
    [field: SerializeField] public ItemTier Tier { get; set; }
    
    [SerializeField] List<ItemData> _parents = new List<ItemData>();
    [SerializeField] List<int> _amounts = new List<int>();
    
    public Dictionary<ItemData, int> Recipes { get; private set; }

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        if (_parents != null && _amounts != null && _parents.Count == _amounts.Count)
        {
            Recipes = new Dictionary<ItemData, int>();

            for (int i = 0; i < _parents.Count; i++)
            {
                Recipes.Add(_parents[i], _amounts[i]);
            }
        }
    }

    public void AddParent(ItemData parent)
    {
        _parents.Add(parent);
    }

    public void AddAmount(int amount)
    {
        _amounts.Add(amount);
    }
}