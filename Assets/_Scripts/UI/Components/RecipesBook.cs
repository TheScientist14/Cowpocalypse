using NaughtyAttributes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipesBook : MonoBehaviour
{
	[Serializable]
	struct Page
	{
		public Image Image;
		public TextMeshProUGUI Title;
		public TextMeshProUGUI PageIndex;
		public GameObject SummaryContainer;
		public GameObject ItemInfoContainer;
		public GameObject ItemLockedMask;
		public Image ItemIcon;
		public TextMeshProUGUI ItemPrice;
		public GameObject ItemRecipeContainer;
		public GameObject ItemUsagesContainer;
	}

	[SerializeField] Page m_LeftPage;
	[SerializeField] Page m_RightPage;

	[SerializeField] Button m_PrevPageButton;
	[SerializeField] Button m_NextPageButton;

	[SerializeField] Sprite m_FrontCoverRecto;
	[SerializeField] Sprite m_FrontCoverVerso;
	[SerializeField] Sprite m_BackCoverRecto;
	[SerializeField] Sprite m_BackCoverVerso;

	[SerializeField] Sprite m_BlankPageSprite;

	[SerializeField] GameObject m_RessourceUIPrefab;
	[SerializeField] GameObject m_ClickableTextPrefab;

	private List<ItemTier> m_ItemTiers = new List<ItemTier>();
	private List<List<ItemData>> m_ItemsByTier = new List<List<ItemData>>(); // last element in every list can be null when need an empty page
	private Dictionary<ItemData, List<KeyValuePair<ItemData, int>>> m_ItemsUsages =
		new Dictionary<ItemData, List<KeyValuePair<ItemData, int>>>();
	private int m_TotalItems = 0;

	private int m_RightPageIdx;

	// Start is called before the first frame update
	void Start()
	{
		InitItemsLists();
		GenerateSummaryLinks();

		SetPageIndex(GetFrontCoverPageIndex());

		m_PrevPageButton.onClick.AddListener(PreviousPage);
		m_NextPageButton.onClick.AddListener(NextPage);
	}

	private void InitItemsLists()
	{
		IEnumerable<ItemData> itemDatas = ItemCreator.LoadAllResourceAtPath<ItemData>();
		Dictionary<ItemTier, List<ItemData>> itemDatasByTier = new Dictionary<ItemTier, List<ItemData>>();
		foreach(ItemData itemData in itemDatas)
		{
			if(itemData == null || itemData.Tier == null)
			{
				if(itemData == null)
					Debug.Log("Empty item data.");
				else
					Debug.LogError("Incomplete item " + itemData.name + ": no assigned tier.");
				continue;
			}

			m_TotalItems++;

			if(!itemDatasByTier.ContainsKey(itemData.Tier))
				itemDatasByTier.Add(itemData.Tier, new List<ItemData>());

			itemDatasByTier[itemData.Tier].Add(itemData);

			if(itemData.Recipes == null)
				continue;

			foreach(KeyValuePair<ItemData, int> recipeItemCount in itemData.Recipes)
			{
				if(!m_ItemsUsages.ContainsKey(recipeItemCount.Key))
					m_ItemsUsages.Add(recipeItemCount.Key, new List<KeyValuePair<ItemData, int>>());

				m_ItemsUsages[recipeItemCount.Key].Add(new KeyValuePair<ItemData, int>(itemData, recipeItemCount.Value));
			}
		}
		m_ItemTiers = new List<ItemTier>(itemDatasByTier.Keys);
		m_ItemsByTier = new List<List<ItemData>>(itemDatasByTier.Values);

		// sorting
		m_ItemTiers.Sort((ItemTier tier1, ItemTier tier2) => tier1.Level.CompareTo(tier2.Level));
		m_ItemsByTier.Sort((List<ItemData> items1, List<ItemData> items2) => items1[0].Tier.Level.CompareTo(items2[0].Tier.Level));

		foreach(List<ItemData> itemsInTier in m_ItemsByTier)
		{
			itemsInTier.Sort((ItemData item1, ItemData item2) => item1.Name.CompareTo(item2.Name));
			if(itemsInTier.Count % 2 == 0)
			{
				itemsInTier.Add(null); // empty page to ensure next tier page lays on the right page
				m_TotalItems++;
			}
		}
	}

	private void GenerateSummaryLinks()
	{
		// only right page shall contain the summary
		foreach(Transform child in m_RightPage.SummaryContainer.transform)
			Destroy(child.gameObject);

		foreach(ItemTier itemTier in m_ItemTiers)
		{
			GameObject clickableText = Instantiate(m_ClickableTextPrefab, m_RightPage.SummaryContainer.transform);
			Button button = clickableText.GetComponentInChildren<Button>();
			int tierPageIdx = GetTierPageIndex(itemTier);
			button.onClick.AddListener(() => SetPageIndex(tierPageIdx));
			TextMeshProUGUI text = clickableText.GetComponentInChildren<TextMeshProUGUI>();
			text.text = itemTier.Name + " (" + GetRealPageIndex(tierPageIdx) + ")";
		}
	}

	void OnEnable()
	{
		SetPageIndex(GetFrontCoverPageIndex());
		Render();
	}

	[Button]
	public void NextPage()
	{
		if(m_RightPageIdx >= GetPageNumber() - 1)
			return;

		m_RightPageIdx += 2;
		Render();
	}

	[Button]
	public void PreviousPage()
	{
		if(m_RightPageIdx <= 0)
			return;

		m_RightPageIdx -= 2;
		Render();
	}

	public void SetPageIndex(int iPageIndex)
	{
		int pageIndex = Mathf.Clamp(iPageIndex, 0, GetPageNumber());
		if(pageIndex % 2 != 0)
			pageIndex++;

		m_RightPageIdx = pageIndex;
		Render();
	}

	public int GetFrontCoverPageIndex()
	{
		return 0;
	}

	public int GetSummaryPageIndex()
	{
		return GetFrontCoverPageIndex() + 2;
	}

	public int GetItemPageIndex(ItemData iItem)
	{
		if(iItem == null)
			return 0;

		List<ItemData> itemsInTier = m_ItemsByTier.Find(iItems => iItems.Contains(iItem));
		if(itemsInTier == null)
			return 0;
		int itemIdx = itemsInTier.IndexOf(iItem) + 1;

		return GetTierPageIndex(iItem.Tier) + itemIdx;
	}

	public int GetTierPageIndex(ItemTier iTier)
	{
		int tierIdx = m_ItemTiers.IndexOf(iTier) + 1;
		int itemsInPreviousTiers = 0;
		int visitedTiers = 0;
		foreach(List<ItemData> itemsInTier in m_ItemsByTier)
		{
			if(visitedTiers == tierIdx - 1)
				break;

			itemsInPreviousTiers += itemsInTier.Count;
			visitedTiers++;
		}

		return GetSummaryPageIndex() + 1 + tierIdx + itemsInPreviousTiers;
	}

	public int GetBackCoverPageIndex()
	{
		return GetPageNumber() - 1;
	}

	private int GetRealPageIndex(int iPageIndex)
	{
		int firstIndexedPage = GetSummaryPageIndex();
		return iPageIndex - firstIndexedPage + 1;
	}

	private void Render()
	{
		m_PrevPageButton.gameObject.SetActive(m_RightPageIdx >= 2);
		m_NextPageButton.gameObject.SetActive(m_RightPageIdx < GetPageNumber() - 1);

		RenderPage(m_LeftPage, m_RightPageIdx - 1);
		RenderPage(m_RightPage, m_RightPageIdx);
	}

	private void RenderPage(Page iPage, int iPageIndex)
	{
		_ClearPage(iPage);

		int realPageIndex = GetRealPageIndex(iPageIndex);
		if(1 <= realPageIndex && realPageIndex <= GetRealPageNumber())
			iPage.PageIndex.text = realPageIndex.ToString();

		if(iPageIndex < 0 || iPageIndex > GetPageNumber() - 1)
			_HidePage(iPage);

		if(iPageIndex == GetFrontCoverPageIndex())
			_ShowSprite(iPage, m_FrontCoverRecto);
		if(iPageIndex == GetFrontCoverPageIndex() + 1)
			_ShowSprite(iPage, m_FrontCoverVerso);

		if(iPageIndex == GetSummaryPageIndex())
			_ShowSummary(iPage);
		if(iPageIndex == GetSummaryPageIndex() + 1)
			_ShowSprite(iPage, m_BlankPageSprite);

		int firstContentPageIdx = GetSummaryPageIndex() + 2;
		int pageContentIdx = iPageIndex - firstContentPageIdx;
		if(0 <= pageContentIdx && pageContentIdx < m_TotalItems + m_ItemTiers.Count)
		{
			int tierLevel = 0;
			int pageIndexInTier = pageContentIdx;
			while(tierLevel < m_ItemsByTier.Count && pageIndexInTier > m_ItemsByTier[tierLevel].Count)
			{
				pageIndexInTier -= m_ItemsByTier[tierLevel].Count + 1;
				tierLevel++;
			}

			if(pageIndexInTier <= 0)
				_ShowItemTier(iPage, tierLevel);
			else
				_ShowItemData(iPage, m_ItemsByTier[tierLevel][pageIndexInTier - 1]);
		}

		if(iPageIndex == GetBackCoverPageIndex() - 1)
			_ShowSprite(iPage, m_BackCoverRecto);
		if(iPageIndex == GetBackCoverPageIndex())
			_ShowSprite(iPage, m_BackCoverVerso);
	}

	private void _ClearPage(Page iPage)
	{
		iPage.Title.text = "";
		iPage.PageIndex.text = "";
		iPage.ItemInfoContainer.SetActive(false);
		iPage.ItemLockedMask.SetActive(false);
		iPage.SummaryContainer.SetActive(false);
	}

	private void _HidePage(Page iPage)
	{
		iPage.Image.gameObject.SetActive(false);
	}

	private void _ShowSprite(Page iPage, Sprite iSprite, string iTitle = "")
	{
		iPage.Image.gameObject.SetActive(true);
		iPage.Image.sprite = iSprite;
		iPage.Title.text = iTitle;
	}

	private void _ShowSummary(Page iPage)
	{
		_ShowSprite(iPage, m_BlankPageSprite, "Summary");
		iPage.SummaryContainer.SetActive(true);
	}

	private void _ShowItemTier(Page iPage, int iTier)
	{
		iPage.Image.gameObject.SetActive(true);
		iPage.Image.sprite = m_BlankPageSprite;
		ItemTier tier = m_ItemTiers[iTier];
		iPage.Title.text = "Tier " + tier.Level + "\n" + tier.Name;
	}

	private void _ShowItemData(Page iPage, ItemData iItemData)
	{
		if(iItemData == null)
		{
			_ShowSprite(iPage, m_BlankPageSprite);
			return;
		}

		iPage.Image.gameObject.SetActive(true);
		iPage.Image.sprite = m_BlankPageSprite;

		iPage.ItemInfoContainer.SetActive(true);
		iPage.ItemLockedMask.SetActive(!iItemData.Unlocked);
		iPage.Title.text = iItemData.Name;
		iPage.ItemIcon.sprite = iItemData.Sprite;
		iPage.ItemPrice.text = iItemData.Price + "$";

		foreach(Transform child in iPage.ItemRecipeContainer.transform)
			Destroy(child.gameObject);
		foreach(Transform child in iPage.ItemUsagesContainer.transform)
			Destroy(child.gameObject);

		foreach(KeyValuePair<ItemData, int> recipeItemCount in iItemData.Recipes)
		{
			GameObject ressource = Instantiate(m_RessourceUIPrefab, iPage.ItemRecipeContainer.transform);
			RessourceUI ressourceUi = ressource.GetComponent<RessourceUI>();
			ressourceUi.SetItemData(recipeItemCount.Key);
			ressourceUi.UpdateValue(iMax: recipeItemCount.Value);
			ressourceUi.OnItemDataClicked.AddListener(item => SetPageIndex(GetItemPageIndex(item)));
		}

		if(!m_ItemsUsages.ContainsKey(iItemData))
			return;
		foreach(KeyValuePair<ItemData, int> itemUsageCount in m_ItemsUsages[iItemData])
		{
			GameObject ressource = Instantiate(m_RessourceUIPrefab, iPage.ItemUsagesContainer.transform);
			RessourceUI ressourceUi = ressource.GetComponent<RessourceUI>();
			ressourceUi.SetItemData(itemUsageCount.Key);
			ressourceUi.UpdateValue(iMax: itemUsageCount.Value);
			ressourceUi.OnItemDataClicked.AddListener(item => SetPageIndex(GetItemPageIndex(item)));
		}
	}

	// front cover (recto/verso) + summary + empty page + 1 page per tier + 1 page per item + back cover (recto/verso)
	private int GetPageNumber()
	{
		return 6 + m_TotalItems + m_ItemTiers.Count;
	}

	private int GetRealPageNumber()
	{
		return 2 + m_TotalItems + m_ItemTiers.Count;
	}
}
