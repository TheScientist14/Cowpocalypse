using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;

public class StatManager : Singleton<StatManager>
{
    [Expandable]
    [SerializeField] List<StatData> _statsData;

    public List<Stat> Stats { get; private set; }

    void Awake()
    {
        Stats = _statsData.Select(statData => new Stat { StatData = statData }).ToList();
    }

    [ContextMenu("Log stat values")][Button("Log Values")]
    public void LogValues()
    {
        foreach (var stat in Stats)
        {
            Debug.Log($"{stat.StatData.Name} : {stat.CurrentLevel}, {stat.Value}");
        }
    }


    [Button("Add Values")]
    private void AddLevelToStat()
    {
        Stats[0].CurrentLevel++;
    }
    
}