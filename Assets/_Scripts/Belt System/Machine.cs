using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : Belt
{
    [SerializeField] private float CraftSpeed = 1;
    // Start is called before the first frame update

    private void Start()
    {
        gameObject.name = $"Machine: {BeltID++}";
    }

    public override IEnumerator StartBeltMove()
    {
        isSpaceTaken = true;

        yield return new WaitForSeconds(CraftSpeed);

        if (BeltItem.item != null && BeltInSequence != null && BeltInSequence.isSpaceTaken == false)
        {
            Vector3 toPosition = BeltInSequence.GetItemPosition();
            BeltInSequence.isSpaceTaken = true;
            float step = BeltManager.Instance.speed * Time.deltaTime;

            while (BeltItem.item.transform.position != toPosition)
            {
                BeltItem.item.transform.position = Vector3.MoveTowards(BeltItem.transform.position, toPosition, step);
                yield return null;
            }

            isSpaceTaken = false;
            BeltInSequence.BeltItem = BeltItem;
            BeltItem = null;
        }
    }
}
