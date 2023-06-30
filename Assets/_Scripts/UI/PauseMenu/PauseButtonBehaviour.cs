using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PauseButtonBehaviour : MonoBehaviour
{
    [SerializeField] GameObject m_PauseMenuPanel;

    public void PauseGame()
    {
        m_PauseMenuPanel.SetActive(true);
        InputStateMachine.instance.SetState(new PauseState());
    }
}