using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using static UnityEditor.PlayerSettings;
using System.Linq;

namespace _Scripts.Save_System
{
    class UnsupportedVersion : Exception { }

    class Helper
    {
        public static void WriteList<T>(List<T> iList, BinaryWriter ioWriter, Action<BinaryWriter, T> iWriteFunc)
        {
            ioWriter.Write(iList.Count);
            foreach(T elem in iList)
                iWriteFunc(ioWriter, elem);
        }

        public static List<T> ReadList<T>(BinaryReader ioReader, Func<BinaryReader, T> iReadFunc)
        {
            List<T> list = new List<T>();
            int size = ioReader.ReadInt32();
            list.Capacity = size;

            for(int iElem = 0; iElem < size; iElem++)
                list.Add(iReadFunc(ioReader));

            return list;
        }
    }

    [Serializable]
    public class SaveData
    {
        // Versions :
        // 0 : init
        public const uint Version = 0;

        private List<BeltSaveData> _beltDatas = new();
        private List<MachineSaveData> _machineDatas = new();
        private List<SplitterSaveData> _splitterDatas = new();
        private List<MergerSaveData> _mergerDatas = new();
        private List<SellerSaveData> _sellerDatas = new();
        private List<SpawnerSaveData> _spawnerDatas = new();
        private PlayerSaveData _playerSaveData;

