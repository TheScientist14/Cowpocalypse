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
            foreach(Belt belt in GameObject.FindObjectsOfType<Belt>())
                _beltDatas.Add(new BeltSaveData(belt));

            foreach(Machine machine in GameObject.FindObjectsOfType<Machine>())
                _machineDatas.Add(new MachineSaveData(machine));

            foreach(Splitter splitter in GameObject.FindObjectsOfType<Splitter>())
                _splitterDatas.Add(new SplitterSaveData(splitter));

            foreach(Merger merger in GameObject.FindObjectsOfType<Merger>())
                _mergerDatas.Add(new MergerSaveData(merger));

            foreach(Seller seller in GameObject.FindObjectsOfType<Seller>())
                _sellerDatas.Add(new SellerSaveData(seller));

            foreach(Spawner spawner in GameObject.FindObjectsOfType<Spawner>())
                _spawnerDatas.Add(new SpawnerSaveData(spawner));

            _playerSaveData = new PlayerSaveData(true);
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

        public BeltSaveData(Belt iBelt)
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

        public SplitterSaveData(Splitter iSplitter)
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

        public MergerSaveData(Merger iMerger)
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
        private List<ItemSaveData> _itemsInTransfer;

        public MachineSaveData(Machine iMachine)
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

            _itemsInTransfer = new List<ItemSaveData>();
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
        private List<ItemSaveData> _itemsInTransfer;

        public SellerSaveData(Seller iSeller)
        {
            _transform = new Transform2DSaveData(iSeller.transform);

            _itemsInTransfer = new List<ItemSaveData>();
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
        private float _leftSpawnTime;

        public SpawnerSaveData(Spawner iSpawner)
        {
            _transform = new Transform2DSaveData(iSpawner.transform);
            _leftSpawnTime = iSpawner.GetTimeForNextSpawn();
        }

        public Transform2DSaveData Transform => _transform;
        public float TimeToNextSpawn => _leftSpawnTime;
    }

    [Serializable]
    public struct PlayerSaveData
    {
        private int _extractLevel;
        private int _beltLevel;
        private int _craftLevel;
        private int _money;
        private int _seed;

        public PlayerSaveData(bool _)
        {
            _extractLevel = StatManager.instance.GetStatLevel(StatManager.ExtractSpeedIndex);
            _beltLevel = StatManager.instance.GetStatLevel(StatManager.BeltSpeedIndex);
            _craftLevel = StatManager.instance.GetStatLevel(StatManager.CraftSpeedIndex);
            _money = Wallet.instance.Money;
            _seed = MapGenerator.instance.GetSeed();
        }

        public int ExtractLevel => _extractLevel;
        public int BeltLevel => _beltLevel;
        public int CraftLevel => _craftLevel;
        public int Money => _money;
        public int Seed => _seed;
    }

    #endregion
}