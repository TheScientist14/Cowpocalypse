using System;
using UnityEngine;

[Serializable]
public class Stat
{
	[SerializeField] int _currentLevel = 1;

	public StatData StatData
	{
		get; set;
	}

	public int CurrentLevel
	{
		get => _currentLevel;
		set
		{
			if(value <= 0 || value > StatData.MaxLevelInclusive)
				return;

			_currentLevel = value;
		}
	}

	public bool IsMaxedOut()
	{
		return CurrentLevel >= StatData.MaxLevelInclusive;
	}

	public float Value => StatData.BaseValue * Mathf.Pow(StatData.MultiplierPerLevel, (CurrentLevel - 1));
}