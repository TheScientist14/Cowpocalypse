using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutoView : MonoBehaviour
{
    [SerializeField] RawImage m_RawImage;

    private Vector2 m_OriginalAnchorMin;
    private Vector2 m_OriginalAnchorMax;
    private Vector2 m_OriginalOffsetMin;
    private Vector2 m_OriginalOffsetMax;

    void Start()
    {
        m_OriginalAnchorMin = m_RawImage.rectTransform.anchorMin;
        m_OriginalAnchorMax = m_RawImage.rectTransform.anchorMax;
        m_OriginalOffsetMin = m_RawImage.rectTransform.offsetMin;
        m_OriginalOffsetMax = m_RawImage.rectTransform.offsetMax;
    }

    void OnEnable()
    {
        if(!IsFullScreen())
            RestoreSize();
    }

    public void ToggleFullScreen()
    {
        if(IsFullScreen())
            RestoreSize();
        else
            SetFullScreen();
    }

    public void SetFullScreen()
    {
        m_RawImage.rectTransform.anchorMin = Vector2.zero;
        m_RawImage.rectTransform.anchorMax = Vector2.one;
        m_RawImage.rectTransform.offsetMin = Vector2.zero;
        m_RawImage.rectTransform.offsetMax = Vector2.zero;
    }

    public void RestoreSize()
    {
        m_RawImage.rectTransform.anchorMin = m_OriginalAnchorMin;
        m_RawImage.rectTransform.anchorMax = m_OriginalAnchorMax;
        m_RawImage.rectTransform.offsetMin = m_OriginalOffsetMin;
        m_RawImage.rectTransform.offsetMax = m_OriginalOffsetMax;
    }

    public bool IsFullScreen()
    {
        return m_RawImage.rectTransform.anchorMin == Vector2.zero &&
               m_RawImage.rectTransform.anchorMax == Vector2.one &&
               m_RawImage.rectTransform.offsetMin == Vector2.zero &&
               m_RawImage.rectTransform.offsetMax == Vector2.zero;
    }
}
