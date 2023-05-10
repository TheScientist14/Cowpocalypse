using System;
using UnityEngine;

public class Wallet : MonoBehaviour
{
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