using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Splitter : Belt
{
    public List<Belt> OutputBelts = new List<Belt>();
    private int CurrentOutput = 0;
    bool ItemMoving = false;

    private void Start()
    {
        OutputBelts.Add(GetLeftBelt());
        OutputBelts.Add(GetUpBelt());
        OutputBelts.Add(GetRightBelt());
        gameObject.name = $"Splitter: {BeltID++}";
    }

    new void Update()
    {
        if(OutputBelts[0] == null)
            OutputBelts[0] = (GetLeftBelt());
        if(OutputBelts[1] == null)
            OutputBelts[1] = GetUpBelt();
        if(OutputBelts[2] == null)
            OutputBelts[2] = GetRightBelt();

        if(BeltItem != null && BeltItem.GetItem() != null)
            StartCoroutine(StartBeltMove());
    }

    public override IEnumerator StartBeltMove()
    {
        isSpaceTaken = true;
        if(CurrentOutput > 2)
            CurrentOutput = 0;

        if(OutputBelts[CurrentOutput] != null)
        {
            if(OutputBelts[CurrentOutput].isSpaceTaken == false)
            {
                if(OutputBelts[CurrentOutput].GetComponent<Machine>())
                {
                    MachineInSequence = OutputBelts[CurrentOutput].GetComponent<Machine>();
                    if(MachineInSequence.GetCraftedItem() != null)
                        isMachineBlocking = !MachineInSequence.GetCraftedItem().Recipes.ContainsKey(BeltItem.GetItemData());
                    else
                        isMachineBlocking = true;
                }
                if (OutputBelts[CurrentOutput].GetComponent<Spawner>())
                    isMachineBlocking = true;
                if (!isMachineBlocking)
                {
                    if(MachineInSequence != null)
                    {
                        MachineInSequence.AddToStock(BeltItem);
                        isSpaceTaken = false;
                        BeltItem = null;
                        CurrentOutput++;
                        MachineInSequence = null;
                    }
                    else
                    {
                        ItemMoving = true;
                        Vector3 toPosition = OutputBelts[CurrentOutput].transform.position;
                        OutputBelts[CurrentOutput].isSpaceTaken = true;
                        float step = BeltManager.instance.speed * Time.fixedDeltaTime;

                        /*while(BeltItem.GetItem().transform.position != toPosition)
                        {
                            BeltItem.GetItem().transform.position = Vector3.MoveTowards(BeltItem.transform.position, toPosition, step);
                            yield return new WaitForEndOfFrame();
                        }*/

                        ItemMoving = false;
                        isSpaceTaken = false;
                        OutputBelts[CurrentOutput].BeltItem = BeltItem;
                        BeltItem = null;
                        CurrentOutput++;
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
            else if(!ItemMoving)
                CurrentOutput++;
        }
        else if(!ItemMoving)
            CurrentOutput++;
    }

    private Belt GetLeftBelt()
    {
        Transform currentBeltTransform = transform;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + -transform.right, -currentBeltTransform.right, 0.1f);

        if(hit.collider != null)
        {
            Belt belt = hit.collider.GetComponent<Belt>();
            if(belt != null)
                return belt;
        }

        return null;
    }

    private Belt GetUpBelt()
    {
        Transform currentBeltTransform = transform;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.up, currentBeltTransform.up, 0.1f);

        if(hit.collider != null)
        {
            Belt belt = hit.collider.GetComponent<Belt>();
            if(belt != null)
                return belt;
        }

        return null;
    }

    private Belt GetRightBelt()
    {
        Transform currentBeltTransform = transform;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right, currentBeltTransform.right, 0.1f);

        if(hit.collider != null)
        {
            Belt belt = hit.collider.GetComponent<Belt>();
            if(belt != null)
                return belt;
        }

        return null;
    }
}
