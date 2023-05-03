using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StatManager : MonoBehaviour
{
    [SerializeField] List<StatData> _statsData;

    public List<Stat> Stats { get; private set; }

    void Awake()
    {
        Stats = _statsData.Select(statData => new Stat { StatData = statData }).ToList();
    }
}