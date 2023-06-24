using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ItemHandlerFinder))]
public class UpGiver : SimpleItemHandler
{
    protected ItemHandlerFinder m_ItemHandlerFinder;

    [HideInInspector]
    static protected Item m_NullItem = null;

    protected void Start()
    {
        m_ItemHandlerFinder = GetComponent<ItemHandlerFinder>();
        if(m_ItemHandlerFinder == null)
        {
            Debug.LogError("Unexpected error: No found ItemHandlerFinder on " + gameObject.name);
            Debug.LogError("Self destroying");
            Destroy(gameObject);
            return;
        }

        m_ItemHandlerFinder.AddItemHandlerSearch(transform.position + transform.up);
    }

    // Update is called once per frame
    protected void Update()
    {
        if(m_ItemHandlerFinder[0] != null && m_IsItemFullyReceived && m_HandledItem != null)
            m_IsItemFullyReceived = !TrySendItemTo(ref GetItemToSend(), m_ItemHandlerFinder[0]);
    }

    protected virtual ref Item GetItemToSend()
    {
        if(!m_IsItemFullyReceived)
        {
            m_NullItem = null;
            return ref m_NullItem;
        }

        return ref m_HandledItem;
    }

    public override bool CanGive(IItemHandler iReceiver)
    {
        return GetItemToSend() != null && iReceiver == m_ItemHandlerFinder[0];
    }
}
