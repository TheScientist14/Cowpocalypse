using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridManager : Singleton<GridManager>
{
    // contains the instances of belt
    private Dictionary<Vector2Int, IItemHandler> m_WorldGridStorage;

    [SerializeField] Grid m_GridGeometry;

    public UnityEvent OnGridChanged;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        InitGrid();
    }

    // search for existing belt in the world
    public void InitGrid()
    {
        m_WorldGridStorage = new Dictionary<Vector2Int, IItemHandler>();
        if(m_GridGeometry == null)
            return;

        IEnumerable<IItemHandler> itemHandlers = FindObjectsOfType<IItemHandler>();
        foreach(IItemHandler itemHandler in itemHandlers)
        {
            Vector3Int cellPos = m_GridGeometry.WorldToCell(itemHandler.gameObject.transform.position);
            SetItemHandlerAt(new Vector2Int(cellPos.x, cellPos.y), itemHandler);
        }
        OnGridChanged.Invoke();
    }

    public IItemHandler GetItemHandlerAt(Vector2Int iCellPos)
    {
        return m_WorldGridStorage.GetValueOrDefault(iCellPos, null);
    }

    public IItemHandler GetItemHandlerAt(Vector3 iWorldPos)
    {
        Vector3Int cellPos = m_GridGeometry.WorldToCell(iWorldPos);
        return GetItemHandlerAt(new Vector2Int(cellPos.x, cellPos.y));
    }

    public bool IsCellFree(Vector2Int iCellPos)
    {
        return GetItemHandlerAt(iCellPos) != null;
    }

    public bool SetItemHandlerAt(Vector2Int iCellPos, IItemHandler iItemHandler, bool iDoOverride = false)
    {
        IItemHandler prevItemHandler = GetItemHandlerAt(iCellPos);
        if(prevItemHandler != null)
        {
            if(iDoOverride && prevItemHandler.CanBeOverriden())
                Destroy(prevItemHandler.gameObject);
            else
                return false;
        }

        m_WorldGridStorage[iCellPos] = iItemHandler;
        OnGridChanged.Invoke();
        return true;
    }
}
