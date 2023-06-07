using System.Collections.Generic;
using System;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Scripts.Save_System
{
    
    [Serializable]
    public class SaveData
    {
        private List<BeltSaveData> _beltDatas = new();
        private List<MachineSaveData> _machineDatas = new();
        private List<SplitterSaveData> _splitterDatas = new();
        private List<MergerSaveData> _mergerDatas = new();
        private List<SellerSaveData> _sellerDatas = new();
        private PlayerSaveData _playerSaveData;
        

        public SaveData()
        {
            foreach (var o in Object.FindObjectsOfType(typeof(Belt))
                         .Where(b => !b.GetType().IsSubclassOf(typeof(Belt))))
            {
                var belt = (Belt)o;
                var transform = belt.transform;
                _beltDatas.Add(new BeltSaveData(transform.position, transform.rotation.eulerAngles, belt.BeltItem));
            }

            foreach (var o in Object.FindObjectsOfType(typeof(Machine)))
            {
                var machines = (Machine)o;
                var transform = machines.transform;
                _machineDatas.Add(new MachineSaveData( transform.position,
                    transform.rotation.eulerAngles, machines.Stock, machines.BeltItem, machines.GetCraftedItem() ? machines.GetCraftedItem().Name : ""));
            }
            
            foreach (var o in Object.FindObjectsOfType(typeof(Splitter)))
            {
                var splitter = (Splitter)o;
                var transform = splitter.transform;
                _splitterDatas.Add(new SplitterSaveData(transform.position, transform.rotation.eulerAngles, splitter.BeltItem));
            }

            foreach (var o in Object.FindObjectsOfType(typeof(Merger)))
            {
                var merger = (Merger)o;
                var transform = merger.transform;
                _mergerDatas.Add(new MergerSaveData(transform.position, transform.rotation.eulerAngles, merger.BeltItem));
            }

            foreach (var o in Object.FindObjectsOfType(typeof(Seller)))
            {
                var seller = (Seller)o;
                var transform = seller.transform;
                _sellerDatas.Add(new SellerSaveData(transform.position, transform.rotation.eulerAngles, seller.BeltItem));
            }

            List<StatSaveData> stats = new List<StatSaveData>();
            
            foreach (var stat in StatManager.instance.Stats)
            {
                stats.Add(new StatSaveData(stat.CurrentLevel));
            }
            
            _playerSaveData = new PlayerSaveData(stats, Wallet.instance.Money);
        }

        public List<BeltSaveData> BeltDatas => _beltDatas;
        public List<MachineSaveData> MachineDatas => _machineDatas;
        public List<SplitterSaveData> SplitterDatas => _splitterDatas;
        public List<MergerSaveData> MergerDatas => _mergerDatas;
        public List<SellerSaveData> SellerDatas => _sellerDatas;

        public PlayerSaveData PlayerSaveData => _playerSaveData;
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
    public struct MergerSaveData
    {
        private float _x;
        private float _y;
        private float _z;
        private float _rotX;
        private float _rotY;
        private float _rotZ;

        [Serialize] private ItemSaveData? _itemSaveData;

        public MergerSaveData(Vector3 prmPos, Vector3 prmRot, Item prmItem)
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

        public MachineSaveData( Vector3 prmPos, Vector3 prmRot,
            Dictionary<ItemData, int> prmStock, Item prmItem, string prmItemToCraftName)
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
        private List<StatSaveData> _stats;
        private int _money;

        public PlayerSaveData(List<StatSaveData> stats, int money = 0)
        {
            _stats = stats;
            _money = money;
        }

        public List<StatSaveData> Stats => _stats;

        public int Money => _money;
    }

    [Serializable]
    public struct StatSaveData
    {
        private int _currentLevel;

        public StatSaveData(int currentLevel)
        {
            _currentLevel = currentLevel;
        }

        public int CurrentLevel => _currentLevel;
    }

    [Serializable]
    public struct SellerSaveData
    {
        private float _x;
        private float _y;
        private float _z;
        private float _rotX;
        private float _rotY;
        private float _rotZ;
        
        [Serialize] private ItemSaveData? _itemSaveData;
        
        public SellerSaveData(Vector3 prmPos, Vector3 prmRot, Item prmItem)
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

    #endregion
}