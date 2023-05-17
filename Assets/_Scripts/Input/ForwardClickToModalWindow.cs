using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ForwardClickToModalWindow : MonoBehaviour
{
    [SerializeField] private ModalWindowController _window;
    public void CheckMachineClicked(Vector2 pos, GameObject go)
    {
        if (go != null && go.TryGetComponent<Machine>(out Machine machine))
        {
            _window.OpenMachineSettings(pos, machine);
        }
        else
            _window.CheckClickedOutside(pos);
    }
}
