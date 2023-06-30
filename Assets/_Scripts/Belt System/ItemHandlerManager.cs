using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class ItemHandlerManager : Singleton<ItemHandlerManager>
{
    #region Properties
    [SerializeField] GameObject m_MapParentObject;
    [SerializeField] Camera m_Camera;
    private InputsActions m_InputAction;
    private Grid m_WorldGridGeometry;

    [Header("BeltPlacement")]
    [SerializeField] float m_BeltPlacementArrowExtraLength = 0.25f;
    [SerializeField] float m_BeltPlacementArrowSpaceBetweenTips = 0.25f;
    public GameObject m_BeltPrefab;
    private LineRenderer m_LineRenderer;
    private Vector3 m_FirstPos, m_SecondPos;
    private bool m_IsDraggingBelt = false;

    [Header("MachinePlacement")]
    // 0 = belt, 1 = machine, 2 = merger, 3 = seller, 4 = spliter, 5 = spawner
    [SerializeField] List<GameObject> m_ItemHandlerPrefabList;
    private GameObject m_ItemHandlerPrefab;

    [Header("Stats")]
    [SerializeField] int m_MachineBaseprice;
    [SerializeField] float m_MachinePriceMultiplier;
    [SerializeField] int m_SpawnerBasePrice;
    [SerializeField] int m_MaxShop;

    [HideInInspector]
    public UnityEvent OnBeltModeChange;
    #endregion Properties

    private void Start()
    {
        m_WorldGridGeometry = GetComponent<Grid>();

        m_InputAction = InputMaster.instance.InputAction;

        InputMaster.instance.OnStartDragBuildMode.AddListener(InitDrag);
        m_InputAction.Player.DragBuildMode.canceled += _ => EndDrag();
        m_InputAction.Player.PointerPosition.performed += _ => DuringDrag();

        InputMaster.instance.OnStartClickBuildMode.AddListener(PlaceOrDeleteMachine);

        m_LineRenderer = GetComponent<LineRenderer>();
    }

    private Vector3 PlaceInGrid(Vector3 position)
    {
        Vector3Int newPosition = m_WorldGridGeometry.WorldToCell(position);
        position = m_WorldGridGeometry.GetCellCenterWorld(newPosition);
        return position;
    }

    ////////////////////////
    // Placing belts
    ////////////////////////

    private void InitDrag()
    {
        if(m_IsDraggingBelt)
            return;

        m_IsDraggingBelt = true;
        m_LineRenderer.enabled = true;
        m_LineRenderer.positionCount = 0;
        m_FirstPos = m_Camera.ScreenToWorldPoint(m_InputAction.Player.PointerPosition.ReadValue<Vector2>());
        m_SecondPos = m_FirstPos;
    }

    private void EndDrag()
    {
        if(!m_IsDraggingBelt)
            return;

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

        if(m_LineRenderer.positionCount <= 0)
        {
            m_FirstPos = currentDragPosition;
            m_LineRenderer.positionCount = 5; // we now render an arrow at the end
            Vector3 currentDragPositionInGrid = PlaceInGrid(currentDragPosition);
            m_LineRenderer.SetPosition(0, new Vector3(currentDragPositionInGrid.x, currentDragPositionInGrid.y, 0));
        }

        m_SecondPos = currentDragPosition;

        RenderBeltPlacementLine();
    }

    // draw a straight line from FirstPos to SecondPos parallel to x or y axis
    // with an arrow at the end
    private void RenderBeltPlacementLine()
    {
        Vector3 realEnd;
        Vector3 arrowTip1, arrowTip2;

        Vector3 secondPosInGrid = PlaceInGrid(m_SecondPos);
        if(Mathf.Abs(m_SecondPos.x - m_FirstPos.x) >= Mathf.Abs(m_SecondPos.y - m_FirstPos.y)) // horizontal
        {
            if(m_FirstPos.x <= m_SecondPos.x) // right ; default if FirstPos == SecondPos
            {
                realEnd = new Vector3(secondPosInGrid.x + m_BeltPlacementArrowExtraLength, m_LineRenderer.GetPosition(0).y, 0);

                arrowTip1 = new Vector3(realEnd.x - m_BeltPlacementArrowSpaceBetweenTips / 2,
                                        realEnd.y - m_BeltPlacementArrowSpaceBetweenTips / 2,
                                        0);
                arrowTip2 = new Vector3(arrowTip1.x, arrowTip1.y + m_BeltPlacementArrowSpaceBetweenTips, 0);
            }
            else // left
            {
                realEnd = new Vector3(secondPosInGrid.x - m_BeltPlacementArrowExtraLength, m_LineRenderer.GetPosition(0).y, 0);

                arrowTip1 = new Vector3(realEnd.x + m_BeltPlacementArrowSpaceBetweenTips / 2,
                                        realEnd.y - m_BeltPlacementArrowSpaceBetweenTips / 2,
                                        0);
                arrowTip2 = new Vector3(arrowTip1.x, arrowTip1.y + m_BeltPlacementArrowSpaceBetweenTips, 0);
            }
        }
        else // vertical
        {
            if(m_FirstPos.y <= m_SecondPos.y) // up
            {
                realEnd = new Vector3(m_LineRenderer.GetPosition(0).x, secondPosInGrid.y + m_BeltPlacementArrowExtraLength, 0);

                arrowTip1 = new Vector3(realEnd.x + m_BeltPlacementArrowSpaceBetweenTips / 2,
                                        realEnd.y - m_BeltPlacementArrowSpaceBetweenTips / 2,
                                        0);
                arrowTip2 = new Vector3(arrowTip1.x - m_BeltPlacementArrowSpaceBetweenTips, arrowTip1.y, 0);
            }
            else // down
            {
                realEnd = new Vector3(m_LineRenderer.GetPosition(0).x, secondPosInGrid.y - m_BeltPlacementArrowExtraLength, 0);

                arrowTip1 = new Vector3(realEnd.x + m_BeltPlacementArrowSpaceBetweenTips / 2,
                                        realEnd.y + m_BeltPlacementArrowSpaceBetweenTips / 2,
                                        0);
                arrowTip2 = new Vector3(arrowTip1.x - m_BeltPlacementArrowSpaceBetweenTips, arrowTip1.y, 0);
            }
        }

        m_LineRenderer.SetPosition(1, realEnd);
        m_LineRenderer.SetPosition(2, arrowTip1);
        m_LineRenderer.SetPosition(3, realEnd);
        m_LineRenderer.SetPosition(4, arrowTip2);
    }

    private void SpawnBelts()
    {
        Vector3Int firstGridPos = m_WorldGridGeometry.WorldToCell(m_FirstPos);
        firstGridPos.z = 0;
        Vector3Int secondGridPos = m_WorldGridGeometry.WorldToCell(m_SecondPos);
        secondGridPos.z = 0;
        Vector3Int delta;
        float beltAngle;

        if(Mathf.Abs(m_SecondPos.x - m_FirstPos.x) >= Mathf.Abs(m_SecondPos.y - m_FirstPos.y)) // horizontal
        {
            secondGridPos = new Vector3Int(secondGridPos.x, firstGridPos.y, 0);
            if(m_FirstPos.x <= m_SecondPos.x) // right ; default if FirstPos == SecondPos
            {
                beltAngle = -90;
                delta = Vector3Int.right;
            }
            else // left
            {
                beltAngle = 90;
                delta = Vector3Int.left;
            }
        }
        else // vertical
        {
            secondGridPos = new Vector3Int(firstGridPos.x, secondGridPos.y, 0);
            if(m_FirstPos.y <= m_SecondPos.y) // up
            {
                beltAngle = 0;
                delta = Vector3Int.up;
            }
            else // down
            {
                beltAngle = 180;
                delta = Vector3Int.down;
            }
        }

        SpawnBelt(firstGridPos, beltAngle);
        if(firstGridPos != secondGridPos)
        {
            for(Vector3Int beltGridPos = firstGridPos; beltGridPos - delta != secondGridPos; beltGridPos += delta)
                SpawnBelt(beltGridPos, beltAngle);
        }
    }

    private GameObject SpawnBelt(Vector3Int iGridPos, float iAngle)
    {
        IItemHandler itemHandler = m_BeltPrefab.GetComponent<IItemHandler>();
        if(itemHandler == null)
            return null;
        TerrainType terrainType = MapGenerator.instance.GetTileType(iGridPos);
        if(!itemHandler.CanBePlacedOn(terrainType))
            return null;

        Vector3 pos = m_WorldGridGeometry.GetCellCenterWorld(iGridPos);
        pos.z = 0;
        Quaternion rot = Quaternion.Euler(0, 0, iAngle);
        GameObject spawnedBeltGameObject = Instantiate(m_BeltPrefab, pos, rot, m_MapParentObject.transform);

        IItemHandler spawnedBelt = spawnedBeltGameObject.GetComponent<IItemHandler>();
        Assert.IsNotNull(spawnedBelt);
        if(!GridManager.instance.SetItemHandlerAt(new Vector2Int(iGridPos.x, iGridPos.y), spawnedBelt, true))
        {
            Destroy(spawnedBeltGameObject);
            return null;
        }

        return spawnedBeltGameObject;
    }

    ////////////////////////
    // Placing/deleting/rotating machines
    ////////////////////////

    private void PlaceSimpleMachine()
    {
        Assert.IsNotNull(m_ItemHandlerPrefab);
        Vector3 mouseWorldPos = m_Camera.ScreenToWorldPoint(m_InputAction.Player.PointerPosition.ReadValue<Vector2>());
        Vector3 machineWorldPosInGrid = PlaceInGrid(mouseWorldPos);
        machineWorldPosInGrid.z = 0;

        SpawnMachine(machineWorldPosInGrid, Vector3.up);
    }

    private GameObject SpawnMachine(Vector3 position, Vector3 direction)
    {
        Vector3Int cellPos = m_WorldGridGeometry.WorldToCell(position);
        IItemHandler itemHandler = m_ItemHandlerPrefab.GetComponent<IItemHandler>();
        if(itemHandler == null)
            return null;
        TerrainType terrainType = MapGenerator.instance.GetTileType(cellPos);
        if(!itemHandler.CanBePlacedOn(terrainType))
            return null;

        GameObject spawnedItemHandlerGameObject = Instantiate(m_ItemHandlerPrefab, position, Quaternion.identity, m_MapParentObject.transform);

        IItemHandler spawnedItemHandler = spawnedItemHandlerGameObject.GetComponent<IItemHandler>();
        Assert.IsNotNull(spawnedItemHandler);

        if(!GridManager.instance.SetItemHandlerAt(new Vector2Int(cellPos.x, cellPos.y), spawnedItemHandler, true))
        {
            Destroy(spawnedItemHandlerGameObject);
            return null;
        }

        return spawnedItemHandlerGameObject;
    }

    private void PlaceOrDeleteMachine()
    {
        if(IsDeleting())
            DeleteMachineOnCursor();
        else
            PlaceSimpleMachine();
    }

    private void DeleteMachineOnCursor()
    {
        Vector3 mouseWorldPos = m_Camera.ScreenToWorldPoint(m_InputAction.Player.PointerPosition.ReadValue<Vector2>());
        Vector3Int cellPos = m_WorldGridGeometry.WorldToCell(mouseWorldPos);

        GridManager.instance.SetItemHandlerAt(new Vector2Int(cellPos.x, cellPos.y), null, true); // do not delete spawners
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
        bool isAssigned = GridManager.instance.SetItemHandlerAt(new Vector2Int(GridPos.x, GridPos.y), newBeltObject.GetComponent<IItemHandler>(), true);
        if(!isAssigned)
            Destroy(newBeltObject);
    }

    ////////////////////////
    // Managing states
    ////////////////////////

    public void StartPlaceBelts()
    {
        InputStateMachine.instance.SetState(new BuildBeltState());
        OnBeltModeChange.Invoke();
    }

    public void EndPlaceBelts()
    {
        InputStateMachine.instance.SetState(new FreeViewState());
        OnBeltModeChange.Invoke();
    }

    public void StartPlaceMachine(GameObject iMachineToPlace)
    {
        m_ItemHandlerPrefab = iMachineToPlace;
        InputStateMachine.instance.SetState(new BuildMachineState());
        OnBeltModeChange.Invoke();
    }

    public void EndPlaceMachine()
    {
        InputStateMachine.instance.SetState(new FreeViewState());
        OnBeltModeChange.Invoke();
    }

    public void SwitchDeleteMode()
    {
        InputStateMachine.instance.SetState(IsDeleting() ? new FreeViewState() : new DeleteState());
        OnBeltModeChange.Invoke();
    }

    public bool IsPlacingBelts()
    {
        return InputStateMachine.instance.GetState() is BuildBeltState;
    }

    public bool IsPlacingMachine(GameObject iMachine)
    {
        return InputStateMachine.instance.GetState() is BuildMachineState && m_ItemHandlerPrefab == iMachine;
    }

    public bool IsDeleting()
    {
        return InputStateMachine.instance.GetState() is DeleteState;
    }

    ////////////////////////
    // Stats
    ////////////////////////

    public int GetMaxNbShop()
    {
        return m_MaxShop;
    }

    public int GetMachinePrice()
    {
        return (int)(m_MachineBaseprice * Mathf.Pow(m_MachinePriceMultiplier, Machine.GetCount()));
    }

    public int GetSpawnerPrice()
    {
        if(m_MachineBaseprice == 1)
            return Spawner.GetCount();

        return (int)((Mathf.Pow(m_SpawnerBasePrice, Spawner.GetCount() + 1) - 1) / (m_SpawnerBasePrice - 1)) - 1;
    }

    public float GetSpawnRate()
    {
        return StatManager.instance.GetStatValue(StatManager.ExtractSpeedIndex);
    }

    public float GetBeltSpeed()
    {
        return StatManager.instance.GetStatValue(StatManager.BeltSpeedIndex);
    }

    public float GetCraftingSpeedMultiplier()
    {
        return StatManager.instance.GetStatValue(StatManager.CraftSpeedIndex);
    }
}
