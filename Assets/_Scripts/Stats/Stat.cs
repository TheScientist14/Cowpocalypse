using System;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] int _currentLevel = 1;
    
    [field: SerializeField] public StatData StatData { get; set; }

    public int CurrentLevel
    {
        get => _currentLevel;
        set
        {
            if (value <= 0 || value > StatData.MaxLevelInclusive)
                return;

            _currentLevel = value;
            
            if (StatData.Name == "Belt speed")
            {
                Shader.SetGlobalFloat("_speed", Value);
            }
        }
    }

    public bool IsMaxedOut => CurrentLevel == StatData.MaxLevelInclusive;
    
    public float Value => StatData.BaseValue + StatData.MultiplierPerLevel * (CurrentLevel - 1);
}