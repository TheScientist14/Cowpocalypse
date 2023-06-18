using _Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class BeltManager : Singleton<BeltManager>
{
    public GameObject BeltPrefab;
    private GameObject OtherMachinePrefab;

    [SerializeField]
    private List<GameObject> BeltList; // 1 = belt, 2 = machine, 3 = merger, 4 = seller, 5 = spawner, 6 = spliter

    public float speed = 0.2f;
    private InputsActions m_InputAction;
    private LineRenderer lineRenderer;

    [SerializeField] private GameObject mapParentObject;

    [SerializeField] private float MachineBaseprice;
    [SerializeField] private float MachinePriceMultiplier;
    private int MachineCount;
    [SerializeField] private int maxShop;

    public int MaxShop => maxShop;

    private int shopCount;

    public int ShopCount => shopCount;

    [SerializeField] float SpawnRate;
    [SerializeField] private float CraftingSpeedMultiplier;

    private bool IsDraggingBelt = false;
    private bool BeltIsVertical;

    private bool IsInDeleteMode = false;

    [SerializeField] private Camera m_Camera;
    private Grid GameGrid;

    public UnityEvent<bool> deleteMode;

    private void Start()
    {
        GameGrid = GetComponent<Grid>();

        m_InputAction = InputMaster.instance.InputAction;

        m_InputAction.Player.DragBuildMode.started += _ => InitDrag();
        m_InputAction.Player.DragBuildMode.canceled += _ => EndDrag();
        m_InputAction.Player.PointerPosition.performed += _ => DuringDrag();

        m_InputAction.Player.ClickBuildMode.started += context => PlaceOrDeleteMachine();

        // Validate.onClick.AddListener(SpawnBelts);

        lineRenderer = GetComponent<LineRenderer>();
        
        Shader.SetGlobalFloat("_speed", speed);

        /*DisableBuildMode();
        EndPlaceMachine();*/
    }

    private void InitDrag()
    {
        if(IsDraggingBelt)
            return;

        IsDraggingBelt = true;
        lineRenderer.enabled = true;
        Vector3 m_InitMouseWorldPos = m_Camera.ScreenToWorldPoint(m_InputAction.Player.PointerPosition.ReadValue<Vector2>());
        m_InitMouseWorldPos = PlaceInGrid(m_InitMouseWorldPos);
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(m_InitMouseWorldPos.x, m_InitMouseWorldPos.y, 0));
        lineRenderer.SetPosition(1, new Vector3(m_InitMouseWorldPos.x, m_InitMouseWorldPos.y, 0));


    }

    private void EndDrag()
    {
        IsDraggingBelt = false;

        SpawnBelts();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
    }

    private void DuringDrag()
    {
        if(!IsDraggingBelt)
            return;

        Vector3 currentDragPosition = m_Camera.ScreenToWorldPoint(m_InputAction.Player.PointerPosition.ReadValue<Vector2>());
        currentDragPosition = PlaceInGrid(currentDragPosition);
        MakeFirstLine(currentDragPosition);
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
        if(Mathf.Abs(Mathf.Abs(end.x) - Mathf.Abs(lineRenderer.GetPosition(0).x)) > Mathf.Abs(Mathf.Abs(end.y) - Mathf.Abs(lineRenderer.GetPosition(0).y)))
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
        if(lineRenderer.GetPosition(0) == lineRenderer.GetPosition(1))
            return;
        int x = (int)lineRenderer.GetPosition(0).x;
        int y = (int)lineRenderer.GetPosition(0).y;
        float cellSize = GameGrid.cellSize.x;
        GameObject spawnedBelt;

        spawnedBelt = SpawnBelt(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1));

        while(spawnedBelt.transform.position != lineRenderer.GetPosition(1) - spawnedBelt.transform.up * cellSize)
            spawnedBelt = SpawnBelt(spawnedBelt.transform.position + spawnedBelt.transform.up * cellSize, lineRenderer.GetPosition(1));

        StartCoroutine(DelayDisableBuildMode());
    }

    IEnumerator DelayDisableBuildMode()
    {
        yield return new WaitForEndOfFrame();
        DisableBuildMode();
    }

    private GameObject SpawnBelt(Vector3 position, Vector3 direction)
    {
        GameObject belt = Instantiate(BeltPrefab, position, Quaternion.identity, mapParentObject.transform);
        belt.transform.LookAt(direction);

        if(!BeltIsVertical)
            belt.transform.Rotate(Vector3.forward, 90, Space.Self);
        belt.transform.Rotate(Vector3.right, 90, Space.Self);
        if(belt.transform.position == direction)
            belt.transform.Rotate(Vector3.left, 90, Space.Self);

        Belt b = belt.GetComponent<Belt>();
        Assert.IsNotNull(b);
        Vector3Int cellPos = GameGrid.WorldToCell(position);
        if(!GridManager.instance.SetBeltAt(new Vector2Int(cellPos.x, cellPos.y), b, true))
        {
            Destroy(belt);
            return null;
        }

        return belt;
    }

    public void EnableBuildMode()
    {
        InputStateMachine.instance.SetState(new BuildBeltState());
    }

    private void DisableBuildMode()
    {
        InputStateMachine.instance.SetState(new FreeViewState());
    }

    private void PlaceSimpleMachine()
    {
        Assert.IsNotNull(OtherMachinePrefab);
        Vector3 mouseWorldPos = m_Camera.ScreenToWorldPoint(m_InputAction.Player.PointerPosition.ReadValue<Vector2>());
        Vector3 machineWorldPosInGrid = PlaceInGrid(mouseWorldPos);
        machineWorldPosInGrid.z = 0;
        SpawnMachine(machineWorldPosInGrid, Vector3.up);
        EndPlaceMachine();
    }

    public int GetMachinePrice()
    {
        return (int)(MachineBaseprice *  Mathf.Pow(MachinePriceMultiplier,MachineCount));
    }
    
    private GameObject SpawnMachine(Vector3 position, Vector3 direction)
    {
        int machinePrice = GetMachinePrice();
        if(OtherMachinePrefab.GetComponent<Machine>())
        {
            if(Wallet.instance.Money >= machinePrice)
            {
                Wallet.instance.Money -= machinePrice;
                MachineCount++;
            }
            else
                return null;
        }
        else if(OtherMachinePrefab.GetComponent<Seller>())
        {
            if(shopCount >= maxShop)
                return null;
            shopCount++;
        }
        GameObject belt = Instantiate(OtherMachinePrefab, position, Quaternion.identity, mapParentObject.transform);

        Belt b = belt.GetComponent<Belt>();
        Assert.IsNotNull(b);
        Vector3Int cellPos = GameGrid.WorldToCell(position);

        if(!GridManager.instance.SetBeltAt(new Vector2Int(cellPos.x, cellPos.y), b, true))
        {
            Destroy(belt);
            return null;
        }

        return belt;
    }

    public void StartPlaceMachine(GameObject iMachineToPlace)
    {
        OtherMachinePrefab = iMachineToPlace;
        InputStateMachine.instance.SetState(new BuildMachineState());
        // SetUpCallbacks();
    }
    public void EndPlaceMachine()
    {
        InputStateMachine.instance.SetState(new FreeViewState());
    }

    private void PlaceOrDeleteMachine()
    {
        if(IsInDeleteMode)
            DeleteMachineOnCursor();
        else
            PlaceSimpleMachine();
    }

    private void DeleteMachineOnCursor()
    {
        Vector3 mouseWorldPos = m_Camera.ScreenToWorldPoint(m_InputAction.Player.PointerPosition.ReadValue<Vector2>());
        Vector3Int cellPos = GameGrid.WorldToCell(mouseWorldPos);

        GridManager.instance.SetBeltAt(new Vector2Int(cellPos.x, cellPos.y), null, true); // do not delete spawners
    }

    public void SwitchDeleteMode()
    {
        InputStateMachine.instance.SetState(IsInDeleteMode ? new FreeViewState() : new DeleteState());
        IsInDeleteMode = !IsInDeleteMode;
        deleteMode.Invoke(IsInDeleteMode);
    }

    public void RotateBelt(GameObject go, int machineType)
    {
        Quaternion savedRotation = go.transform.rotation;
        Vector3 savedPosition = go.transform.position;
        Transform savedParent = go.transform.parent;

        GameObject newBeltPrefab = BeltList[machineType];

        // Instantiate a new belt object with the saved rotation plus an additional 90 degrees rotation on the Z-axis
        GameObject newBeltObject = Instantiate(newBeltPrefab, savedPosition, savedRotation * Quaternion.Euler(0f, 0f, 90f));

        // Optionally, you can parent the new belt to the same transform as the previous belt
        newBeltObject.transform.parent = savedParent;

        // Destroy the previous belt object and assign the new belt to the grid
        Vector3Int GridPos = GameGrid.WorldToCell(savedPosition);
        bool isAssigned = GridManager.instance.SetBeltAt(new Vector2Int(GridPos.x, GridPos.y), newBeltObject.GetComponent<Belt>(), true);
        if(!isAssigned)
        {
            Destroy(newBeltObject);
        }
    }

    public void UpdateStat(Stat stat)
    {
        if (stat.StatData.Name == "Belt speed")
        {
            speed = stat.Value;
            Shader.SetGlobalFloat("_speed", speed);
            Debug.Log(Shader.GetGlobalFloat("_speed"));
        }
            
        else if(stat.StatData.Name == "Extract speed")
            SpawnRate = stat.Value;
        else if(stat.StatData.Name == "Craft Speed")
            CraftingSpeedMultiplier = stat.Value;


    }

    public float GetSpawnRate()
    {
        return SpawnRate;
    }

    public float GetCraftingSpeedMultiplier()
    {
        return CraftingSpeedMultiplier;
    }

    public void RemoveOneShop()
    {
        shopCount--;
    }

    public void RemoveOneMachine()
    {
        MachineCount--;
        Wallet.instance.Money += GetMachinePrice();
    }
}
