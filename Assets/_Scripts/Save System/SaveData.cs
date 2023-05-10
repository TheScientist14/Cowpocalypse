using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Pooling_System;
using UnityEditor;
using UnityEngine;

namespace _Scripts.Save_System
{
    // need to save items --> pool
    // need to save machines --> idk
    // need to save player stats --> idk


    [System.Serializable]
    public class SaveData
    {
        private Item[] _items = {};

        public SaveData()
        {
            foreach (var item in PoolManager.instance.GetExistingItems)
            {
                _items.Append(item);
            }
        }
    }
}