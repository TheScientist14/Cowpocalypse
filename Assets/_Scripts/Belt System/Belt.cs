using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Belt : MonoBehaviour
{
    public static int BeltID = 0;

    public Belt BeltInSequence;
    public Item BeltItem;
    public bool isSpaceTaken;
    public bool isMachineBlocking;

    public Machine MachineInSequence;

    private void Start()
    {
        isMachineBlocking = false;
        BeltInSequence = null;
        BeltInSequence = GetNextBelt();
        gameObject.name = $"Belt: {BeltID++}";
    }

    private void Update()
    {
        if (BeltInSequence == null)
            BeltInSequence = GetNextBelt();

        if (BeltItem != null && BeltItem.GetItem() != null)
            StartCoroutine(StartBeltMove());
    }

    public virtual IEnumerator StartBeltMove()
    {
        isSpaceTaken = true;

        if (BeltItem.GetItem() != null && BeltInSequence != null && BeltInSequence.isSpaceTaken == false)
        {
            if (BeltInSequence.GetComponent<Machine>())
            {
                MachineInSequence = BeltInSequence.GetComponent<Machine>();
                if(MachineInSequence.GetCraftedItem() != null)
                    isMachineBlocking = !MachineInSequence.GetCraftedItem().Recipes.ContainsKey(BeltItem.GetItemData());
                else
                    isMachineBlocking = true;
            }
            if (!isMachineBlocking)
            {
                if(MachineInSequence != null)
                {
                    MachineInSequence.AddToStock(BeltItem);
                    isSpaceTaken = false;
                    BeltItem = null;
                }
                else
                {
                    Vector3 toPosition = BeltInSequence.transform.position;
                    BeltInSequence.isSpaceTaken = true;
                    float step = BeltManager.instance.speed * Time.fixedDeltaTime;

                    while (BeltItem.GetItem().transform.position != toPosition)
                    {
                        BeltItem.GetItem().transform.position = Vector3.MoveTowards(BeltItem.transform.position, toPosition, step);
                        yield return null;
                    }
                    isSpaceTaken = false;
                    BeltInSequence.BeltItem = BeltItem;
                    BeltItem = null;
                }
            }
        }
    }
    public Belt GetNextBelt()
    {
        Transform currentBeltTransform = transform;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.up, currentBeltTransform.up, 0.1f);

        if (hit.collider != null)
        {
            Belt belt = hit.collider.GetComponent<Belt>();
            if (belt != null)
            {
                if (belt.GetComponent<Machine>())
                    MachineInSequence = belt.GetComponent<Machine>();
                return belt;
            }
        }

        return null;
    }
}
