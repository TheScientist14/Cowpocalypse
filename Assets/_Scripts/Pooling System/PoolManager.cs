using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.Pooling_System
{
    public class PoolManager : Singleton<PoolManager>
    {
        [Header("Parameters")]
        
        [Tooltip("Start number of pooled items")]
        [SerializeField] private int _startNumberOfPooledObjects = 10;
        [Tooltip("Limit of pooled items left in the pool before adding new ones (value between 0 and 1)")]
        [SerializeField] private float minPoolSize = 0.2f;
        [Tooltip("The multiplier for adding pooled items (value between 0 and 1)")]
        [SerializeField] private float percentageToAdd= 0.5f;
        
        [HorizontalLine(color: EColor.Green)]

        [Header("Info")]
        
        [Tooltip("The total number of pooled items (in use and idle)")]
        [ReadOnly] [SerializeField] private int totalNumberOfPooledItems = 0;

        [ReadOnly] [SerializeField] private List<GameObject> spawners;

        [ReadOnly] [SerializeField]
        private List<Item> itemPool = new();

        [ReadOnly] [SerializeField]
        private List<Item> existingItems = new();

        
        //Add a starting amount of spawnable items to the pool
        private void Awake()
        {
            StartCoroutine(AddToPool(_startNumberOfPooledObjects));
        }

        //Checks if the number of items are sufficient in order to spawn at least one item per spawner, used for debugging purposes not releasing with this
        private void Start()
        {
            if (itemPool.Count <= spawners.Count)
            {
                StartCoroutine(AddToPool(spawners.Count));
            }
        }
        
        
        /// <summary>
        /// Spawns an item using the pooling system, DO NOT INSTANTIATE
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="prmPosition"></param>
        /// <returns></returns>
        public Item SpawnObject(ItemData itemData, Vector3 prmPosition)
        {
            itemPool[0].SetItemData(itemData);
            if (itemData.Sprite)
            {
               itemPool[0].GetComponent<SpriteRenderer>().sprite = itemData.Sprite; 
            }
            
            itemPool[0].transform.position = prmPosition;
            itemPool[0].gameObject.SetActive(true);
            existingItems.Add(itemPool[0]);
            itemPool.RemoveAt(0);
            if (itemPool.Count <= totalNumberOfPooledItems * minPoolSize)
            {
                StartCoroutine(AddToPool(Mathf.FloorToInt(totalNumberOfPooledItems * percentageToAdd)));
            }

            return existingItems[^1];
        }

        /// <summary>
        /// Despawns an object using the pool system, DO NOT USE DESTROY
        /// </summary>
        /// <param name="prmItem"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Adds items to the pool, ie. increasing the amount of spawnable items
        /// </summary>
        /// <param name="numberOfItemsToAdd"></param>
        /// <returns></returns>
        
        private IEnumerator AddToPool(int numberOfItemsToAdd)
        {
            for (int i = 0; i < numberOfItemsToAdd; i++)
            {
                Item newItem = new GameObject().AddComponent<Item>();
                newItem.AddComponent<SpriteRenderer>().sortingOrder = 1;
                newItem.transform.parent = transform;
                newItem.gameObject.SetActive(false);
                itemPool.Add(newItem);
                totalNumberOfPooledItems++;
                yield return new WaitForEndOfFrame();
            }
        }

        #region Accessors
        
        /// <summary>
        ///  Returns the total number of pooled items, counts the items being used/displayed and those that aren't
        /// </summary>
        public int GetTotalNumberOfPooledItems => totalNumberOfPooledItems;

        /// <summary>
        /// Returns the List of all the spawners
        /// </summary>
        public List<GameObject> GetSpawners => spawners;

        /// <summary>
        /// returns the List of items not being used/displayed
        /// </summary>
        public List<Item> GetItemPool => itemPool;

        /// <summary>
        /// Returns the List of items being used/displayed
        /// </summary>
        public List<Item> GetExistingItems => existingItems;

        /// <summary>
        /// Adds spawners to the list of GameObjects that can spawn Items
        /// </summary>
        /// <param name="prmSpawner"></param>
        public void AddSpawnerToList(GameObject prmSpawner)
        {
            spawners.Add(prmSpawner);
        }

        #endregion
        
    }
}