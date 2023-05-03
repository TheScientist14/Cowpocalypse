using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.Pooling_System
{
    public class PoolManager : Singleton<PoolManager>
    {
        [Header("Start number of pooled items")]
        [ShowNonSerializedField] private int _numberOfPooledObjects = 10;
        [Header("Limit of pooled items left in the pool before adding new ones (value between 0 and 1)")]
        [SerializeField] private float minPoolSize;
        [Header("The multiplier for adding pooled items (value between 0 and 1")]
        [SerializeField] private float percentageToAdd;

        //TODO : uncoment sprite line (line 43)
        
        public bool NotEnabled => false;

        [EnableIf("NotEnabled")] [SerializeField]
        private List<Item> itemPool = new();

        [EnableIf("NotEnabled")] [SerializeField]
        private List<Item> existingItems = new();

        private void Awake()
        {
            for (int i = 0; i < _numberOfPooledObjects; i++)
            {
                Item newItem = new GameObject().AddComponent<Item>();
                newItem.AddComponent<SpriteRenderer>();
                newItem.transform.parent = transform;
                newItem.gameObject.SetActive(false);
                itemPool.Add(newItem);
            }
        }
        
        public Item SpawnObject(ItemData itemData, Vector3 prmPosition)
        {
            itemPool[0].SetItemData(itemData);
            //itemPool[0].GetComponent<SpriteRenderer>().sprite = itemData.sprite;
            itemPool[0].transform.position = prmPosition;
            itemPool[0].gameObject.SetActive(true);
            existingItems.Add(itemPool[0]);
            itemPool.RemoveAt(0);
            if (itemPool.Count <= _numberOfPooledObjects * minPoolSize)
            {
                StartCoroutine(AddToPool(Mathf.FloorToInt(_numberOfPooledObjects * percentageToAdd)));
            }

            return existingItems[^1];
        }


        public bool DespawnObject(Item prmItem)
        {
            prmItem.gameObject.SetActive(false);
            if (existingItems.Contains(prmItem))
            {
                existingItems.Remove(prmItem);

                itemPool.Add(prmItem);
                return true;
            }

            return false;
        }

        private IEnumerator AddToPool(int numberOfItemsToAdd)
        {
            for (int i = 0; i < numberOfItemsToAdd; i++)
            {
                Item newItem = new GameObject().AddComponent<Item>();
                newItem.transform.parent = transform;
                newItem.gameObject.SetActive(false);
                itemPool.Add(newItem);
                _numberOfPooledObjects++;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}