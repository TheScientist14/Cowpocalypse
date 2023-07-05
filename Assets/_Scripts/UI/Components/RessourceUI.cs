using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RessourceUI : MonoBehaviour
{
    [SerializeField] Image m_Image;
    [SerializeField] Sprite m_NoItemSprite = null;

    [SerializeField] TextMeshProUGUI m_MaxCountText;
    [SerializeField] TextMeshProUGUI m_CurrentCountText;
    [SerializeField] string m_Separator = "/";

    [SerializeField] TextMeshProUGUI m_UnlockPriceText;
    [SerializeField] GameObject m_LockedIcon;

    private ItemData m_ItemData = null;

    // invoked only if item is unlocked
    public UnityEvent<ItemData> OnItemDataClicked;

    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnEnable()
    {
        UpdateValue(); // refresh locked state
    }

    void OnClick()
    {
        if(m_ItemData == null)
            return;

        if(!m_ItemData.Unlocked)
        {
            if(Wallet.instance.Money < m_ItemData.Tier.UnlockPrice)
            {
                // TODO: ajout d'un feedback (son, message ou autre) pour prévenir le joueur qu'il n'as pas assez d'argent
                Debug.LogWarning(
                    $"Not enough cash to unlock {m_ItemData.Name}. Need ${m_ItemData.Tier.UnlockPrice - Wallet.instance.Money} more.");
                return;
            }

            Wallet.instance.Money -= m_ItemData.Tier.UnlockPrice;
            m_ItemData.Unlocked = true;
            UpdateValue();
            return;
        }
        else
            OnItemDataClicked.Invoke(m_ItemData);
    }

    public void UpdateValue(int? iCurrent = null, int? iMax = null)
    {
        if(m_ItemData == null || !m_ItemData.Unlocked)
        {
            m_LockedIcon.SetActive(true);
            m_UnlockPriceText.gameObject.SetActive(true);
            if(m_ItemData != null)
                m_UnlockPriceText.text = $"{m_ItemData.Tier.UnlockPrice}";

            m_CurrentCountText.gameObject.SetActive(false);
            m_MaxCountText.gameObject.SetActive(false);

            m_Image.color = Color.black;
        }
        else
        {
            m_LockedIcon.SetActive(false);
            m_UnlockPriceText.gameObject.SetActive(false);

            m_CurrentCountText.gameObject.SetActive(true);
            m_MaxCountText.gameObject.SetActive(true);

            m_Image.color = Color.white;

            if(iMax.HasValue)
            {
                if(iCurrent.HasValue)
                {
                    m_CurrentCountText.text = iCurrent.Value.ToString();
                    m_MaxCountText.text = m_Separator + iMax.Value.ToString();
                }
                else
                {
                    m_CurrentCountText.text = "";
                    m_MaxCountText.text = iMax.Value.ToString();
                }
            }
            else if(iCurrent.HasValue)
            {
                m_CurrentCountText.text = iCurrent.Value.ToString();
            }
        }
    }

    public ItemData GetItemData()
    {
        return m_ItemData;
    }

    public void SetItemData(ItemData iItemData)
    {
        m_ItemData = iItemData;
        if(m_ItemData == null)
        {
            m_CurrentCountText.text = "NO";
            m_MaxCountText.text = "NE";
            m_Image.sprite = m_NoItemSprite;
            gameObject.name = "None";
            return;
        }

        m_Image.sprite = m_ItemData.Sprite;
        gameObject.name = m_ItemData.name;
        UpdateValue(iMax: m_ItemData.Price);
    }
}