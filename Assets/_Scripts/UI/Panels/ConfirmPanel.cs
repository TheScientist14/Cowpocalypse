using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ConfirmPanel : BasicPanel
{
    [SerializeField] TextMeshProUGUI m_ConfirmText;

    private UnityAction m_OnConfirm;
    private UnityAction m_OnCancel;

    // Start is called before the first frame update
    void Start()
    {
        Close();
    }

    public void AskForConfirmation(string iQuestion, UnityAction iConfirmCallback, UnityAction iCancelCallback = null)
    {
        Open();

        m_ConfirmText.text = iQuestion;
        m_OnConfirm = iConfirmCallback;
        m_OnCancel = iCancelCallback;
    }

    public void Confirm()
    {
        Close();

        if(m_OnConfirm == null)
            return;
        m_OnConfirm.Invoke();
    }

    public void Cancel()
    {
        Close();

        if(m_OnConfirm == null)
            return;
        m_OnCancel.Invoke();
    }

}
