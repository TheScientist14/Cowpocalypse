using UnityEngine;

public class ForwardClickToModalWindow : MonoBehaviour
{
    [SerializeField] private ModalWindowController _window;
    [SerializeField] private BeltManager _beltManager;
    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f; // Adjust this value to define the double-click speed

    public void CheckMachineClicked(Vector2 pos, GameObject go)
    {
        if (go != null)
        {
            Belt belt = go.GetComponent<Belt>();
            if (belt != null)
            {
                int machineType = TestMachineType(go);
                if (machineType == -1)
                {
                    return;
                }

                if (IsDoubleClick())
                {
                    Debug.Log(go.name);
                    // Double-click detected on the Belt
                    _beltManager.RotateBelt(go, machineType); // Rotate belt
                }
                return;
            }

            Machine machine = go.GetComponent<Machine>();
            if (machine != null)
            {
                _window.OpenMachineSettings(pos, machine);
                return;
            }
        }

        // If no Belt or Machine was clicked, treat it as a click outside
        _window.CheckClickedOutside(pos);
    }

    private bool IsDoubleClick()
    {
        bool isDoubleClick = false;

        if (Time.time - lastClickTime < doubleClickThreshold)
        {
            isDoubleClick = true;
        }

        lastClickTime = Time.time;

        return isDoubleClick;
    }

    private int TestMachineType(GameObject go)
    {
        int machineType =
            go.GetComponent<Machine>() != null ? 1 :
            go.GetComponent<Merger>() != null ? 2 :
            go.GetComponent<Seller>() != null ? 3 :
            go.GetComponent<Splitter>() != null ? 4 :
            go.GetComponent<Spawner>() != null ? -1 :
            0;

        return machineType;
    }

}
