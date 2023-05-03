using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Pooling_System
{
    public class PoolManager : MonoBehaviour
    {
        [Header("Start number of pooled items")]
        [SerializeField] private int numberOfPooledObjects;
        [Header("Limit of pooled items left in the pool before adding new ones")]
        [SerializeField] private int minPoolSize;
        [Header("Number of pooled items to add once limit is reached")]
        [SerializeField] private int numberToAdd;
        [Header("The multiplier for adding pooled items (the more you produce the more you should add) the added pooled items shouldn't be a constant")]
        [SerializeField] private int numberToAddMultiplier;

        public bool NotEnabled => false;

        [EnableIf("NotEnabled")] [SerializeField]
        private List<Item> _itemPool = new();

        [EnableIf("NotEnabled")] [SerializeField]
        private List<Item> _existingItems = new();

        private void Awake()
        {
            for (int i = 0; i < numberOfPooledObjects; i++)
            {
                Item newItem = new GameObject().AddComponent<Item>();
                newItem.transform.parent = transform;
                newItem.gameObject.SetActive(false);
                _itemPool.Add(newItem);
            }
        }

        [Button("SpawnObject")]
        public void SpawnObject()
        {
            _itemPool[0].gameObject.SetActive(true);
            _existingItems.Add(_itemPool[0]);
            _itemPool.RemoveAt(0);

            if (_itemPool.Count <= minPoolSize)
            {
                StartCoroutine(AddToPool(numberToAdd * numberToAddMultiplier));
            }
        }


        public bool DespawnObject(Item prmItem)
        {
            prmItem.gameObject.SetActive(false);
            if (_existingItems.Contains(prmItem))
            {
                _existingItems.Remove(prmItem);

                _itemPool.Add(prmItem);
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
                _itemPool.Add(newItem);
            }
            
            yield break;
        }
    }
}