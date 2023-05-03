using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : Belt
{
    public GameObject SpawnedItem;
    public float SpawnRate;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = $"Spawner: {BeltID++}";
        StartCoroutine(Spawn());
    }


    private IEnumerator Spawn()
    {
        while(true)
        {
            yield return new WaitForSeconds(SpawnRate);
            if (isSpaceTaken == false)
            {
                GameObject Item = Instantiate(SpawnedItem, transform.position, Quaternion.Euler(Vector3.zero));
                BeltItem = Item.GetComponent<Item>();
                isSpaceTaken = true;
            }
        }
    }
}
