using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StatData")]
public class StatData : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public float MultiplierPerLevel { get; private set; }
    [field: SerializeField] public float BaseValue { get; private set; }
    [field: SerializeField] public int MaxLevelInclusive { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public List<int> Prices { get; private set; }
}