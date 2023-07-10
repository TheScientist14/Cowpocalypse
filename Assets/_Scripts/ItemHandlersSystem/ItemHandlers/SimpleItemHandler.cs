using _Scripts.Pooling_System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class SimpleItemHandler : IItemHandler
{
	protected Item m_HandledItem;
	protected bool m_IsItemFullyReceived = false;

	public UnityEvent OnItemMove;

	protected virtual void Awake()
	{
		if(OnItemMove == null)
			OnItemMove = new UnityEvent();
	}

	public Item GetCurrentItem()
	{
		return m_HandledItem;
	}

	public virtual void SetItemInTransfer(Item iItem)
	{
		m_HandledItem = iItem;
		StartCoroutine(MoveReceivedItem(iItem));
	}

	protected bool HasFullyReceivedItem()
	{
		return m_HandledItem != null && m_IsItemFullyReceived;
	}

	public override bool CanBeOverriden()
	{
		return true;
	}

	public override bool CanGive(IItemHandler iReceiver)
	{
		return false;
	}

	public override bool CanReceive(IItemHandler iGiver, Item iItem)
	{
		return iGiver != null && iItem != null && iGiver.CanGive(this) && m_HandledItem == null;
	}

	public override bool Receive(IItemHandler iGiver, Item iItem)
	{
		bool canReceive = CanReceive(iGiver, iItem);
		Assert.IsTrue(canReceive);
		if(!canReceive)
			return false;

		m_HandledItem = iItem;
		m_IsItemFullyReceived = false;

		StartCoroutine(MoveReceivedItem(iItem));

		return true;
	}

	protected virtual IEnumerator MoveReceivedItem(Item iItem)
	{
		if(iItem == null)
			yield break;

		Transform itemTransform = iItem.transform;
		while(Vector3.SqrMagnitude(itemTransform.position - transform.position) > .001)
		{
			itemTransform.position = Vector3.MoveTowards(
				itemTransform.position,
				transform.position,
				ItemHandlerManager.instance.GetBeltSpeed() * Time.fixedDeltaTime);
			yield return new WaitForFixedUpdate();
		}
		m_IsItemFullyReceived = true;
	}

	protected bool TrySendItemTo(ref Item ioItem, IItemHandler iReceiver)
	{
		if(iReceiver != null && CanGive(iReceiver) && iReceiver.CanReceive(this, ioItem))
		{
			bool success = iReceiver.Receive(this, ioItem);
			Assert.IsTrue(success);
			if(success)
			{
				ioItem = null;
				OnItemMove.Invoke();
				return true;
			}
		}
		return false;
	}

	public override bool CanBePlacedOn(TerrainType iTerrainType)
	{
		return iTerrainType != TerrainType.Border && iTerrainType != TerrainType.Water && iTerrainType != TerrainType.Oil;
	}

	protected virtual void OnDestroy()
	{
		if(m_HandledItem != null && PoolManager.instance != null)
			PoolManager.instance.DespawnObject(m_HandledItem);
	}
}
