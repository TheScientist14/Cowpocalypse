using NaughtyAttributes;
using System;
using UnityEngine;

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
			if(value < 0)
			{
				Debug.LogError($"{nameof(Money)} cannot be negative.");
				return;
			}

			_money = value;
			MoneyChanged?.Invoke(_money);
		}
	}

	public Action<int> MoneyChanged;

	[ContextMenu("Add 10000")]
	[Button("Add 10000")]
	public void Add()
	{
		Money += 10000;
	}
}