using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Merger : Belt
{
    public List<Belt> InputBelts = new List<Belt>();
    [SerializeField] private int CurrentInput = 0;
    // Start is called before the first frame update
    void Start()
    {
        BeltInSequence = GetNextBelt();
        InputBelts.Add(GetLeftBelt());
        InputBelts.Add(GetDownBelt());
        InputBelts.Add(GetRightBelt());
        gameObject.name = $"Merger: {BeltID++}";
        //OnItemMoved.AddListener(ChooseInput);
    }

    // Update is called once per frame
    new void Update()
    {
        if(InputBelts[0] == null)
            InputBelts[0] = (GetLeftBelt());
        if(InputBelts[1] == null)
            InputBelts[1] = GetDownBelt();
        if(InputBelts[2] == null)
            InputBelts[2] = GetRightBelt();

        if (InputBelts[CurrentInput] == null || InputBelts[CurrentInput].BeltItem == null)
        {
            ChooseInput();
        }   

        base.Update();
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

    private Belt GetDownBelt()
    {
        Transform currentBeltTransform = transform;
        RaycastHit2D hit = Physics2D.Raycast(transform.position - transform.up, currentBeltTransform.up, 0.1f);

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

    public void SwitchInput()
    {
        CloseInput();
        if(InputBelts[CurrentInput] != null)
            InputBelts[CurrentInput].isMachineBlocking = false;
    }

    public void CloseInput()
    {
        foreach(Belt belt in InputBelts)
        {
            if(belt == null)
                continue;
            if(belt.BeltInSequence.GetComponent<Merger>() != null)
            {
                if (belt.BeltInSequence.GetComponent<Merger>() == this)
                    belt.isMachineBlocking = true;
            }
        }
    }

    public void ChooseInput()
    {
        int oldInput = CurrentInput;
        
        do
        {
            CurrentInput++;
            if (CurrentInput > 2)
                CurrentInput = 0;
        } while (oldInput != CurrentInput && InputBelts[CurrentInput] == null);

        if (InputBelts[CurrentInput] != null)
        {
            if (InputBelts[CurrentInput].isSpaceTaken)
                SwitchInput();
        }
    }
}
