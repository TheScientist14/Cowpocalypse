using System;
using UnityEngine;

[Serializable]
public class Stat
{
    [field: SerializeField] public StatData StatData { get; set; }
    [field: SerializeField] public int CurrentLevel { get; set; } = 1;

    public float Value => StatData.BaseValue + StatData.MultiplierPerLevel * (CurrentLevel - 1);
}