using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPanel : PanelComponent
{
    protected override void OnOpen()
    {
        gameObject.SetActive(true);
    }

    protected override void OnClose()
    {
        gameObject.SetActive(false);
    }

    public void TogglePanel()
    {
        if(!transform.parent.gameObject.activeInHierarchy)
            return;

        if(gameObject.activeSelf)
            Close();
        else
            Open();
    }
}
