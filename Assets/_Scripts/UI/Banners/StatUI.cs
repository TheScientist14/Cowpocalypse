﻿using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatUI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] TextMeshProUGUI m_PriceText;
    [SerializeField] Image m_StatIcon;
    [SerializeField] Image m_LvlImage;

    [SerializeField] int m_StatIdx;
    private Stat m_Stat;

    void Start()
    {
        m_Stat = StatManager.instance.GetStat(m_StatIdx);
        if(m_Stat == null)
            return;

        StatManager.instance.OnStatUpdated.AddListener(Draw);
        Draw();
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        Buy();
    }

    void Draw()
    {
        m_PriceText.text = m_Stat.IsMaxedOut() ? "MAX" : $"$ {m_Stat.StatData.Prices[m_Stat.CurrentLevel - 1]}";
        m_StatIcon.sprite = m_Stat.StatData.Icon;
        m_LvlImage.fillAmount = (float)(m_Stat.CurrentLevel - 1) / m_Stat.StatData.MaxLevelInclusive;
    }

    void Buy()
    {
        StatManager.instance.AddLevelToStat(m_StatIdx);
    }
}