using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Path;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BeltManager : Singleton<BeltManager>
{
    public GameObject TestPrefab;

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
        GameGrid = GetComponent<Grid>();
        m_InputAction = InputMaster.instance.InputAction;
        m_InputAction.Player.Drag.started += context => InitDrag();
        m_InputAction.Player.Drag.canceled += ctx => EndDrag(ctx.ReadValue<Vector2>());
        m_InputAction.Player.Drag.performed += context => DuringDrag();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void InitDrag()
    {
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
        if (DraggingPhase >= 2)
            DraggingPhase = 0;
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
}
