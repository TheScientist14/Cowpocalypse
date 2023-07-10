using UnityEngine;

[RequireComponent(typeof(ItemHandlerFinder))]
public class Splitter : SimpleItemHandler
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

		m_ItemHandlerFinder.AddItemHandlerSearch(Vector3.left);
		m_ItemHandlerFinder.AddItemHandlerSearch(Vector3.up);
		m_ItemHandlerFinder.AddItemHandlerSearch(Vector3.right);
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

	public void UnsafeSetCurrentOutputIndex(int iOutputIdx)
	{
		m_CurrentOutputIdx = iOutputIdx;
	}
}