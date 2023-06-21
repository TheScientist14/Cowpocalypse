using _Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ItemHandlerManager : Singleton<ItemHandlerManager>
{
    public GameObject m_BeltPrefab;
    private GameObject m_ItemHandlerPrefab;

    // 1 = belt, 2 = machine, 3 = merger, 4 = seller, 5 = spawner, 6 = spliter
    [SerializeField] List<GameObject> m_ItemHandlerPrefabList;

    public float speed = 0.2f;
    private InputsActions m_InputAction;
    private LineRenderer m_LineRenderer;

    [SerializeField] GameObject m_MapParentObject;

    [SerializeField] float m_MachineBaseprice;
    [SerializeField] float m_MachinePriceMultiplier;
    private int m_MachineCount;

    public int MachineCount
    {
        get => m_MachineCount;
        set => m_MachineCount = value;
    }

    [SerializeField] int m_MaxShop;

    public int MaxShop => m_MaxShop;

    private int m_ShopCount;

    public int ShopCount
    {
        get => m_ShopCount;
        set => m_ShopCount = value;
    }

    [SerializeField] float m_SpawnRate;
    [SerializeField] float m_CraftingSpeedMultiplier;

    private bool m_IsDraggingBelt = false;
    private bool m_BeltIsVertical;

    private bool m_IsInDeleteMode = false;

    [SerializeField] Camera m_Camera;
    private Grid m_WorldGridGeometry;

    public UnityEvent<bool> OnDeleteMode;

    private void Start()
    {
        m_WorldGridGeometry = GetComponent<Grid>();

        m_InputAction = InputMaster.instance.InputAction;

        m_InputAction.Player.DragBuildMode.started += _ => InitDrag();
        m_InputAction.Player.DragBuildMode.canceled += _ => EndDrag();
        m_InputAction.Player.PointerPosition.performed += _ => DuringDrag();

        m_InputAction.Player.ClickBuildMode.started += context => PlaceOrDeleteMachine();

        // Validate.onClick.AddListener(SpawnBelts);

        m_LineRenderer = GetComponent<LineRenderer>();

        Shader.SetGlobalFloat("_speed", speed);

        /*DisableBuildMode();
        EndPlaceMachine();*/
    }

    private void InitDrag()
    {
        if(m_IsDraggingBelt)
            return;

        m_IsDraggingBelt = true;
        m_LineRenderer.enabled = true;

    }

    private void EndDrag()
    {
        if(!m_IsDraggingBelt)
        {
            return;
        }
        m_IsDraggingBelt = false;

        SpawnBelts();
        m_LineRenderer.enabled = false;
        m_LineRenderer.positionCount = 0;
    }

    private void DuringDrag()
    {
        if(!m_IsDraggingBelt)
            return;

        Vector3 currentDragPosition = m_Camera.ScreenToWorldPoint(m_InputAction.Player.PointerPosition.ReadValue<Vector2>());
        currentDragPosition = PlaceInGrid(currentDragPosition);

        if(m_LineRenderer.positionCount <= 0)
        {
            m_LineRenderer.positionCount = 2;
            m_LineRenderer.SetPosition(0, new Vector3(currentDragPosition.x, currentDragPosition.y, 0));
        }

        m_LineRenderer.SetPosition(1, new Vector3(currentDragPosition.x, currentDragPosition.y, 0));

        MakeFirstLine(currentDragPosition);
    }

    private void MakeLShape(Vector3 end)
    {
        end = PlaceInGrid(end);
        m_LineRenderer.positionCount = 3;
        if(m_BeltIsVertical)
        {
            m_LineRenderer.SetPosition(1, new Vector3(m_LineRenderer.GetPosition(0).x, end.y, 0));
            m_LineRenderer.SetPosition(2, new Vector3(end.x, end.y, 0));
        }
        else
        {
            m_LineRenderer.SetPosition(1, new Vector3(end.x, m_LineRenderer.GetPosition(0).y, 0));
            m_LineRenderer.SetPosition(2, new Vector3(end.x, end.y, 0));
        }
    }

    private void MakeFirstLine(Vector3 end)
    {
        Vector3 realEnd;
        if(Mathf.Abs(Mathf.Abs(end.x) - Mathf.Abs(m_LineRenderer.GetPosition(0).x)) > Mathf.Abs(Mathf.Abs(end.y) - Mathf.Abs(m_LineRenderer.GetPosition(0).y)))
        {
            m_BeltIsVertical = false;
            realEnd = new Vector3(end.x, m_LineRenderer.GetPosition(0).y, 0);
        }
        else
        {
            m_BeltIsVertical = true;
            realEnd = new Vector3(m_LineRenderer.GetPosition(0).x, end.y, 0);
        }
        //realEnd = PlaceInGrid(realEnd);
        m_LineRenderer.SetPosition(1, realEnd);
    }

    private Vector3 PlaceInGrid(Vector3 position)
    {
        Vector3Int newPosition = m_WorldGridGeometry.WorldToCell(position);
        position = m_WorldGridGeometry.GetCellCenterWorld(newPosition);
        return position;
    }

    private void SpawnBelts()
    {
        if(m_LineRenderer.GetPosition(0) == m_LineRenderer.GetPosition(1))
            return;
        int x = (int)m_LineRenderer.GetPosition(0).x;
        int y = (int)m_LineRenderer.GetPosition(0).y;
        float cellSize = m_WorldGridGeometry.cellSize.x;
        GameObject spawnedBelt;

        spawnedBelt = SpawnBelt(m_LineRenderer.GetPosition(0), m_LineRenderer.GetPosition(1));

        while(spawnedBelt.transform.position != m_LineRenderer.GetPosition(1) - spawnedBelt.transform.up * cellSize)
            spawnedBelt = SpawnBelt(spawnedBelt.transform.position + spawnedBelt.transform.up * cellSize, m_LineRenderer.GetPosition(1));

        StartCoroutine(DelayDisableBuildMode());
    }

    IEnumerator DelayDisableBuildMode()
    {
        yield return new WaitForEndOfFrame();
        EndPlaceBelt();
    }

    private GameObject SpawnBelt(Vector3 position, Vector3 direction)
    {
        GameObject spawnedBeltGameObject = Instantiate(m_BeltPrefab, position, Quaternion.identity, m_MapParentObject.transform);
        spawnedBeltGameObject.transform.LookAt(direction);

        if(!m_BeltIsVertical)
            spawnedBeltGameObject.transform.Rotate(Vector3.forward, 90, Space.Self);
        spawnedBeltGameObject.transform.Rotate(Vector3.right, 90, Space.Self);
        if(spawnedBeltGameObject.transform.position == direction)
            spawnedBeltGameObject.transform.Rotate(Vector3.left, 90, Space.Self);

        IItemHandler spawnedBelt = spawnedBeltGameObject.GetComponent<IItemHandler>();
        Assert.IsNotNull(spawnedBelt);
        Vector3Int cellPos = m_WorldGridGeometry.WorldToCell(position);
        if(!NewGridManager.instance.SetItemHandlerAt(new Vector2Int(cellPos.x, cellPos.y), spawnedBelt, true))
        {
            Destroy(spawnedBeltGameObject);
            return null;
        }

        return spawnedBeltGameObject;
    }

    public void StartPlaceBelt()
    {
        InputStateMachine.instance.SetState(new BuildBeltState());
    }

    private void EndPlaceBelt()
    {
        InputStateMachine.instance.SetState(new FreeViewState());
    }

    private void PlaceSimpleMachine()
    {
        Assert.IsNotNull(m_ItemHandlerPrefab);
        Vector3 mouseWorldPos = m_Camera.ScreenToWorldPoint(m_InputAction.Player.PointerPosition.ReadValue<Vector2>());
        Vector3 machineWorldPosInGrid = PlaceInGrid(mouseWorldPos);
        machineWorldPosInGrid.z = 0;
        SpawnMachine(machineWorldPosInGrid, Vector3.up);
        EndPlaceMachine();
    }

    public int GetMachinePrice()
    {
        return (int)(m_MachineBaseprice * Mathf.Pow(m_MachinePriceMultiplier, m_MachineCount));
    }

    private GameObject SpawnMachine(Vector3 position, Vector3 direction)
    {
        int machinePrice = GetMachinePrice();
        if(m_ItemHandlerPrefab.GetComponent<NewMachine>())
        {
            if(Wallet.instance.Money >= machinePrice)
            {
                Wallet.instance.Money -= machinePrice;
                m_MachineCount++;
            }
            else
                return null;
        }
        else if(m_ItemHandlerPrefab.GetComponent<NewSeller>())
        {
            if(m_ShopCount >= m_MaxShop)
                return null;
            m_ShopCount++;
        }
        GameObject spawnedItemHandlerGameObject = Instantiate(m_ItemHandlerPrefab, position, Quaternion.identity, m_MapParentObject.transform);

        IItemHandler spawnedItemHandler = spawnedItemHandlerGameObject.GetComponent<IItemHandler>();
        Assert.IsNotNull(spawnedItemHandler);
        Vector3Int cellPos = m_WorldGridGeometry.WorldToCell(position);

        if(!NewGridManager.instance.SetItemHandlerAt(new Vector2Int(cellPos.x, cellPos.y), spawnedItemHandler, true))
        {
            Destroy(spawnedItemHandlerGameObject);
            return null;
        }

        return spawnedItemHandlerGameObject;
    }

    public void StartPlaceMachine(GameObject iMachineToPlace)
    {
        m_ItemHandlerPrefab = iMachineToPlace;
        InputStateMachine.instance.SetState(new BuildMachineState());
        // SetUpCallbacks();
    }
    public void EndPlaceMachine()
    {
        InputStateMachine.instance.SetState(new FreeViewState());
    }

    private void PlaceOrDeleteMachine()
    {
        if(m_IsInDeleteMode)
            DeleteMachineOnCursor();
        else
            PlaceSimpleMachine();
    }

    private void DeleteMachineOnCursor()
    {
        Vector3 mouseWorldPos = m_Camera.ScreenToWorldPoint(m_InputAction.Player.PointerPosition.ReadValue<Vector2>());
        Vector3Int cellPos = m_WorldGridGeometry.WorldToCell(mouseWorldPos);

        NewGridManager.instance.SetItemHandlerAt(new Vector2Int(cellPos.x, cellPos.y), null, true); // do not delete spawners
    }

    public void SwitchDeleteMode()
    {
        InputStateMachine.instance.SetState(m_IsInDeleteMode ? new FreeViewState() : new DeleteState());
        m_IsInDeleteMode = !m_IsInDeleteMode;
        OnDeleteMode.Invoke(m_IsInDeleteMode);
    }

    public void RotateBelt(GameObject go, int machineType)
    {
        Quaternion savedRotation = go.transform.rotation;
        Vector3 savedPosition = go.transform.position;
        Transform savedParent = go.transform.parent;

        GameObject newBeltPrefab = m_ItemHandlerPrefabList[machineType];

        // Instantiate a new belt object with the saved rotation plus an additional 90 degrees rotation on the Z-axis
        GameObject newBeltObject = Instantiate(newBeltPrefab, savedPosition, savedRotation * Quaternion.Euler(0f, 0f, 90f));

        // Optionally, you can parent the new belt to the same transform as the previous belt
        newBeltObject.transform.parent = savedParent;

        // Destroy the previous belt object and assign the new belt to the grid
        Vector3Int GridPos = m_WorldGridGeometry.WorldToCell(savedPosition);
        bool isAssigned = NewGridManager.instance.SetItemHandlerAt(new Vector2Int(GridPos.x, GridPos.y), newBeltObject.GetComponent<IItemHandler>(), true);
        if(!isAssigned)
            Destroy(newBeltObject);
    }

    public void UpdateStat(Stat stat)
    {
        if(stat.StatData.Name == "Belt speed")
        {
            speed = stat.Value;
            Shader.SetGlobalFloat("_speed", speed);
            Debug.Log(Shader.GetGlobalFloat("_speed"));
        }

        else if(stat.StatData.Name == "Extract speed")
            m_SpawnRate = stat.Value;
        else if(stat.StatData.Name == "Craft Speed")
            m_CraftingSpeedMultiplier = stat.Value;


    }

    public float GetSpawnRate()
    {
        return m_SpawnRate;
    }

    public float GetCraftingSpeedMultiplier()
    {
        return m_CraftingSpeedMultiplier;
    }

    // Should only be called from the OnDestroy of NewSeller
    public void RemoveOneShop()
    {
        m_ShopCount--;
    }

    // Should only be called from the OnDestroy of NewMachine
    public void RemoveOneMachine()
    {
        m_MachineCount--;
        Wallet.instance.Money += GetMachinePrice();
    }
}
