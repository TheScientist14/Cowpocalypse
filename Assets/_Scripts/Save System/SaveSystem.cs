using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using _Scripts.Pooling_System;
using NaughtyAttributes;
using Unity.Jobs;
using UnityEngine.Events;
using UnityEngine.Assertions;

namespace _Scripts.Save_System
{
    public class SaveSystem : Singleton<SaveSystem>
    {
        [SerializeField] public static bool _loadOnStartup = false;

        [SerializeField]
        private GameObject saveIconPrefab;

        [ShowAssetPreview()]
        [SerializeField]
        private GameObject beltPrefab;

        [ShowAssetPreview()]
        [SerializeField]
        private GameObject machinePrefab;

        [ShowAssetPreview()]
        [SerializeField]
        private GameObject splitterPrefab;

        [ShowAssetPreview()]
        [SerializeField]
        private GameObject mergerPrefab;

        [ShowAssetPreview()]
        [SerializeField]
        private GameObject sellerPrefab;

        [ShowAssetPreview()]
        [SerializeField]
        private GameObject spawnerPrefab;

        private const string Filename = "Cowpocalypse.noext";
        private string _path;

        private const int LastVersion = 1;

        [SerializeField] private float loadTime;
        private GameObject _playerSpawnedObjects;

        private Dictionary<string, ItemData> _itemDatas;

        public UnityEvent savedGame;
        public UnityEvent loadedGame;

        private GameObject _saveIcon;

        protected override void Awake()
        {
            base.Awake();

            var sOs = ItemCreator.LoadAllResourceAtPath<ItemData>();

            _itemDatas = sOs.ToDictionary(i => i.Name);

            _path = Application.persistentDataPath + "/" + Filename;

            if(_saveIcon == null)
                _saveIcon = Instantiate(saveIconPrefab);
            _saveIcon.SetActive(false);

            _playerSpawnedObjects = GameObject.FindWithTag("Map");
        }

        private void Start()
        {
            savedGame.AddListener(OnGameSaved);
            loadedGame.AddListener(OnGameLoaded);

            if(_loadOnStartup)
            {
                LoadGame();
                _loadOnStartup = false;
            }
        }

        private void OnGameSaved()
        {
            // Debug.Log("Game Saved");
            _saveIcon.SetActive(false);
        }

        private void OnGameLoaded()
        {
            _saveIcon.transform.GetChild(0).gameObject.SetActive(false);
            _saveIcon.SetActive(false);
            InputStateMachine.instance.SetState(new FreeViewState());

            // Debug.Log("Game Loaded");
        }

        [Button("Save Game async")]
        public void SaveGame()
        {
#pragma warning disable CS4014 // Don't want to await, it should block gameplay;
            SaveGameAsync();
#pragma warning restore CS4014
        }

        private async Task SaveGameAsync()
        {
            _saveIcon.SetActive(true);

            // Debug.Log("Game Save async Started : { Thread : " + Thread.CurrentThread.ManagedThreadId + " }");
            SaveData data = new SaveData();
            await Task.Run(() =>
            {
                BinaryFormatter formatter = new BinaryFormatter();

                using(FileStream stream = new FileStream(_path, FileMode.Create))
                {
                    int? Version = LastVersion;
                    formatter.Serialize(stream, Version);
                    formatter.Serialize(stream, data);
                }

                // Debug.Log("Game Saved: { Thread : " + Thread.CurrentThread.ManagedThreadId + " }");
            });
            savedGame.Invoke();
        }

        [Button("Save Game non async")]
        public void SaveNonAsync()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            SaveData data = new SaveData();

            using(FileStream stream = new FileStream(_path, FileMode.Create))
            {
                formatter.Serialize(stream, data);
            }
            savedGame.Invoke();
        }

        [Button("Load Game")]
        public void LoadGame()
        {
            _saveIcon.SetActive(true);
            _saveIcon.transform.GetChild(0).gameObject.SetActive(true);
            InputStateMachine.instance.SetState(new PauseState());

            StartCoroutine(Load());
        }

        private IEnumerator Load()
        {
            foreach(Transform child in _playerSpawnedObjects.transform)
                Destroy(child.gameObject);

            yield return new WaitForEndOfFrame(); // wait for destroys to be effective

            SaveData data = GetSavedGameData();

            Wallet.instance.Money = int.MaxValue; // avoid errors about negative money when spawning machines & extractors

            foreach(MachineSaveData machineData in data.MachineDatas)
                LoadMachine(machineData);

            foreach(BeltSaveData beltData in data.BeltDatas)
                LoadBelt(beltData);

            foreach(SplitterSaveData splitterData in data.SplitterDatas)
                LoadSplitter(splitterData);

            foreach(MergerSaveData mergerData in data.MergerDatas)
                LoadMerger(mergerData);

            foreach(SellerSaveData sellerData in data.SellerDatas)
                LoadSeller(sellerData);

            foreach(SpawnerSaveData spawnerData in data.SpawnerDatas)
                LoadSpawner(spawnerData);

            yield return new WaitForEndOfFrame(); // wait for all Start functions to be called

            // important to load player after spawning spawners & machines
            // to reset money
            LoadPlayer(data.PlayerSaveData);

            // this is only to have loading time perceptible
            yield return new WaitForSecondsRealtime(loadTime);

            loadedGame.Invoke();
        }

