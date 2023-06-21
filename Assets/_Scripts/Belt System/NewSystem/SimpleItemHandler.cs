using _Scripts.Pooling_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SimpleItemHandler : IItemHandler
{
    protected Item m_HandledItem;
    protected bool m_IsItemFullyReceived = false;

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
            yield return new WaitForEndOfFrame();

        Transform itemTransform = iItem.transform;
        while(Vector3.SqrMagnitude(itemTransform.position - transform.position) > .001)
        {
            itemTransform.position = Vector3.MoveTowards(itemTransform.position, transform.position, ItemHandlerManager.instance.speed * Time.fixedDeltaTime);
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
                return true;
            }
        }
        return false;
    }

    protected void OnDestroy()
    {
        if(m_HandledItem != null)
            PoolManager.instance.DespawnObject(m_HandledItem);
    }
}
