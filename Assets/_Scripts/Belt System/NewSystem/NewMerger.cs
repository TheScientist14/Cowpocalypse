using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMerger : UpGiver
{
    // left, down, right
    private IItemHandler[] m_InputItemHandlers;
    private bool[] m_IsLookingForInput;

    private int m_CurrentInputIdx = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_InputItemHandlers = new IItemHandler[3];
        m_IsLookingForInput = new bool[3];
        for(int inputIdx = 0; inputIdx < m_InputItemHandlers.Length; inputIdx++)
        {
            m_InputItemHandlers[inputIdx] = null;
            m_IsLookingForInput[inputIdx] = false;
        }
    }

    // Update is called once per frame
    protected new void Update()
    {
        base.Update();

        for(int inputIdx = 0; inputIdx < m_InputItemHandlers.Length; inputIdx++)
        {
            if(m_InputItemHandlers[inputIdx] == null && !m_IsLookingForInput[inputIdx])
                StartCoroutine(LookForInput(inputIdx));
        }
    }

    IEnumerator LookForInput(int iInputIdx)
    {
        m_IsLookingForInput[iInputIdx] = true;

        RaycastHit2D hit;
        while(m_InputItemHandlers[iInputIdx] == null)
        {
            Vector3 direction = GetDirectionOfInputIndex(iInputIdx);
            // transform.position + direction => assuming item handlers are 1 unit large
            hit = Physics2D.Raycast(transform.position + direction, direction, 0.1f);
            if(hit.collider != null)
                m_InputItemHandlers[iInputIdx] = hit.collider.GetComponent<IItemHandler>();

            yield return new WaitForSecondsRealtime(.1f);
        }

        m_IsLookingForInput[iInputIdx] = false;
    }

    private Vector3 GetDirectionOfInputIndex(int iOutputIdx)
    {
        switch(iOutputIdx)
        {
            case 0:
                return -transform.right;
            case 1:
                return -transform.up;
            case 2:
                return transform.right;
            default:
                return Vector3.zero;
        }
    }

    private void SwitchInputIndex()
    {
        int oldInputIdx = m_CurrentInputIdx;
        do
        {
            m_CurrentInputIdx++;
            if(m_CurrentInputIdx > m_InputItemHandlers.Length)
                m_CurrentInputIdx = 0;
        } while((m_InputItemHandlers[m_CurrentInputIdx] == null || !m_InputItemHandlers[m_CurrentInputIdx].CanGive())
            && oldInputIdx != m_CurrentInputIdx);
    }

    public override bool CanReceive(IItemHandler iGiver, Item iItem)
    {
        if(!base.CanReceive(iGiver, iItem))
            return false;

        return iGiver == m_InputItemHandlers[m_CurrentInputIdx];
    }

    public override bool Receive(IItemHandler iGiver, Item iItem)
    {
        bool received = base.Receive(iGiver, iItem);

        if(received)
            SwitchInputIndex();

        return received;
    }
}
