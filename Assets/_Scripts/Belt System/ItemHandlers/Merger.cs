using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Merger : UpGiver
{
    private int m_CurrentInputIdx = 0;

    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();

        m_ItemHandlerFinder.AddItemHandlerSearch(transform.position - transform.right); // left
        m_ItemHandlerFinder.AddItemHandlerSearch(transform.position - transform.up);    // down
        m_ItemHandlerFinder.AddItemHandlerSearch(transform.position + transform.right); // right
    }

    // Update is called once per frame
    protected new void Update()
    {
        base.Update();

        if(m_ItemHandlerFinder[m_CurrentInputIdx] == null || !m_ItemHandlerFinder[m_CurrentInputIdx].CanGive(this))
            SwitchInputIndex();
    }

    private void SwitchInputIndex()
    {
        int oldInputIdx = m_CurrentInputIdx;
        do
        {
            m_CurrentInputIdx++;
            if(m_CurrentInputIdx >= m_ItemHandlerFinder.Count)
                m_CurrentInputIdx = 0;
        } while((m_ItemHandlerFinder[m_CurrentInputIdx] == null || !m_ItemHandlerFinder[m_CurrentInputIdx].CanGive(this))
            && oldInputIdx != m_CurrentInputIdx);
    }

    public override bool CanReceive(IItemHandler iGiver, Item iItem)
    {
        if(!base.CanReceive(iGiver, iItem))
            return false;

        bool doNeedToMerge = false;
        foreach(IItemHandler inputItemHandler in m_ItemHandlerFinder)
        {
            if(inputItemHandler != null && inputItemHandler.CanGive(this))
            {
                doNeedToMerge = true;
                break;
            }
        }

        if(!doNeedToMerge)
            return true;

        return iGiver == m_ItemHandlerFinder[m_CurrentInputIdx];
    }

    public override bool Receive(IItemHandler iGiver, Item iItem)
    {
        bool received = base.Receive(iGiver, iItem);

        if(received)
            SwitchInputIndex();

        return received;
    }

    public int GetCurrentInputIndex()
    {
        return m_CurrentInputIdx;
    }

    public void SetCurrentInputIndex(int iInputIdx)
    {
        Assert.IsTrue(0 <= iInputIdx && iInputIdx < m_ItemHandlerFinder.Count);
        m_CurrentInputIdx = iInputIdx;
    }
}