        public bool CheckForSave()
        {
            return File.Exists(_path);
        }

        public SaveData GetSavedGameData()
        {
            if(CheckForSave())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(_path, FileMode.Open);

                int? Version = formatter.Deserialize(stream) as int?;
                if(!Version.HasValue || Version.Value < 0 || Version.Value > LastVersion)
                {
                    Debug.LogWarning("Save file version is not supported");
                    return null;
                }

                SaveData data = formatter.Deserialize(stream) as SaveData;

                stream.Close();
                return data;
            }
            else
            {
                Debug.Log("Save file not found in " + _path);
                return null;
            }
        }

        public void LoadPlayer(PlayerSaveData iData)
        {
            StatManager.instance.SetLevelToStat(StatManager.ExtractSpeedIndex, iData.ExtractLevel);
            StatManager.instance.SetLevelToStat(StatManager.BeltSpeedIndex, iData.BeltLevel);
            StatManager.instance.SetLevelToStat(StatManager.CraftSpeedIndex, iData.CraftLevel);
            Wallet.instance.Money = iData.Money;
            MapGenerator.instance.InitMap(iData.Seed);
        }

        public Belt LoadBelt(BeltSaveData iData)
        {
            Belt belt = Instantiate(beltPrefab, iData.Transform.Pos, iData.Transform.Rot, _playerSpawnedObjects.transform)
                .GetComponent<Belt>();

            if(iData.Item.HasValue)
                belt.SetItemInTransfer(LoadItem(iData.Item.Value));

            return belt;
        }

        public Machine LoadMachine(MachineSaveData iData)
        {
            Machine machine = Instantiate(machinePrefab, iData.Transform.Pos, iData.Transform.Rot, _playerSpawnedObjects.transform)
                    .GetComponent<Machine>();

            machine.SetCurrentStock(new Dictionary<ItemData, int>(iData.ItemNames.Zip(
                iData.ItemQuantity, (name, quantity) => new KeyValuePair<ItemData, int>(_itemDatas[name], quantity))));

            if(iData.ItemToCraftName != "")
                machine.SetCraftedItem(_itemDatas[iData.ItemToCraftName]);

            machine.SetTimeLeftForCurrentCraft(iData.TimeLeftToCraft);

            if(iData.CraftedItem.HasValue)
                machine.SetAlreadyCraftedItem(LoadItem(iData.CraftedItem.Value));

            List<Item> itemsInTransfer = new List<Item>();
            foreach(ItemSaveData itemData in iData.ItemsInTransfer)
                itemsInTransfer.Add(LoadItem(itemData));
            machine.AddItemsInTransfer(itemsInTransfer);

            return machine;
        }

        public Splitter LoadSplitter(SplitterSaveData iData)
        {
            Splitter splitter = Instantiate(splitterPrefab, iData.Transform.Pos, iData.Transform.Rot, _playerSpawnedObjects.transform)
                        .GetComponent<Splitter>();

            if(iData.Item.HasValue)
                splitter.SetItemInTransfer(LoadItem(iData.Item.Value));

            splitter.SetCurrentOutputIndex(iData.OutputIndex);

            return splitter;
        }

        public Merger LoadMerger(MergerSaveData iData)
        {
            Merger merger = Instantiate(mergerPrefab, iData.Transform.Pos, iData.Transform.Rot, _playerSpawnedObjects.transform)
                    .GetComponent<Merger>();

            if(iData.Item.HasValue)
                merger.SetItemInTransfer(LoadItem(iData.Item.Value));

            merger.SetCurrentInputIndex(iData.InputIndex);

            return merger;
        }

        public Seller LoadSeller(SellerSaveData iData)
        {
            Seller seller = Instantiate(sellerPrefab, iData.Transform.Pos, iData.Transform.Rot, _playerSpawnedObjects.transform)
                    .GetComponent<Seller>();

            List<Item> itemsInTransfer = new List<Item>();
            foreach(ItemSaveData itemData in iData.ItemsInTransfer)
                itemsInTransfer.Add(LoadItem(itemData));
            seller.AddItemsInTransfer(itemsInTransfer);

            return seller;
        }

        public Spawner LoadSpawner(SpawnerSaveData iData)
        {
            Spawner spawner = Instantiate(spawnerPrefab, iData.Transform.Pos, iData.Transform.Rot, _playerSpawnedObjects.transform)
                .GetComponent<Spawner>();
            spawner.InitSpawnTime(Mathf.Max(iData.TimeToNextSpawn, 0));

            return spawner;
        }

        public Item LoadItem(ItemSaveData iData)
        {
            return PoolManager.instance.SpawnObject(_itemDatas[iData.Name], iData.Pos);
        }

        public void OverideSave()
        {
            File.Delete(_path);
        }
    }
}