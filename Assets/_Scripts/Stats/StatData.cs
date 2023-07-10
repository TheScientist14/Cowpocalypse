using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StatData")]
public class StatData : ScriptableObject
{
	public string Name;
	public float MultiplierPerLevel;
	public float BaseValue;
	public int MaxLevelInclusive;
	public Sprite Icon;
	public List<int> Prices;
}