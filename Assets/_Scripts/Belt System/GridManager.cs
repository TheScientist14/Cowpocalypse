using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // contains the instances of belt to 
    private Dictionary<Vector2Int, Belt> m_WorldGrid;

    // Start is called before the first frame update
    void Start()
    {
        m_WorldGrid = new Dictionary<Vector2Int, Belt>();
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
        if(!IsCellFree(iCellPos))
        {
            if(iDoOverride)
                Destroy(GetBeltAt(iCellPos));
            else
                return false;
        }

        m_WorldGrid[iCellPos] = iBelt;
        return true;
    }
}
