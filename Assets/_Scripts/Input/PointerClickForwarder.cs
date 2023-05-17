using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PointerClickForwarder : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _layers;
    [SerializeField, Tooltip("Negative value for -2XCamera Z")] private float _distance = -1f;
    [SerializeField] private UnityEvent<Vector2, GameObject> _onClick;
    InputsActions _inputAction;
    private void Awake()
    {
        _inputAction = InputMaster.instance.InputAction;
        if (_distance < 0f)
            _distance = -2f * _camera.transform.position.z;
    }
    private void OnEnable()
    {
        _inputAction.Player.ClickButton.started += ctx => CheckValue(_inputAction.Player.PointerPosition.ReadValue<Vector2>());
    }
    private void OnDisable()
    {
        _inputAction.Player.ClickButton.started -= ctx => CheckValue(_inputAction.Player.PointerPosition.ReadValue<Vector2>());
    }
    void CheckValue(Vector3 mousePosition)
    {
        // Set the z position to the distance from the camera
        mousePosition.z = -_camera.transform.position.z;

        // Convert the mouse position from screen space to world space
        Vector3 mousePosWorld = _camera.ScreenToWorldPoint(mousePosition);

        // Check if there is any collider at the mouse position
        Collider2D collider = Physics2D.OverlapPoint(mousePosWorld, _layers);
        _onClick.Invoke(mousePosition, collider?.gameObject);
        //Ray approach
        /*Ray ray = _camera.ScreenPointToRay(_inputAction.Player.PointerPosition.ReadValue<Vector2>());
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, _distance, _layers);
        Debug.Log("hit : " + hit.collider);
        if (hit && hit.collider != null)
        {
            _onClick.Invoke(mousePosition,hit.collider.gameObject);
        }*/
    }
}
