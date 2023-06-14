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
    [SerializeField] Transform m_MinCamera;
    [SerializeField] Transform m_MaxCamera;

    private Camera m_Camera;

    private bool m_IsPinching = false;
    private float m_PrevPinchDist;

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
        m_InputAction.Player.Pinch2.started += _ => StartPinch();
        m_InputAction.Player.Pinch1.performed += _ => UpdatePinch();
        m_InputAction.Player.Pinch2.performed += _ => UpdatePinch();
        m_InputAction.Player.Pinch2.canceled += _ => EndPinch();

        m_InputAction.Player.Drag.started += _ => InitMoveCamera();
        m_InputAction.Player.Drag.canceled += _ => StopMoveCamera();
        m_InputAction.Player.PointerPosition.performed += MovePosition;

    }

    void UpdateZoom(float iZoomDelta)
    {
        m_Camera.orthographicSize -= iZoomDelta * m_ZoomSpeed;
        m_Camera.orthographicSize = Mathf.Clamp(m_Camera.orthographicSize, m_MinZoom, m_MaxZoom);
    }

    void StartPinch()
    {
        if(m_IsPinching)
            return;

        m_IsPinching = true;
        m_PrevPinchDist = (m_InputAction.Player.Pinch1.ReadValue<Vector2>() - m_InputAction.Player.Pinch2.ReadValue<Vector2>()).magnitude;
    }

    void UpdatePinch()
    {
        if(!m_IsPinching)
            return;

        float pinchDist = (m_InputAction.Player.Pinch1.ReadValue<Vector2>() - m_InputAction.Player.Pinch2.ReadValue<Vector2>()).magnitude;
        UpdateZoom(pinchDist - m_PrevPinchDist);
        m_PrevPinchDist = pinchDist;
    }

    void EndPinch()
    {
        m_IsPinching = false;
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

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, m_MinCamera.position.x, m_MaxCamera.position.x),
            Mathf.Clamp(transform.position.x, m_MinCamera.position.y, m_MaxCamera.position.y),
            transform.position.z);
    }
}
