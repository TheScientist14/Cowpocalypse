using _Scripts.Save_System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LoadButtonBehaviour : MonoBehaviour
{
	private Button m_Button;

	// Start is called before the first frame update
	void Start()
	{
		m_Button = GetComponent<Button>();
		m_Button.interactable = false;

		CheckForSave();
	}

	void OnEnable()
	{
		if(m_Button == null)
			return;

		CheckForSave();
	}

	private void CheckForSave()
	{
		if(SaveSystem.instance == null)
			return;

		m_Button.interactable = SaveSystem.instance.CheckForSave();
	}
}
