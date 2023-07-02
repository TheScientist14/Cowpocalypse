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
    [SerializeField] Transform m_MinCamera; // bottom left corner
    [SerializeField] Transform m_MaxCamera; // top right corner

    private Camera m_Camera;

    private bool m_IsPinching = false;
    private float m_PrevPinchDist;

    private Vector3 m_InitMouseWorldPos;
    private Vector3 m_InitTransformWorldPos;
    private bool m_IsDragging = false;

    private bool m_MoveWithKB = false;
    private Vector3 m_CameraMovementKBInput = Vector3.zero;

    private InputsActions m_InputAction;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = GetComponent<Camera>();

        m_InputAction = InputMaster.instance.InputAction;

        m_InputAction.Player.ZoomValue.performed += ctx => UpdateZoom(ctx.ReadValue<float>() * m_ZoomSpeed);
        m_InputAction.Player.Pinch2.started += _ => StartPinch();
        m_InputAction.Player.PointerPosition1.performed += _ => UpdatePinch();
        m_InputAction.Player.PointerPosition.performed += _ => UpdatePinch();
        m_InputAction.Player.Pinch2.canceled += _ => EndPinch();

        InputMaster.instance.OnStartDrag.AddListener(InitMoveCamera);
        m_InputAction.Player.Drag.canceled += _ => StopMoveCamera();
        m_InputAction.Player.PointerPosition.performed += MovePositionThroughDrag;

        m_InputAction.Player.Camera.started += _ => m_MoveWithKB = true;
        m_InputAction.Player.Camera.performed += ctx => m_CameraMovementKBInput = ctx.ReadValue<Vector3>();
        m_InputAction.Player.Camera.canceled += _ => m_MoveWithKB = false;

    }

    void Update()
    {
        if(!m_MoveWithKB)
            return;

        UpdateZoom(m_CameraMovementKBInput.z * Time.deltaTime * 4);
        Vector3 localDelta = m_Camera.orthographicSize * Time.deltaTime * new Vector3(m_CameraMovementKBInput.x, m_CameraMovementKBInput.y, 0);
        MovePosition(localDelta);
        Vector3 worldDelta = transform.localToWorldMatrix * new Vector4(localDelta.x, localDelta.y);
        m_InitMouseWorldPos += worldDelta;
        m_InitTransformWorldPos += worldDelta;
    }

    void UpdateZoom(float iZoomDelta)
    {
        m_Camera.orthographicSize -= iZoomDelta;
        m_Camera.orthographicSize = Mathf.Clamp(m_Camera.orthographicSize, m_MinZoom, m_MaxZoom);
        ClampPosition();
    }

    void StartPinch()
    {
        if(m_IsPinching)
            return;

        StopMoveCamera();

        m_IsPinching = true;
        m_PrevPinchDist = (m_InputAction.Player.PointerPosition.ReadValue<Vector2>() - m_InputAction.Player.PointerPosition1.ReadValue<Vector2>()).magnitude;
    }

    void UpdatePinch()
    {
        if(!m_IsPinching)
            return;

        float pinchDist = (m_InputAction.Player.PointerPosition.ReadValue<Vector2>() - m_InputAction.Player.PointerPosition1.ReadValue<Vector2>()).magnitude;
        UpdateZoom(pinchDist - m_PrevPinchDist);
        m_PrevPinchDist = pinchDist;
    }

    void EndPinch()
    {
        m_IsPinching = false;
        m_PrevPinchDist = 0;
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

    void MovePositionThroughDrag(InputAction.CallbackContext iCtx)
    {
        if(!m_IsDragging)
            return;

        Vector2 newMouseScreenPos = iCtx.ReadValue<Vector2>();
        transform.position = m_InitTransformWorldPos;
        Vector3 deltaPosWC = m_InitMouseWorldPos - m_Camera.ScreenToWorldPoint(newMouseScreenPos);
        if(deltaPosWC.sqrMagnitude < 0.01f)
            return;

        Vector3 localDelta = transform.worldToLocalMatrix * new Vector4(deltaPosWC.x, deltaPosWC.y, deltaPosWC.z);
        MovePosition(localDelta);
    }

    void MovePosition(Vector3 iLocalDelta)
    {
        transform.Translate(iLocalDelta, Space.Self);
        ClampPosition();
    }

    void ClampPosition()
    {
        // assuming camera plane is (x, y)
        Vector3 centerToBottomLeft = new Vector3(-m_Camera.orthographicSize * m_Camera.aspect, -m_Camera.orthographicSize, 0);

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, m_MinCamera.position.x - centerToBottomLeft.x, m_MaxCamera.position.x + centerToBottomLeft.x),
            Mathf.Clamp(transform.position.y, m_MinCamera.position.y - centerToBottomLeft.y, m_MaxCamera.position.y + centerToBottomLeft.y),
            transform.position.z);
    }

}
