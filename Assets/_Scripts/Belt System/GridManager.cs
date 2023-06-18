using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GridManager : Singleton<GridManager>
{
    // contains the instances of belt
    private Dictionary<Vector2Int, Belt> m_WorldGrid;

    [SerializeField] Grid gridGeometry;

    public UnityEvent onGridChanged;

    // Start is called before the first frame update
    void Awake()
    {
       InitGrid();
    }

    // search for existing belt in the world
    public void InitGrid()
    {
        m_WorldGrid = new Dictionary<Vector2Int, Belt>();
        if(gridGeometry == null)
            return;

        Belt[] belts = FindObjectsOfType<Belt>();
        foreach(Belt belt in belts)
        {
            Vector3Int cellPos = gridGeometry.WorldToCell(belt.gameObject.transform.position);
            SetBeltAt(new Vector2Int(cellPos.x, cellPos.y), belt);
        }
        onGridChanged.Invoke();
    }

    public Belt GetBeltAt(Vector2Int iCellPos)
    {
        return m_WorldGrid.GetValueOrDefault(iCellPos, null);
    }

    public bool IsCellFree(Vector2Int iCellPos)
    {
        return GetBeltAt(iCellPos) != null;
    }

    public bool SetBeltAt(Vector2Int iCellPos, Belt iBelt, bool iDoOverride = false)
    {
        Belt prevBelt = GetBeltAt(iCellPos);
        if(prevBelt != null)
        {
            if(iDoOverride && prevBelt.GetComponent<Spawner>() == null) // cannot override spawners
                Destroy(prevBelt.gameObject);
            else
                return false;
        }

        m_WorldGrid[iCellPos] = iBelt;
        onGridChanged.Invoke();
        return true;
    }
}
