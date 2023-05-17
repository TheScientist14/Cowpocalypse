using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ConfirmBehaviour : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_ConfirmText;

    private UnityAction m_OnConfirm;
    private UnityAction m_OnCancel;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void AskForConfirmation(string iQuestion, UnityAction iConfirmCallback, UnityAction iCancelCallback = null)
    {
        m_ConfirmText.text = iQuestion;
        m_OnConfirm = iConfirmCallback;
        m_OnCancel = iCancelCallback;
        gameObject.SetActive(true);
    }

    public void Confirm()
    {
        gameObject.SetActive(false);
        if(m_OnConfirm == null)
            return;
        m_OnConfirm.Invoke();
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
        if(m_OnConfirm == null)
            return;
        m_OnCancel.Invoke();
    }

}
