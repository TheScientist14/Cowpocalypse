using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutoPanel : MonoBehaviour
{
    [SerializeField]
    private RawImage rawImage;
    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private Vector2 originalOffsetMin;
    private Vector2 originalOffsetMax;

    void Start()
    {
        originalAnchorMin = rawImage.rectTransform.anchorMin;
        originalAnchorMax = rawImage.rectTransform.anchorMax;
        originalOffsetMin = rawImage.rectTransform.offsetMin;
        originalOffsetMax = rawImage.rectTransform.offsetMax;
    }

        public void ToggleFullScreen()
    {
        if (IsFullScreen())
        {
            RestoreSize();
        }
        else
        {
            SetFullScreen();
        }
    }

    void SetFullScreen()
    {
        rawImage.rectTransform.anchorMin = Vector2.zero;
        rawImage.rectTransform.anchorMax = Vector2.one;
        rawImage.rectTransform.offsetMin = Vector2.zero;
        rawImage.rectTransform.offsetMax = Vector2.zero;
    }

    void RestoreSize()
    {
        rawImage.rectTransform.anchorMin = originalAnchorMin;
        rawImage.rectTransform.anchorMax = originalAnchorMax;
        rawImage.rectTransform.offsetMin = originalOffsetMin;
        rawImage.rectTransform.offsetMax = originalOffsetMax;
    }

    bool IsFullScreen()
    {
        return rawImage.rectTransform.anchorMin == Vector2.zero &&
               rawImage.rectTransform.anchorMax == Vector2.one &&
               rawImage.rectTransform.offsetMin == Vector2.zero &&
               rawImage.rectTransform.offsetMax == Vector2.zero;
    }
}
