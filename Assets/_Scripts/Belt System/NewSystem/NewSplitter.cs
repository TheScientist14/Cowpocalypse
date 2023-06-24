using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(ItemHandlerFinder))]
public class NewSplitter : SimpleItemHandler
{
    // left = 0, up = 1, right = 2
    private ItemHandlerFinder m_ItemHandlerFinder;

    private int m_CurrentOutputIdx = 0;

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

        m_ItemHandlerFinder.AddItemHandlerSearch(transform.position - transform.right); // left
        m_ItemHandlerFinder.AddItemHandlerSearch(transform.position + transform.up);    // up
        m_ItemHandlerFinder.AddItemHandlerSearch(transform.position + transform.right); // right
    }

    // Update is called once per frame
    protected void Update()
    {
        if(m_HandledItem != null && m_IsItemFullyReceived)
        {
            SwitchOutputIndex();
            m_IsItemFullyReceived = !TrySendItemTo(ref m_HandledItem, m_ItemHandlerFinder[m_CurrentOutputIdx]);
        }
    }

    private void SwitchOutputIndex()
    {
        int oldOutputIdx = m_CurrentOutputIdx;
        do
        {
            m_CurrentOutputIdx++;
            if(m_CurrentOutputIdx >= m_ItemHandlerFinder.Count)
                m_CurrentOutputIdx = 0;
        } while((m_ItemHandlerFinder[m_CurrentOutputIdx] == null || !m_ItemHandlerFinder[m_CurrentOutputIdx].CanReceive(this, m_HandledItem))
            && oldOutputIdx != m_CurrentOutputIdx);
    }

    public override bool CanGive(IItemHandler iReceiver)
    {
        return HasFullyReceivedItem() && iReceiver == m_ItemHandlerFinder[m_CurrentOutputIdx];
    }

    public int GetCurrentOutputIndex()
    {
        return m_CurrentOutputIdx;
    }

    public void SetCurrentOutputIndex(int iOutputIdx)
    {
        Assert.IsTrue(0 <= iOutputIdx && iOutputIdx < m_ItemHandlerFinder.Count);
        m_CurrentOutputIdx = iOutputIdx;
    }
}