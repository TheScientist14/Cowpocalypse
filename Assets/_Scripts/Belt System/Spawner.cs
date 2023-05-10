using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Pooling_System;
using UnityEngine;
using System;

public class Spawner : Belt
{
    public GameObject SpawnedItem;
    [Expandable]
    public ItemData SpawnedItemData;
    public float SpawnRate;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = $"Spawner: {BeltID++}";
        PoolManager.instance.AddSpawnerToList(gameObject);
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