        public SaveData(bool iInit = false)
        {
            if(!iInit)
                return;

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

        public void WriteBin(BinaryWriter ioWriter)
        {
            ioWriter.Write(Version);
            ioWriter.Write(ItemSaveData.Version);
            ioWriter.Write(Transform2DSaveData.Version);
            ioWriter.Write(BeltSaveData.Version);
            ioWriter.Write(MachineSaveData.Version);
            ioWriter.Write(SplitterSaveData.Version);
            ioWriter.Write(MergerSaveData.Version);
            ioWriter.Write(SellerSaveData.Version);
            ioWriter.Write(SpawnerSaveData.Version);
            ioWriter.Write(PlayerSaveData.Version);

            Helper.WriteList(_beltDatas, ioWriter, (writer, b) => b.WriteBin(ioWriter));
            Helper.WriteList(_machineDatas, ioWriter, (writer, m) => m.WriteBin(ioWriter));
            Helper.WriteList(_splitterDatas, ioWriter, (writer, s) => s.WriteBin(ioWriter));
            Helper.WriteList(_mergerDatas, ioWriter, (writer, m) => m.WriteBin(ioWriter));
            Helper.WriteList(_sellerDatas, ioWriter, (writer, s) => s.WriteBin(ioWriter));
            Helper.WriteList(_spawnerDatas, ioWriter, (writer, s) => s.WriteBin(ioWriter));
            _playerSaveData.WriteBin(ioWriter);
        }

        public SaveData(BinaryReader ioReader, out string oMsg)
        {
            oMsg = "";

            uint version = ioReader.ReadUInt32();
            uint itemVersion = ioReader.ReadUInt32();
            uint transformVersion = ioReader.ReadUInt32();
            uint beltVersion = ioReader.ReadUInt32();
            uint machineVersion = ioReader.ReadUInt32();
            uint splitterVersion = ioReader.ReadUInt32();
            uint mergerVersion = ioReader.ReadUInt32();
            uint sellerVersion = ioReader.ReadUInt32();
            uint spawnerVersion = ioReader.ReadUInt32();
            uint playerVersion = ioReader.ReadUInt32();
            if(version > Version
                || itemVersion > ItemSaveData.Version
                || transformVersion > Transform2DSaveData.Version
                || beltVersion > BeltSaveData.Version
                || machineVersion > MachineSaveData.Version
                || splitterVersion > SplitterSaveData.Version
                || mergerVersion > MergerSaveData.Version
                || sellerVersion > SellerSaveData.Version
                || spawnerVersion > SpawnerSaveData.Version
                || playerVersion > PlayerSaveData.Version)
            {
                oMsg = "Game version is older than save file, can't load.";
                throw new UnsupportedVersion();
            }

            _beltDatas = Helper.ReadList(ioReader, (reader) => new BeltSaveData(reader, beltVersion, transformVersion, itemVersion));
            _machineDatas = Helper.ReadList(ioReader, (reader) => new MachineSaveData(reader, machineVersion, transformVersion, itemVersion));
            _splitterDatas = Helper.ReadList(ioReader, (reader) => new SplitterSaveData(reader, splitterVersion, transformVersion, itemVersion));
            _mergerDatas = Helper.ReadList(ioReader, (reader) => new MergerSaveData(reader, mergerVersion, transformVersion, itemVersion));
            _sellerDatas = Helper.ReadList(ioReader, (reader) => new SellerSaveData(reader, sellerVersion, transformVersion, itemVersion));
            _spawnerDatas = Helper.ReadList(ioReader, (reader) => new SpawnerSaveData(reader, spawnerVersion, transformVersion, itemVersion));
            _playerSaveData = new PlayerSaveData(ioReader, playerVersion, transformVersion);
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
        // Versions :
        // 0 : init
        public const uint Version = 0;

        private string _name;
        private float _x;
        private float _y;

        public ItemSaveData(Item iItem)
        {
            _name = iItem.GetItemData().Name;
            _x = iItem.transform.position.x;
            _y = iItem.transform.position.y;
        }

        public void WriteBin(BinaryWriter ioWriter)
        {
            ioWriter.Write(_name);
            ioWriter.Write(_x);
            ioWriter.Write(_y);
        }

        public ItemSaveData(BinaryReader ioReader, uint iVersion)
        {
            if(iVersion > Version)
                throw new UnsupportedVersion();

            _name = ioReader.ReadString();
            _x = ioReader.ReadSingle();
            _y = ioReader.ReadSingle();
        }

        public Vector3 Pos => new Vector3(_x, _y, 0);
        public string Name => _name;
    }

    [Serializable]
    public struct Transform2DSaveData
    {
        // Versions :
        // 0 : init
        public const uint Version = 0;

        private float _x;
        private float _y;
        private float _rotZ;

        public Transform2DSaveData(Transform iTransform)
        {
            _x = iTransform.position.x;
            _y = iTransform.position.y;
            _rotZ = iTransform.rotation.eulerAngles.z;
        }

        public void WriteBin(BinaryWriter ioWriter)
        {
            ioWriter.Write(_x);
            ioWriter.Write(_y);
            ioWriter.Write(_rotZ);
        }

        public Transform2DSaveData(BinaryReader ioReader, uint iVersion)
        {
            if(iVersion > Version)
                throw new UnsupportedVersion();

            _x = ioReader.ReadSingle();
            _y = ioReader.ReadSingle();
            _rotZ = ioReader.ReadSingle();
        }

        public Vector3 Pos => new Vector3(_x, _y, 0);
        public Quaternion Rot => Quaternion.Euler(0, 0, _rotZ);
    }

    [Serializable]
    public struct BeltSaveData
    {
        // Versions :
        // 0 : init
        public const uint Version = 0;

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

        public void WriteBin(BinaryWriter ioWriter)
        {
            _transform.WriteBin(ioWriter);

            ioWriter.Write(_itemSaveData.HasValue);
            if(_itemSaveData.HasValue)
                _itemSaveData.Value.WriteBin(ioWriter);
        }

        public BeltSaveData(BinaryReader ioReader, uint iBeltVersion, uint iTransformVersion, uint iItemVersion)
        {
            if(iBeltVersion > Version)
                throw new UnsupportedVersion();

            _transform = new Transform2DSaveData(ioReader, iTransformVersion);

            if(ioReader.ReadBoolean())
                _itemSaveData = new ItemSaveData(ioReader, iItemVersion);
            else
                _itemSaveData = null;
        }

        public Transform2DSaveData Transform => _transform;
        public ItemSaveData? Item => _itemSaveData;
    }

    [Serializable]
    public struct SplitterSaveData
    {
        // Versions :
        // 0 : init
        public const uint Version = 0;

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

        public void WriteBin(BinaryWriter ioWriter)
        {
            _transform.WriteBin(ioWriter);

            ioWriter.Write(_itemSaveData.HasValue);
            if(_itemSaveData.HasValue)
                _itemSaveData.Value.WriteBin(ioWriter);

            ioWriter.Write(_outputIdx);
        }

        public SplitterSaveData(BinaryReader ioReader, uint iSplitterVersion, uint iTransformVersion, uint iItemVersion)
        {
            if(iSplitterVersion > Version)
                throw new UnsupportedVersion();

            _transform = new Transform2DSaveData(ioReader, iTransformVersion);

            if(ioReader.ReadBoolean())
                _itemSaveData = new ItemSaveData(ioReader, iItemVersion);
            else
                _itemSaveData = null;

            _outputIdx = ioReader.ReadInt32();
        }

        public Transform2DSaveData Transform => _transform;
        public ItemSaveData? Item => _itemSaveData;
        public int OutputIndex => _outputIdx;
    }

    [Serializable]
    public struct MergerSaveData
    {
        // Versions :
        // 0 : init
        public const uint Version = 0;

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

        public void WriteBin(BinaryWriter ioWriter)
        {
            _transform.WriteBin(ioWriter);

            ioWriter.Write(_itemSaveData.HasValue);
            if(_itemSaveData.HasValue)
                _itemSaveData.Value.WriteBin(ioWriter);

            ioWriter.Write(_inputIdx);
        }

        public MergerSaveData(BinaryReader ioReader, uint iMergerVersion, uint iTransformVersion, uint iItemVersion)
        {
            if(iMergerVersion > Version)
                throw new UnsupportedVersion();

            _transform = new Transform2DSaveData(ioReader, iTransformVersion);

            if(ioReader.ReadBoolean())
                _itemSaveData = new ItemSaveData(ioReader, iItemVersion);
            else
                _itemSaveData = null;

            _inputIdx = ioReader.ReadInt32();
        }

        public Transform2DSaveData Transform => _transform;
        public ItemSaveData? Item => _itemSaveData;
        public int InputIndex => _inputIdx;
    }

    [Serializable]
    public struct MachineSaveData
    {
        // Versions :
        // 0 : init
        public const uint Version = 0;

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

        public void WriteBin(BinaryWriter ioWriter)
        {
            _transform.WriteBin(ioWriter);

            ioWriter.Write(_craftedItem.HasValue);
            if(_craftedItem.HasValue)
                _craftedItem.Value.WriteBin(ioWriter);

            ioWriter.Write(_itemToCraftName);
            ioWriter.Write(_leftCraftTime);
            Helper.WriteList(_itemNames, ioWriter, (writer, s) => writer.Write(s));
            Helper.WriteList(_itemQuantity, ioWriter, (writer, i) => writer.Write(i));
            Helper.WriteList(_itemsInTransfer, ioWriter, (writer, it) => it.WriteBin(ioWriter));
        }

        public MachineSaveData(BinaryReader ioReader, uint iMachineVersion, uint iTransformVersion, uint iItemVersion)
        {
            if(iMachineVersion > Version)
                throw new UnsupportedVersion();

            _transform = new Transform2DSaveData(ioReader, iTransformVersion);

            if(ioReader.ReadBoolean())
                _craftedItem = new ItemSaveData(ioReader, iItemVersion);
            else
                _craftedItem = null;

            _itemToCraftName = ioReader.ReadString();
            _leftCraftTime = ioReader.ReadSingle();
            _itemNames = Helper.ReadList(ioReader, (reader) => reader.ReadString());
            _itemQuantity = Helper.ReadList(ioReader, (reader) => reader.ReadInt32());
            _itemsInTransfer = Helper.ReadList(ioReader, (reader) => new ItemSaveData(reader, iItemVersion));
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
        // Versions :
        // 0 : init
        public const uint Version = 0;

        private Transform2DSaveData _transform;
        private List<ItemSaveData> _itemsInTransfer;

        public SellerSaveData(Seller iSeller)
        {
            _transform = new Transform2DSaveData(iSeller.transform);

            _itemsInTransfer = new List<ItemSaveData>();
            foreach(Item itemInTransfer in iSeller.GetItemsInTransfer())
                _itemsInTransfer.Add(new ItemSaveData(itemInTransfer));
        }

        public void WriteBin(BinaryWriter ioWriter)
        {
            _transform.WriteBin(ioWriter);
            Helper.WriteList(_itemsInTransfer, ioWriter, (writer, it) => it.WriteBin(ioWriter));
        }

        public SellerSaveData(BinaryReader ioReader, uint iSellerVersion, uint iTransformVersion, uint iItemVersion)
        {
            if(iSellerVersion > Version)
                throw new UnsupportedVersion();

            _transform = new Transform2DSaveData(ioReader, iTransformVersion);
            _itemsInTransfer = Helper.ReadList(ioReader, (reader) => new ItemSaveData(reader, iItemVersion));
        }

        public Transform2DSaveData Transform => _transform;
        public IEnumerable<ItemSaveData> ItemsInTransfer => _itemsInTransfer;
    }

    [Serializable]
    public struct SpawnerSaveData
    {
        // Versions :
        // 0 : init
        public const uint Version = 0;

        private Transform2DSaveData _transform;
        private float _leftSpawnTime;

        public SpawnerSaveData(Spawner iSpawner)
        {
            _transform = new Transform2DSaveData(iSpawner.transform);
            _leftSpawnTime = iSpawner.GetTimeForNextSpawn();
        }

        public void WriteBin(BinaryWriter ioWriter)
        {
            _transform.WriteBin(ioWriter);
            ioWriter.Write(_leftSpawnTime);
        }

        public SpawnerSaveData(BinaryReader ioReader, uint iSpawnerVersion, uint iTransformVersion, uint iItemVersion)
        {
            if(iSpawnerVersion > Version)
                throw new UnsupportedVersion();

            _transform = new Transform2DSaveData(ioReader, iTransformVersion);
            _leftSpawnTime = ioReader.ReadSingle();
        }

        public Transform2DSaveData Transform => _transform;
        public float TimeToNextSpawn => _leftSpawnTime;
    }

    [Serializable]
    public struct PlayerSaveData
    {
        // Versions :
        // 0 : init
        public const uint Version = 0;

        private int _extractLevel;
        private int _beltLevel;
        private int _craftLevel;
        private int _money;
        private int _seed;
        private Transform2DSaveData _cameraTransform;
        private float _cameraOrthoSize;
        private List<KeyValuePair<string, bool>> _itemUnlocks;

        public PlayerSaveData(bool _)
        {
            _extractLevel = StatManager.instance.GetStatLevel(StatManager.ExtractSpeedIndex);
            _beltLevel = StatManager.instance.GetStatLevel(StatManager.BeltSpeedIndex);
            _craftLevel = StatManager.instance.GetStatLevel(StatManager.CraftSpeedIndex);
            _money = Wallet.instance.Money;
            _seed = MapGenerator.instance.GetSeed();
            Camera camera = Camera.main;
            _cameraTransform = new Transform2DSaveData(camera.transform);
            _cameraOrthoSize = camera.orthographicSize;
            IEnumerable<ItemData> items = ItemCreator.LoadAllResourceAtPath<ItemData>();
            _itemUnlocks = items.Select(i => new KeyValuePair<string, bool>(i.Name, i.Unlocked)).ToList();
        }

        public void WriteBin(BinaryWriter ioWritter)
        {
            ioWritter.Write(_extractLevel);
            ioWritter.Write(_beltLevel);
            ioWritter.Write(_craftLevel);
            ioWritter.Write(_money);
            ioWritter.Write(_seed);
            _cameraTransform.WriteBin(ioWritter);
            ioWritter.Write(_cameraOrthoSize);
            Helper.WriteList(_itemUnlocks, ioWritter, (writter, kv) =>
                {
                    writter.Write(kv.Key);
                    writter.Write(kv.Value);
                });
        }

        public PlayerSaveData(BinaryReader ioReader, uint iPlayerVersion, uint iTransformVersion)
        {
            if(iPlayerVersion > Version)
                throw new UnsupportedVersion();

            _extractLevel = ioReader.ReadInt32();
            _beltLevel = ioReader.ReadInt32();
            _craftLevel = ioReader.ReadInt32();
            _money = ioReader.ReadInt32();
            _seed = ioReader.ReadInt32();
            _cameraTransform = new Transform2DSaveData(ioReader, iTransformVersion);
            _cameraOrthoSize = ioReader.ReadSingle();
            _itemUnlocks = Helper.ReadList(ioReader, (reader) => new KeyValuePair<string, bool>(reader.ReadString(), reader.ReadBoolean()));
        }

        public int ExtractLevel => _extractLevel;
        public int BeltLevel => _beltLevel;
        public int CraftLevel => _craftLevel;
        public int Money => _money;
        public int Seed => _seed;
        public Transform2DSaveData CameraTransform => _cameraTransform;
        public float CameraOrthoSize => _cameraOrthoSize;
        public List<KeyValuePair<string, bool>> ItemUnlocks => _itemUnlocks;
    }

    #endregion
}