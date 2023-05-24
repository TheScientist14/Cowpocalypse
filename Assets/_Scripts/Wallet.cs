using System;
using UnityEngine;
using NaughtyAttributes;

public class Wallet : Singleton<Wallet>
{
    [ReadOnly]
    [SerializeField]
    int _money;
        
    public int Money
    {
        get => _money;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException($"{nameof(Money)} cannot be negative.");

            _money = value;
            MoneyChanged?.Invoke(_money);
        }
    }

    public Action<int> MoneyChanged;

    [ContextMenu("Add 100")]
    public void Add()
    {
        Money += 100;
    }
}