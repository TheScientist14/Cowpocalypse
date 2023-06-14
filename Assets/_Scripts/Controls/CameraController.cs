using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] float m_ZoomSpeed = 0.001f;
    [SerializeField] float m_MinZoom = 1;
    [SerializeField] float m_MaxZoom = 10;

    private Camera m_Camera;

    private Vector3 m_InitMouseWorldPos;
    private Vector3 m_InitTransformWorldPos;
    private bool m_IsDragging = false;

    private InputsActions m_InputAction;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = GetComponent<Camera>();

        m_InputAction = InputMaster.instance.InputAction;

        m_InputAction.Player.ZoomValue.performed += ctx => UpdateZoom(ctx.ReadValue<float>());

        m_InputAction.Player.Drag.started += _ => InitMoveCamera();
        m_InputAction.Player.Drag.canceled += _ => StopMoveCamera();
        m_InputAction.Player.PointerPosition.performed += MovePosition;

    }

    void UpdateZoom(float iZoomDelta)
    {
        m_Camera.orthographicSize -= iZoomDelta * m_ZoomSpeed;
        m_Camera.orthographicSize = Mathf.Clamp(m_Camera.orthographicSize, m_MinZoom, m_MaxZoom);
    }

    void InitMoveCamera()
    {
        if(m_IsDragging)
            return;

        m_IsDragging = true;
        m_InitMouseWorldPos = m_Camera.ScreenToWorldPoint(m_InputAction.Player.PointerPosition.ReadValue<Vector2>());
        m_InitTransformWorldPos = transform.position;
    }

    void StopMoveCamera()
    {
        m_IsDragging = false;
    }

    void MovePosition(InputAction.CallbackContext iCtx)
    {
        if(!m_IsDragging)
            return;

        Vector2 newMouseScreenPos = iCtx.ReadValue<Vector2>();
        transform.position = m_InitTransformWorldPos;
        Vector3 deltaPosWC = m_InitMouseWorldPos - m_Camera.ScreenToWorldPoint(newMouseScreenPos);
        transform.Translate(deltaPosWC, Space.Self);
    }
}
