using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TierUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_TierName;
    [SerializeField] LayoutGroup m_Layout;
    [SerializeField] RessourceUI m_RessourceUiPrefab;

    public void InstantiateRecipes(IEnumerable<ItemData> iItemsInTier, MachinePanel iMachinePanel)
    {
        foreach(ItemData itemData in iItemsInTier)
        {
            RessourceUI ressource = Instantiate(m_RessourceUiPrefab, m_Layout.transform);
            ressource.SetItemData(itemData);
            ressource.OnItemDataClicked.AddListener(iMachinePanel.SetItemData);
            ressource.OnItemDataClicked.AddListener(_ => iMachinePanel.Close());
        }
    }

    public void SetTierName(string iName)
    {
        m_TierName.text = iName;
    }
}
