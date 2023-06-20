using System.Collections;
using UnityEngine;

public class UpGiver : SimpleItemHandler
{
    protected IItemHandler m_NextItemHandler;
    private bool m_IsLookingForNextBelt = false;

    // Update is called once per frame
    protected void Update()
    {
        if(m_NextItemHandler == null)
        {
            if(!m_IsLookingForNextBelt)
                StartCoroutine(LookForNextBelt());
        }
        else
            m_IsItemFullyReceived = !TrySendItemTo(GetItemToSend(), m_NextItemHandler);
    }

    IEnumerator LookForNextBelt()
    {
        m_IsLookingForNextBelt = true;

        RaycastHit2D hit;
        while(m_NextItemHandler == null)
        {
            // transform.position + transform.up => assuming item handlers are 1 unit large
            hit = Physics2D.Raycast(transform.position + transform.up, transform.up, 0.1f);
            if(hit.collider != null)
                m_NextItemHandler = hit.collider.GetComponent<IItemHandler>();

            yield return new WaitForSecondsRealtime(.1f);
        }

        m_IsLookingForNextBelt = false;
    }

    protected virtual Item GetItemToSend()
    {
        if(!m_IsItemFullyReceived)
            return null;

        return m_HandledItem;
    }

    public override bool CanGive()
    {
        return GetItemToSend() != null;
    }
}
