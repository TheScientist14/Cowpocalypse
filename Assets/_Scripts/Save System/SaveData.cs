using System.Collections.Generic;
using _Scripts.Pooling_System;
using System;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Scripts.Save_System
{
    // need to save items --> pool
    // need to save machines --> idk
    // need to save player stats --> idk


    [System.Serializable]
    public class SaveData
    {
        private List<BeltSaveData> _beltDatas = new ();
        private List<MachineSaveData> _machineDatas = new();
        private PlayerSaveData _playerSaveData;

        public SaveData()
        {
            foreach (Belt belt in Object.FindObjectsOfType(typeof(Belt)))
            {
                var transform = belt.gameObject.transform;
                _beltDatas.Add(new BeltSaveData(transform.position, transform.rotation.eulerAngles, belt.BeltItem));
            }
            Debug.Log(PoolManager.instance.GetExistingItems.Count);
            Debug.Log(_beltDatas[0]);
        }

        public List<BeltSaveData> BeltDatas => _beltDatas;
    }

    [Serializable]
    public struct ItemSaveData
    {
        private string _name;
        private float _x;
        private float _y;
        private float _z;

        public ItemSaveData(string prmName, Vector3 prmPos)
        {
            _name = prmName;
            _x = prmPos.x;
            _y = prmPos.y;
            _z = prmPos.z;
        }

        public Vector3 GetPos()
        {
            return new Vector3(_x, _y, _z);
        }
    }
    
    [Serializable]
    public struct BeltSaveData
    {
        private float _x;
        private float _y;
        private float _z;
        private float _rotX;
        private float _rotY;
        private float _rotZ;
        [Serialize] private ItemSaveData? _itemSaveData;

        public BeltSaveData(Vector3 prmPos, Vector3 prmRot, Item prmItem)
        {
            _x = prmPos.x;
            _y = prmPos.y;
            _z = prmPos.z;
            _rotX = prmRot.x;
            _rotY = prmRot.y;
            _rotZ = prmRot.z;

            if (prmItem != null)
            {
                _itemSaveData = new ItemSaveData(prmItem.GetItemData().Name, prmItem.gameObject.transform.position);
            }
            else
            {
                _itemSaveData = null;
            }
        }

        public Vector3 GetPos()
        {
            return new Vector3(_x, _y, _z);
        }
        public Vector3 GetRot()
        {
            return new Vector3(_rotX, _rotY, _rotZ);
        }

        public ItemSaveData GetItem()
        {
            System.Diagnostics.Debug.Assert(_itemSaveData != null);
            return (ItemSaveData)_itemSaveData;
        }
    }
    
    [Serializable]
    internal struct MachineSaveData
    {
        private string _name;
        private float _x;
        private float _y;
        private float _z;
    }

    [Serializable]
    internal struct PlayerSaveData
    {
        
    }
}