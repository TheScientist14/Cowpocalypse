using NaughtyAttributes;
using System;
using System.Collections;
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

    [SerializeField] Sprite m_FirstCover;
    [SerializeField] Sprite m_LastCover;

    [SerializeField] Sprite m_BlankPageSprite;

    [SerializeField] GameObject m_RessourceUIPrefab;

    private List<ItemTier> m_ItemTiers = new List<ItemTier>();
    private List<List<ItemData>> m_ItemsByTier = new List<List<ItemData>>();
    private Dictionary<ItemData, List<KeyValuePair<ItemData, int>>> m_ItemsUsages = new Dictionary<ItemData, List<KeyValuePair<ItemData, int>>>();
    private int m_TotalItems = 0;

    private bool m_NeedToAddAnEmptyPage = false;

    private int m_RightPageIdx = 0; // 0 is the first cover

    // Start is called before the first frame update
    void Start()
    {
        IEnumerable<ItemData> itemDatas = ItemCreator.LoadAllResourceAtPath<ItemData>();
        Dictionary<ItemTier, List<ItemData>> itemDatasByTier = new Dictionary<ItemTier, List<ItemData>>();
        foreach(ItemData itemData in itemDatas)
        {
            if(itemData == null || itemData.Tier == null)
            {
                if(itemData == null)
                    Debug.Log("Empty item data");
                else
                    Debug.LogError("Incomplete item " + itemData.name);
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
        m_ItemTiers.Sort((ItemTier tier1, ItemTier tier2) => tier1.Level.CompareTo(tier2.Level));

        m_ItemsByTier = new List<List<ItemData>>(itemDatasByTier.Values);
        m_ItemsByTier.Sort((List<ItemData> items1, List<ItemData> items2) => items1[0].Tier.Level.CompareTo(items2[0].Tier.Level));

        if(GetPageNumber() % 2 != 0)
            m_NeedToAddAnEmptyPage = true;

        m_PrevPageButton.onClick.AddListener(PreviousPage);
        m_NextPageButton.onClick.AddListener(NextPage);
    }

    void OnEnable()
    {
        m_RightPageIdx = 0;
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

    public void GoToItemPage(ItemData iItem)
    {
        if(iItem == null)
            return;

        int itemIdx = 0;
        int tierIdx = 0;
        IEnumerator<List<ItemData>> tierIt = m_ItemsByTier.GetEnumerator();
        tierIt.MoveNext();
        while(tierIt.Current[0].Tier != iItem.Tier)
        {
            itemIdx += tierIt.Current.Count;
            if(!tierIt.MoveNext())
                return;
            tierIdx++;
        }
        IEnumerator<ItemData> itemDataIt = tierIt.Current.GetEnumerator();
        itemDataIt.MoveNext();
        while(itemDataIt.Current != iItem)
        {
            if(!itemDataIt.MoveNext())
                return;
            itemIdx++;
        }

        SetPageIndex(1 + itemIdx + (tierIdx + 1));
    }

    public void GoToItemTier(ItemTier iTier)
    {
        int pageIdx = 0;
        IEnumerator<List<ItemData>> tierIt = m_ItemsByTier.GetEnumerator();
        tierIt.MoveNext();
        while(tierIt.Current[0].Tier != iTier)
        {
            pageIdx += tierIt.Current.Count + 1;
            if(!tierIt.MoveNext())
                return;
        }

        SetPageIndex(1 + pageIdx);
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
        if(iPageIndex < 0 || iPageIndex > GetPageNumber() - 1)
            HidePage(iPage);

        if(iPageIndex == 0)
            ShowSprite(iPage, m_FirstCover);

        if(iPageIndex - 1 >= 0 && iPageIndex - 1 < m_TotalItems + m_ItemTiers.Count)
        {
            int tierLevel = 0;
            int pageIndexInTier = iPageIndex - 1;
            while(tierLevel < m_ItemsByTier.Count && pageIndexInTier > m_ItemsByTier[tierLevel].Count)
            {
                pageIndexInTier -= m_ItemsByTier[tierLevel].Count + 1;
                tierLevel++;
            }

            if(pageIndexInTier <= 0)
                ShowItemTier(iPage, tierLevel);
            else
                ShowItemData(iPage, m_ItemsByTier[tierLevel][pageIndexInTier - 1]);
        }

        if(iPageIndex == GetPageNumber() - 2 && m_NeedToAddAnEmptyPage)
            ShowSprite(iPage, m_BlankPageSprite);

        if(iPageIndex == GetPageNumber() - 1)
            ShowSprite(iPage, m_LastCover);
    }

    private void HidePage(Page iPage)
    {
        iPage.Image.gameObject.SetActive(false);
    }

    private void ShowSprite(Page iPage, Sprite iSprite, string iTitle = "")
    {
        iPage.Image.gameObject.SetActive(true);
        iPage.Image.sprite = iSprite;
        iPage.Title.text = iTitle;
        iPage.ItemInfoContainer.SetActive(false);
        iPage.ItemLockedMask.SetActive(false);
    }

    private void ShowSummary(Page iPage)
    {
        // TODO
    }

    private void ShowItemTier(Page iPage, int iTier)
    {
        iPage.Image.gameObject.SetActive(true);
        iPage.Image.sprite = m_BlankPageSprite;
        ItemTier tier = m_ItemTiers[iTier];
        iPage.Title.text = "Tier " + tier.Level + "\n" + tier.Name;
        iPage.ItemInfoContainer.SetActive(false);
        iPage.ItemLockedMask.SetActive(false);
    }

    private void ShowItemData(Page iPage, ItemData iItemData)
    {
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
            ressourceUi.OnItemDataClicked.AddListener(GoToItemPage);
        }

        if(!m_ItemsUsages.ContainsKey(iItemData))
            return;
        foreach(KeyValuePair<ItemData, int> itemUsageCount in m_ItemsUsages[iItemData])
        {
            GameObject ressource = Instantiate(m_RessourceUIPrefab, iPage.ItemUsagesContainer.transform);
            RessourceUI ressourceUi = ressource.GetComponent<RessourceUI>();
            ressourceUi.SetItemData(itemUsageCount.Key);
            ressourceUi.UpdateValue(iMax: itemUsageCount.Value);
            ressourceUi.OnItemDataClicked.AddListener(GoToItemPage);
        }
    }

    // first and last page + 1 page per item + 1 page per tier for presentation + 1 empty page if needed;
    private int GetPageNumber()
    {
        return 2 + m_TotalItems + m_ItemTiers.Count + (m_NeedToAddAnEmptyPage ? 1 : 0);
    }
}
