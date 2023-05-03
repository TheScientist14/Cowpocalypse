using System.Collections;
using System.Collections.Generic;
using _Scripts.Pooling_System;
using UnityEngine;

public class Spawner : Belt
{
    public GameObject SpawnedItem;
    public ItemData SpawnedItemData;
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
                BeltItem = PoolManager.instance.SpawnObject(SpawnedItemData, transform.position);
                isSpaceTaken = true;
            }
        }
    }
}
