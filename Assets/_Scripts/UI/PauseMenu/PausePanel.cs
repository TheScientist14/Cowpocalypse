using _Scripts.Save_System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanel : BasicPanel
{
	[SerializeField] ConfirmPanel m_ConfirmationWidget;

	protected override void OnOpen()
	{
		base.OnOpen();

		InputStateMachine.instance.SetState(new PauseState());
	}

	protected override void OnClose()
	{
		base.OnClose();

		InputStateMachine.instance.SetState(new FreeViewState());
	}

	public void SaveGame()
	{
		Close();

		SaveSystem saveSystem = SaveSystem.instance;

		if(!saveSystem)
			Debug.LogError("No SaveSystem found");
		else
			saveSystem.SaveGame();
	}

	public void LoadGame()
	{
		Close();
		SaveSystem.instance.LoadGame();
	}

	private void _GoToMainMenu()
	{
		InputStateMachine.instance.SetState(new FreeViewState());
		SceneManager.LoadScene(0);
	}

	public void GoToMainMenu()
	{
		if(m_ConfirmationWidget == null)
		{
			Debug.LogWarning("No confirmation panel, skipping user confirmation.");
			_GoToMainMenu();
			return;
		}

		m_ConfirmationWidget.AskForConfirmation("Do you really want to quit? Unsaved progress will be lost.", _GoToMainMenu);
	}
}