using UnityEngine;

public class RotateInput : MonoBehaviour
{
	private InputsActions m_InputsActions;
	private Camera m_Camera;

	// Start is called before the first frame update
	void Start()
	{
		m_Camera = Camera.main;
		m_InputsActions = InputMaster.instance.InputAction;
		m_InputsActions.Player.DoubleClickButton.performed += _ => RotateItemHandlerOnCursor();
	}

	private void RotateItemHandlerOnCursor()
	{
		Vector2 mousePosition = m_InputsActions.Player.PointerPosition.ReadValue<Vector2>();
		Vector3 worldMousePos = m_Camera.ScreenToWorldPoint(mousePosition);
		ItemHandlerManager.instance.RotateItemHandler(GridManager.instance.GetItemHandlerAt(worldMousePos));
	}
}
