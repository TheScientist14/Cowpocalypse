using System.Collections.Generic;
using System;
using UnityEngine;

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
        private List<SpawnerSaveData> _spawnerDatas = new();
        private PlayerSaveData _playerSaveData;

        public SaveData()
        {
            foreach(NewBelt belt in GameObject.FindObjectsOfType<NewBelt>())
                _beltDatas.Add(new BeltSaveData(belt));

            foreach(NewMachine machine in GameObject.FindObjectsOfType<NewMachine>())
                _machineDatas.Add(new MachineSaveData(machine));

            foreach(NewSplitter splitter in GameObject.FindObjectsOfType<NewSplitter>())
                _splitterDatas.Add(new SplitterSaveData(splitter));

            foreach(NewMerger merger in GameObject.FindObjectsOfType<NewMerger>())
                _mergerDatas.Add(new MergerSaveData(merger));

            foreach(NewSeller seller in GameObject.FindObjectsOfType<NewSeller>())
                _sellerDatas.Add(new SellerSaveData(seller));

            foreach(NewSpawner spawner in GameObject.FindObjectsOfType<NewSpawner>())
                _spawnerDatas.Add(new SpawnerSaveData(spawner));

            List<StatSaveData> stats = new List<StatSaveData>();

            foreach(var stat in StatManager.instance.Stats)
                stats.Add(new StatSaveData(stat.CurrentLevel));

            _playerSaveData = new PlayerSaveData(stats, Wallet.instance.Money);
        }

        public List<BeltSaveData> BeltDatas => _beltDatas;
        public List<MachineSaveData> MachineDatas => _machineDatas;
        public List<SplitterSaveData> SplitterDatas => _splitterDatas;
        public List<MergerSaveData> MergerDatas => _mergerDatas;
        public List<SellerSaveData> SellerDatas => _sellerDatas;
        public List<SpawnerSaveData> SpawnerDatas => _spawnerDatas;
        public PlayerSaveData PlayerSaveData => _playerSaveData;
    }

    #region DataStruct

    [Serializable]
    public struct ItemSaveData
    {
        private string _name;
        private float _x;
        private float _y;

        public ItemSaveData(Item iItem)
        {
            _name = iItem.GetItemData().Name;
            _x = iItem.transform.position.x;
            _y = iItem.transform.position.y;
        }

        public Vector3 Pos => new Vector3(_x, _y, 0);
        public string Name => _name;
    }

    [Serializable]
    public struct Transform2DSaveData
    {
        private float _x;
        private float _y;
        private float _rotZ;

        public Transform2DSaveData(Transform iTransform)
        {
            _x = iTransform.position.x;
            _y = iTransform.position.y;
            _rotZ = iTransform.rotation.eulerAngles.z;
        }

        public Vector3 Pos => new Vector3(_x, _y, 0);
        public Quaternion Rot => Quaternion.Euler(0, 0, _rotZ);
    }

    [Serializable]
    public struct BeltSaveData
    {
        private Transform2DSaveData _transform;
        private ItemSaveData? _itemSaveData;

        public BeltSaveData(NewBelt iBelt)
        {
            _transform = new Transform2DSaveData(iBelt.transform);

            Item item = iBelt.GetCurrentItem();
            if(item != null)
                _itemSaveData = new ItemSaveData(item);
            else
                _itemSaveData = null;
        }

        public Transform2DSaveData Transform => _transform;
        public ItemSaveData? Item => _itemSaveData;
    }

    [Serializable]
    public struct SplitterSaveData
    {
        private Transform2DSaveData _transform;
        private ItemSaveData? _itemSaveData;
        private int _outputIdx;

        public SplitterSaveData(NewSplitter iSplitter)
        {
            _transform = new Transform2DSaveData(iSplitter.transform);
            Item item = iSplitter.GetCurrentItem();
            if(item != null)
                _itemSaveData = new ItemSaveData(item);
            else
                _itemSaveData = null;
            _outputIdx = iSplitter.GetCurrentOutputIndex();
        }

        public Transform2DSaveData Transform => _transform;
        public ItemSaveData? Item => _itemSaveData;
        public int OutputIndex => _outputIdx;
    }

    [Serializable]
    public struct MergerSaveData
    {
        private Transform2DSaveData _transform;
        private ItemSaveData? _itemSaveData;
        private int _inputIdx;

        public MergerSaveData(NewMerger iMerger)
        {
            _transform = new Transform2DSaveData(iMerger.transform);
            Item item = iMerger.GetCurrentItem();
            if(item != null)
                _itemSaveData = new ItemSaveData(item);
            else
                _itemSaveData = null;
            _inputIdx = iMerger.GetCurrentInputIndex();
        }

        public Transform2DSaveData Transform => _transform;
        public ItemSaveData? Item => _itemSaveData;
        public int InputIndex => _inputIdx;
    }

    [Serializable]
    public struct MachineSaveData
    {
        private Transform2DSaveData _transform;
        private ItemSaveData? _craftedItem;
        private string _itemToCraftName;
        private float _leftCraftTime;
        private List<string> _itemNames;
        private List<int> _itemQuantity;
        private HashSet<ItemSaveData> _itemsInTransfer;

        public MachineSaveData(NewMachine iMachine)
        {
            _transform = new Transform2DSaveData(iMachine.transform);
            Item item = iMachine.GetCraftedItem();
            if(item != null)
                _craftedItem = new ItemSaveData(item);
            else
                _craftedItem = null;
            ItemData itemData = iMachine.GetCraftedItemData();
            if(itemData != null)
                _itemToCraftName = itemData.Name;
            else
                _itemToCraftName = ""; // assuming there is no item with empty name
            _leftCraftTime = iMachine.GetTimeLeftForCurrentCraft();

            _itemNames = new List<string>();
            _itemQuantity = new List<int>();
            foreach(KeyValuePair<ItemData, int> itemCount in iMachine.GetCurrentStock())
            {
                _itemNames.Add(itemCount.Key.Name);
                _itemQuantity.Add(itemCount.Value);
            }

            _itemsInTransfer = new HashSet<ItemSaveData>();
            foreach(Item itemInTransfer in iMachine.GetItemsInTransfer())
                _itemsInTransfer.Add(new ItemSaveData(itemInTransfer));
        }

        public Transform2DSaveData Transform => _transform;
        public ItemSaveData? CraftedItem => _craftedItem;
        public string ItemToCraftName => _itemToCraftName;
        public float TimeLeftToCraft => _leftCraftTime;
        public IEnumerable<string> ItemNames => _itemNames;
        public IEnumerable<int> ItemQuantity => _itemQuantity;
        public IEnumerable<ItemSaveData> ItemsInTransfer => _itemsInTransfer;
    }

    [Serializable]
    public struct SellerSaveData
    {
        private Transform2DSaveData _transform;
        private HashSet<ItemSaveData> _itemsInTransfer;

        public SellerSaveData(NewSeller iSeller)
        {
            _transform = new Transform2DSaveData(iSeller.transform);

            _itemsInTransfer = new HashSet<ItemSaveData>();
            foreach(Item itemInTransfer in iSeller.GetItemsInTransfer())
                _itemsInTransfer.Add(new ItemSaveData(itemInTransfer));
        }

        public Transform2DSaveData Transform => _transform;
        public IEnumerable<ItemSaveData> ItemsInTransfer => _itemsInTransfer;
    }

    [Serializable]
    public struct SpawnerSaveData
    {
        private Transform2DSaveData _transform;
        private string _itemNameToSpawn;
        private ItemSaveData? _spawnedItem;
        private float _leftSpawnTime;

        public SpawnerSaveData(NewSpawner iSpawner)
        {
            _transform = new Transform2DSaveData(iSpawner.transform);
            _itemNameToSpawn = iSpawner.GetItemDataToSpawn().Name;
            Item item = iSpawner.GetCurrentItem();
            if(item != null)
                _spawnedItem = new ItemSaveData(item);
            else
                _spawnedItem = null;
            _leftSpawnTime = iSpawner.GetTimeForNextSpawn();
        }

        public Transform2DSaveData Transform => _transform;
        public string ItemNameToSpawn => _itemNameToSpawn;
        public ItemSaveData? SpawnedItem => _spawnedItem;
        public float TimeToNextSpawn => _leftSpawnTime;
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

    #endregion
}