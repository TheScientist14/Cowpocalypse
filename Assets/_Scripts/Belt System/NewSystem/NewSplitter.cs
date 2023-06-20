using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSplitter : SimpleItemHandler
{
    // left, up, right
    private IItemHandler[] m_OutputItemHandlers;
    private bool[] m_IsLookingForOutput;

    private int m_CurrentOutputIdx = 0;

    void Start()
    {
        m_OutputItemHandlers = new IItemHandler[3];
        m_IsLookingForOutput = new bool[3];
        for(int outputIdx = 0; outputIdx < m_OutputItemHandlers.Length; outputIdx++)
        {
            m_OutputItemHandlers[outputIdx] = null;
            m_IsLookingForOutput[outputIdx] = false;
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        for(int outputIdx = 0; outputIdx < m_OutputItemHandlers.Length; outputIdx++)
        {
            if(m_OutputItemHandlers[outputIdx] == null && !m_IsLookingForOutput[outputIdx])
                StartCoroutine(LookForOutput(outputIdx));
        }

        if(m_HandledItem != null && m_IsItemFullyReceived)
        {
            SwitchOutputIndex();
            m_IsItemFullyReceived = !TrySendItemTo(m_HandledItem, m_OutputItemHandlers[m_CurrentOutputIdx]);
        }
    }

    IEnumerator LookForOutput(int iOutputIdx)
    {
        m_IsLookingForOutput[iOutputIdx] = true;

        RaycastHit2D hit;
        while(m_OutputItemHandlers[iOutputIdx] == null)
        {
            Vector3 direction = GetDirectionOfOutputIndex(iOutputIdx);
            // transform.position + direction => assuming item handlers are 1 unit large
            hit = Physics2D.Raycast(transform.position + direction, direction, 0.1f);
            if(hit.collider != null)
                m_OutputItemHandlers[iOutputIdx] = hit.collider.GetComponent<IItemHandler>();

            yield return new WaitForSecondsRealtime(.1f);
        }

        m_IsLookingForOutput[iOutputIdx] = false;
    }

    private Vector3 GetDirectionOfOutputIndex(int iOutputIdx)
    {
        switch(iOutputIdx)
        {
            case 0:
                return -transform.right;
            case 1:
                return transform.up;
            case 2:
                return transform.right;
            default:
                return Vector3.zero;
        }
    }

    private void SwitchOutputIndex()
    {
        int oldOutputIdx = m_CurrentOutputIdx;
        do
        {
            m_CurrentOutputIdx++;
            if(m_CurrentOutputIdx > m_OutputItemHandlers.Length)
                m_CurrentOutputIdx = 0;
        } while((m_OutputItemHandlers[m_CurrentOutputIdx] == null || !m_OutputItemHandlers[m_CurrentOutputIdx].CanReceive(this, m_HandledItem))
            && oldOutputIdx != m_CurrentOutputIdx);
    }
}
