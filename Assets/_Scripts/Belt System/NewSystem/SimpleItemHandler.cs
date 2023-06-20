using _Scripts.Pooling_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SimpleItemHandler : MonoBehaviour, IItemHandler
{
    protected Item m_HandledItem;
    protected bool m_IsItemFullyReceived = false;

    public virtual bool CanGive()
    {
        return m_HandledItem != null && m_IsItemFullyReceived;
    }

    public virtual bool CanReceive(IItemHandler iGiver, Item iItem)
    {
        return iGiver != null && iItem != null && iGiver.CanGive() && m_HandledItem == null;
    }

    public virtual bool Receive(IItemHandler iGiver, Item iItem)
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
            yield return null;

        Transform itemTransform = iItem.transform;
        while(Vector3.SqrMagnitude(itemTransform.position - transform.position) > .001)
        {
            itemTransform.position = Vector3.MoveTowards(itemTransform.position, transform.position, BeltManager.instance.speed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
        m_IsItemFullyReceived = true;
    }

    protected bool TrySendItemTo(Item iItem, IItemHandler iReceiver)
    {
        if(iReceiver != null && CanGive() && iReceiver.CanReceive(this, iItem))
        {
            bool success = iReceiver.Receive(this, iItem);
            Assert.IsTrue(success);
            if(success)
            {
                iItem = null;
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
