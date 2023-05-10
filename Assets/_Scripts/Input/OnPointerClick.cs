using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnPointerClick : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _layers;
    [SerializeField, Tooltip("Negative value for -2XCamera Z")] private float _distance = -1f;
    [SerializeField] private UnityEvent<GameObject> _onClick;
    InputsActions a;
    private void Awake()
    {
        a = InputMaster.instance.InputAction;
        if (_distance < 0f)
            _distance = -2f * _camera.transform.position.z;
    }
    private void OnEnable()
    {
        a.Player.ClickButton.started += ctx => CheckValue();
    }
    private void OnDisable()
    {
        a.Player.ClickButton.started -= ctx => CheckValue();
    }
    void CheckValue()
    {
        Debug.Log("hit : " + a.Player.PointerPosition.ReadValue<Vector2>());
        Vector3 mousePosScreen = a.Player.PointerPosition.ReadValue<Vector2>();

        // Set the z position to the distance from the camera
        mousePosScreen.z = -_camera.transform.position.z;

        // Convert the mouse position from screen space to world space
        Vector3 mousePosWorld = _camera.ScreenToWorldPoint(mousePosScreen);

        // Check if there is any collider at the mouse position
        Collider2D collider = Physics2D.OverlapPoint(mousePosWorld,_layers);

        if (collider != null)
        {
            _onClick.Invoke(collider.gameObject);
        }
        return;
        //Ray approach
        Ray ray = _camera.ScreenPointToRay(a.Player.PointerPosition.ReadValue<Vector2>());
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, _distance, _layers);
        Debug.Log("hit : " + hit.collider);
        if (hit && hit.collider != null)
        {
            _onClick.Invoke(hit.collider.gameObject);
        }
    }
}
