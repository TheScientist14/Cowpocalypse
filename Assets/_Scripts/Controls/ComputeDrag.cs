/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ComputeDrag : MonoBehaviour
{
    private InputsActions m_InputAction;

    private List<Vector2> m_InitPos;
    private List<bool> m_IsDragging;
    public List<UnityEvent<Vector2>> m_DragEvents;

    public static class DragType
    {
        public static readonly int Basic = 0;
        public static readonly int BuildMode = 1;

        public static readonly int NB_OF_DRAG_TYPE = 2; // should always be last
    }

    // Start is called before the first frame update
    void Start()
    {
        m_InputAction = InputMaster.instance.InputAction;
        m_InitPos = new List<Vector2>(DragType.NB_OF_DRAG_TYPE);
        m_IsDragging = new List<bool>(DragType.NB_OF_DRAG_TYPE);
        m_DragEvents = new List<UnityEvent<Vector2>>(DragType.NB_OF_DRAG_TYPE);

    }

    private void OnEnable()
    {
        m_InputAction.Player.Drag.started += ctx => InitDrag(ctx, DragType.Basic);
        m_InputAction.Player.DragBuildMode.started += ctx => InitDrag(ctx, DragType.BuildMode);

        m_InputAction.Player.Drag.performed += ctx => PerformDrag(ctx, DragType.Basic);
        m_InputAction.Player.DragBuildMode.performed += ctx => PerformDrag(ctx, DragType.BuildMode);

        m_InputAction.Player.Drag.canceled += ctx => CancelDrag(ctx, DragType.Basic);
        m_InputAction.Player.DragBuildMode.canceled += ctx => CancelDrag(ctx, DragType.BuildMode);
    }

    private void InitDrag(InputAction.CallbackContext iCtx, int iDragIndex)
    {
        m_InitPos[iDragIndex] = m_InputAction.Player.PointerPosition.ReadValue<Vector2>();
        // m_IsDragging[iDragIndex] = true;
    }

    // Update is called once per frame
    private void PerformDrag(InputAction.CallbackContext iCtx, int iDragIndex)
    {
        m_DragEvents
    }

    private void CancelDrag(InputAction.CallbackContext iCtx, int iDragIndex)
    {
        m_DragEvents
    }
}
*/