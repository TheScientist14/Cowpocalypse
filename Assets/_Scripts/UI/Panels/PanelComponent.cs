using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class PanelComponent : MonoBehaviour
{
    private static PanelComponent s_TopPanel = null;

    public static PanelComponent GetTopPanel()
    {
        return s_TopPanel;
    }

    ///////////////////////////////

    [Tooltip("Null if no parent. This will be a top panel.")]
    [SerializeField] protected PanelComponent m_Parent = null;
    protected PanelComponent m_OpenedChild = null;

    public void Open()
    {
        if(m_Parent != null)
            m_Parent.SetChild(this);
        else
        {
            if(s_TopPanel != null)
                s_TopPanel.Close();

            s_TopPanel = this;
        }

        OnOpen();
    }

    public void Close()
    {
        if(m_OpenedChild != null)
            m_OpenedChild.Close();

        if(m_Parent != null && m_Parent.m_OpenedChild == this)
            m_Parent.RemoveChild(this);

        if(s_TopPanel == this)
            s_TopPanel = null;

        OnClose();
    }

    private void SetChild(PanelComponent iChildPanel)
    {
        if(m_OpenedChild == iChildPanel)
            return;

        if(m_OpenedChild != null)
            iChildPanel.Close();

        Assert.AreEqual(this, iChildPanel.m_Parent);
        m_OpenedChild = iChildPanel;
    }

    private void RemoveChild(PanelComponent iChildPanel)
    {
        Assert.AreEqual(m_OpenedChild, iChildPanel);
        m_OpenedChild = null;
    }

    protected virtual void OnOpen() { }

    protected virtual void OnClose() { }

    protected virtual void OnDestroy()
    {
        Close();
    }
}
