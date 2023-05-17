using System.Collections.Generic;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Scripts.Save_System
{
    // need to save items --> pool
    // need to save machines --> idk
    // need to save player stats --> idk
    
    [Serializable]
    public class SaveData
    {
        //TODO: link array/list of belts in stead of find objects
        private List<BeltSaveData> _beltDatas = new();
        private List<MachineSaveData> _machineDatas = new();
        private List<SplitterSaveData> _splitterDatas = new();
        private PlayerSaveData _playerSaveData;

        public SaveData()
        {
            foreach (Belt belt in Object.FindObjectsOfType(typeof(Belt))
                         .Where(b => !b.GetType().IsSubclassOf(typeof(Belt))))
            {
                var transform = belt.transform;
                _beltDatas.Add(new BeltSaveData(transform.position, transform.rotation.eulerAngles, belt.BeltItem));
            }

            foreach (Machine machines in Object.FindObjectsOfType(typeof(Machine)))
            {
                var transform = machines.transform;
                _machineDatas.Add(new MachineSaveData(machines.GetCraftedItem().Name, transform.position,
                    transform.rotation.eulerAngles, machines.Stock, machines.BeltItem));
            }
            
            foreach (Splitter splitter in Object.FindObjectsOfType(typeof(Splitter)))
            {
                var transform = splitter.transform;
                _splitterDatas.Add(new SplitterSaveData(transform.position, transform.rotation.eulerAngles, splitter.BeltItem));
            }
        }

        public List<BeltSaveData> BeltDatas => _beltDatas;
        public List<MachineSaveData> MachineDatas => _machineDatas;
        public List<SplitterSaveData> SplitterDatas => _splitterDatas;
    }

    #region DataStruct

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

        public Vector3 GetPos => new Vector3(_x, _y, _z);

        public string GetName => _name;
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

        public Vector3 GetPos => new(_x, _y, _z);

        public Vector3 GetRot => new(_rotX, _rotY, _rotZ);

        public ItemSaveData? GetItem => _itemSaveData;
    }

    [Serializable]
    public struct SplitterSaveData
    {
        private float _x;
        private float _y;
        private float _z;
        private float _rotX;
        private float _rotY;
        private float _rotZ;

        [Serialize] private ItemSaveData? _itemSaveData;

        public SplitterSaveData(Vector3 prmPos, Vector3 prmRot, Item prmItem)
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


        public Vector3 GetPos => new(_x, _y, _z);

        public Vector3 GetRot => new(_rotX, _rotY, _rotZ);
        
        public ItemSaveData? GetItem => _itemSaveData;
    }

    [Serializable]
    public struct MachineSaveData
    {
        private float _x;
        private float _y;
        private float _z;
        private float _rotX;
        private float _rotY;
        private float _rotZ;

        private string _itemToCraftName;

        private List<String> _itemNames;
        private List<int> _itemQuantity;

        [Serialize] private ItemSaveData? _itemSaveData;

        public MachineSaveData(string prmItemToCraftName, Vector3 prmPos, Vector3 prmRot,
            Dictionary<ItemData, int> prmStock, Item prmItem)
        {
            _itemToCraftName = prmItemToCraftName;
            _x = prmPos.x;
            _y = prmPos.y;
            _z = prmPos.z;
            _rotX = prmRot.x;
            _rotY = prmRot.y;
            _rotZ = prmRot.z;

            _itemNames = prmStock.Keys.Select((itemData) => itemData.Name).ToList();
            _itemQuantity = prmStock.Values.ToList();
            
            if (prmItem != null)
            {
                _itemSaveData = new ItemSaveData(prmItem.GetItemData().Name, prmItem.gameObject.transform.position);
            }
            else
            {
                _itemSaveData = null;
            }
        }

        public string ItemToCraftName => _itemToCraftName;

        public Vector3 GetPos => new(_x, _y, _z);
        public Vector3 GetRot => new(_rotX, _rotY, _rotZ);

        public IEnumerable<string> ItemNames => _itemNames;

        public IEnumerable<int> ItemQuantity => _itemQuantity;
        
        public ItemSaveData? GetItem => _itemSaveData;
    }

    [Serializable]
    public struct PlayerSaveData
    {
    }

    #endregion
}