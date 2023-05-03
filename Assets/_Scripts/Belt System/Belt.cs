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

    private void Start()
    {
        isMachineBlocking = false;
        BeltInSequence = null;
        BeltInSequence = GetNextBelt();
        gameObject.name = $"Belt: {BeltID++}";
    }

    private void Update()
    {
        if(BeltInSequence == null)
            BeltInSequence = GetNextBelt();

        if (BeltItem != null && BeltItem.GetItem() != null)
            StartCoroutine(StartBeltMove());
    }

    public Vector3 GetItemPosition()
    {
        float padding = 0f;
        Vector3 position = transform.position;
        return new Vector3(position.x, position.y + padding, position.z);
    }

    public virtual IEnumerator StartBeltMove()
    {
        isSpaceTaken = true;

        if (BeltItem.GetItem() != null && BeltInSequence != null && BeltInSequence.isSpaceTaken == false)
        {
            if (BeltInSequence.GetComponent<Machine>())
            {
                Machine machine = BeltInSequence.GetComponent<Machine>();
                isMachineBlocking = !machine.GetCraftedItem().Recipes.ContainsKey(BeltItem.GetItemData());
            }
            if(!isMachineBlocking)
            {
                Vector3 toPosition = BeltInSequence.GetItemPosition();
                BeltInSequence.isSpaceTaken = true;
                float step = BeltManager.Instance.speed * Time.fixedDeltaTime;

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

    private Belt GetNextBelt()
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
}
