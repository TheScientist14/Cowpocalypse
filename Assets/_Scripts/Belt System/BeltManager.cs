using _Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class BeltManager : Singleton<BeltManager>
{
    public InputAction DebugBeltSpawn;

    public GameObject BeltPrefab;

    public static BeltManager Instance;
    public float speed = 2f;
    private InputsActions m_InputAction;
    private LineRenderer lineRenderer;

    private int DraggingPhase = 0;
    private bool BeltIsVertical;

    [SerializeField] private Camera m_Camera;
    private Grid GameGrid;

    private void Start()
    {
        //Debug-----------------
        DebugBeltSpawn.started += context => SpawnBelts();
        DebugBeltSpawn.Enable();
        //-----------------------

        GameGrid = GetComponent<Grid>();

        m_InputAction = InputMaster.instance.InputAction;

        m_InputAction.Player.DragBuildMode.started += context => InitDrag();
        m_InputAction.Player.DragBuildMode.canceled += ctx => EndDrag(ctx.ReadValue<Vector2>());
        m_InputAction.Player.DragBuildMode.performed += context => DuringDrag();

        m_InputAction.Player.ClickBuildMode.started += context => InitDrag();

        lineRenderer = GetComponent<LineRenderer>();

        DisableBuildMode();
    }

    private void InitDrag()
    {
        if(DraggingPhase >= 2)
            DraggingPhase = 0;

        lineRenderer.enabled = true;
        DraggingPhase++;
        Vector3 m_InitMouseWorldPos = m_Camera.ScreenToWorldPoint(m_InputAction.Player.PointerPosition.ReadValue<Vector2>());
        //Instantiate(TestPrefab, new Vector3(m_InitMouseWorldPos.x, m_InitMouseWorldPos.y, 0), Quaternion.Euler(Vector3.one));
        if(DraggingPhase == 1)
        {
            m_InitMouseWorldPos = PlaceInGrid(m_InitMouseWorldPos);
            lineRenderer.SetPosition(0, new Vector3(m_InitMouseWorldPos.x, m_InitMouseWorldPos.y, 0));
            lineRenderer.SetPosition(1, new Vector3(m_InitMouseWorldPos.x, m_InitMouseWorldPos.y, 0));
            lineRenderer.positionCount = 2;
        }
    }

    private void EndDrag(Vector2 endPosition)
    {

    }

    private void DuringDrag()
    {
        Vector3 currentDragPosition = m_Camera.ScreenToWorldPoint(m_InputAction.Player.PointerPosition.ReadValue<Vector2>());
        currentDragPosition = PlaceInGrid(currentDragPosition);
        if (DraggingPhase > 2)
            DraggingPhase = 1;
        if(DraggingPhase == 1)
            MakeFirstLine(currentDragPosition);
        else if(DraggingPhase == 2)
            MakeLShape(currentDragPosition);
    }

    private void MakeLShape(Vector3 end)
    {
        end = PlaceInGrid(end);
        lineRenderer.positionCount = 3;
        if(BeltIsVertical)
        {
            lineRenderer.SetPosition(1, new Vector3(lineRenderer.GetPosition(0).x, end.y, 0));
            lineRenderer.SetPosition(2, new Vector3(end.x, end.y, 0));
        }
        else
        {
            lineRenderer.SetPosition(1, new Vector3(end.x, lineRenderer.GetPosition(0).y, 0));
            lineRenderer.SetPosition(2, new Vector3(end.x, end.y, 0));
        }
    }

    private void MakeFirstLine(Vector3 end)
    {
        Vector3 realEnd;
        if(Mathf.Abs(Mathf.Abs(end.x) -  Mathf.Abs(lineRenderer.GetPosition(0).x)) > Mathf.Abs(Mathf.Abs(end.y) - Mathf.Abs(lineRenderer.GetPosition(0).y)))
        {
            BeltIsVertical = false;
            realEnd = new Vector3(end.x, lineRenderer.GetPosition(0).y, 0);
        }
        else
        {
            BeltIsVertical = true;
            realEnd = new Vector3(lineRenderer.GetPosition(0).x, end.y, 0);
        }
        //realEnd = PlaceInGrid(realEnd);
        lineRenderer.SetPosition(1, realEnd);
    }

    private Vector3 PlaceInGrid(Vector3 position)
    {
        Vector3Int newPosition = GameGrid.WorldToCell(position);
        position = GameGrid.GetCellCenterWorld(newPosition);
        return position;
    }

    private void SpawnBelts()
    {
        int x = (int)lineRenderer.GetPosition(0).x;
        int y = (int)lineRenderer.GetPosition(0).y;
        float cellSize = GameGrid.cellSize.x;
        GameObject spawnedBelt;

        spawnedBelt = SpawnBelt(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1));

        while(spawnedBelt.transform.position != lineRenderer.GetPosition(1) - spawnedBelt.transform.up * cellSize)
            spawnedBelt = SpawnBelt(spawnedBelt.transform.position + spawnedBelt.transform.up * cellSize, lineRenderer.GetPosition(1));
        print(DraggingPhase);
        if(DraggingPhase == 2)
        {
            BeltIsVertical = !BeltIsVertical;
            while (spawnedBelt.transform.position != lineRenderer.GetPosition(2))
            {
                spawnedBelt = SpawnBelt(spawnedBelt.transform.position + spawnedBelt.transform.up * cellSize, lineRenderer.GetPosition(2));
            }
            //Prevent bug
            Destroy(spawnedBelt);
        }
    }

    private GameObject SpawnBelt(Vector3 position, Vector3 direction)
    {
        GameObject belt = Instantiate(BeltPrefab, position, Quaternion.identity);
        belt.transform.LookAt(direction);
        
        if(!BeltIsVertical)
            belt.transform.Rotate(Vector3.forward, 90, Space.Self);
        belt.transform.Rotate(Vector3.right, 90, Space.Self);
        if(belt.transform.position == direction)
            belt.transform.Rotate(Vector3.left, 90, Space.Self);
        DisableBuildMode();
        return belt;
    }

    public void EnableBuildMode()
    {
        StateMachine.instance.SetState(new BuildState());
    }

    private void DisableBuildMode()
    {
        StateMachine.instance.SetState(new FreeViewState());
    }
}