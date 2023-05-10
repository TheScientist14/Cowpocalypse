using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMachineClick : MonoBehaviour
{
    [SerializeField] private ModalWindowController _window;
    public void MachineClicked(GameObject go)
    {
        if (go.TryGetComponent<Machine>(out Machine machine))
        {
            _window.OpenMachineSettings(machine);
        }
    }
}
