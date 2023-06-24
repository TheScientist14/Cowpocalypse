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

        private const string Filename = "Cowpocalypse.noext";
        private string _path;

        private const int LastVersion = 1;

        [SerializeField] private float loadTime;
        private GameObject _playerSpawnedObjects;

        private Dictionary<string, ItemData> _itemDatas;

        public UnityEvent savedGame;
        public UnityEvent loadedGame;

        private JobHandle _jobHandle;
        private GameObject _saveIcon;

        private void Awake()
        {
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
            Debug.Log("Game Saved");
            _saveIcon.SetActive(false);
        }

        private void OnGameLoaded()
        {
            _saveIcon.transform.GetChild(0).gameObject.SetActive(false);
            _saveIcon.SetActive(false);
            InputStateMachine.instance.SetState(new FreeViewState());

            Debug.Log("Game Loaded");
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

            Debug.Log("Game Save async Started : { Thread : " + Thread.CurrentThread.ManagedThreadId + " }");
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

                Debug.Log("Game Saved: { Thread : " + Thread.CurrentThread.ManagedThreadId + " }");
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
            Debug.Log("Loading...");
            foreach(Transform child in _playerSpawnedObjects.transform)
                Destroy(child.gameObject);

            SaveData data = GetSavedGameData();

            LoadPlayer(data);
            Debug.Log("Loaded Player");
            LoadMachines(data);
            Debug.Log("Loaded Machines");
            LoadBelts(data);
            Debug.Log("Loaded Belts");
            LoadSplitters(data);
            Debug.Log("Loaded Splitters");
            LoadMergers(data);
            Debug.Log("Loaded Mergers");
            LoadSellers(data);
            Debug.Log("Loaded Sellers");
            ResetSpawners(data);
            Debug.Log("Reset Spawners");

            // this is only to have loading time perceptible
            yield return new WaitForSecondsRealtime(loadTime);

            loadedGame.Invoke();
            yield return null;
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
                    Debug.Log("Save file version is not supported");
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

        public void LoadPlayer(SaveData iData)
        {
            PlayerSaveData playerSaveData = iData.PlayerSaveData;

            for(int i = 0; i < playerSaveData.Stats.Count; i++)
                StatManager.instance.Stats[i].CurrentLevel = playerSaveData.Stats[i].CurrentLevel;

            Wallet.instance.Money = playerSaveData.Money;
        }

        public void LoadBelts(SaveData iData)
        {
            foreach(BeltSaveData beltSaveData in iData.BeltDatas)
            {
                NewBelt belt = Instantiate(beltPrefab, beltSaveData.Transform.Pos, beltSaveData.Transform.Rot, _playerSpawnedObjects.transform)
                    .GetComponent<NewBelt>();

                if(beltSaveData.Item.HasValue)
                {
                    belt.SetItemInTransfer(
                        PoolManager.instance.SpawnObject(_itemDatas[beltSaveData.Item.Value.Name], beltSaveData.Item.Value.Pos));
                }
            }
        }

        public void LoadMachines(SaveData iData)
        {
            foreach(MachineSaveData machineSaveData in iData.MachineDatas)
            {
                NewMachine machine =
                    Instantiate(machinePrefab, machineSaveData.Transform.Pos, machineSaveData.Transform.Rot, _playerSpawnedObjects.transform)
                        .GetComponent<NewMachine>();

                machine.SetCurrentStock(new Dictionary<ItemData, int>(machineSaveData.ItemNames.Zip(
                    machineSaveData.ItemQuantity, (name, quantity) => new KeyValuePair<ItemData, int>(_itemDatas[name], quantity))));

                if(machineSaveData.ItemToCraftName != "")
                    machine.SetCraftedItem(_itemDatas[machineSaveData.ItemToCraftName]);

                machine.SetTimeLeftForCurrentCraft(machineSaveData.TimeLeftToCraft);

                if(machineSaveData.CraftedItem.HasValue)
                {
                    machine.SetAlreadyCraftedItem(
                        PoolManager.instance.SpawnObject(
                            _itemDatas[machineSaveData.CraftedItem.Value.Name],
                            machineSaveData.CraftedItem.Value.Pos));
                }

                List<Item> itemsInTransfer = new List<Item>();
                foreach(ItemSaveData itemData in machineSaveData.ItemsInTransfer)
                    itemsInTransfer.Add(PoolManager.instance.SpawnObject(_itemDatas[itemData.Name], itemData.Pos));
                machine.AddItemsInTransfer(itemsInTransfer);

                ItemHandlerManager.instance.MachineCount++;
            }
        }

        public void LoadSplitters(SaveData iData)
        {
            foreach(SplitterSaveData splitterSaveData in iData.SplitterDatas)
            {
                NewSplitter splitter =
                    Instantiate(splitterPrefab, splitterSaveData.Transform.Pos, splitterSaveData.Transform.Rot, _playerSpawnedObjects.transform)
                        .GetComponent<NewSplitter>();

                if(splitterSaveData.Item.HasValue)
                {
                    splitter.SetItemInTransfer(
                        PoolManager.instance.SpawnObject(_itemDatas[splitterSaveData.Item.Value.Name], splitterSaveData.Item.Value.Pos));
                }

                splitter.SetCurrentOutputIndex(splitterSaveData.OutputIndex);
            }
        }

        public void LoadMergers(SaveData iData)
        {
            foreach(MergerSaveData mergerSaveData in iData.MergerDatas)
            {
                NewMerger merger =
                    Instantiate(mergerPrefab, mergerSaveData.Transform.Pos, mergerSaveData.Transform.Rot, _playerSpawnedObjects.transform)
                        .GetComponent<NewMerger>();

                if(mergerSaveData.Item.HasValue)
                {
                    merger.SetItemInTransfer(
                        PoolManager.instance.SpawnObject(_itemDatas[mergerSaveData.Item.Value.Name], mergerSaveData.Item.Value.Pos));
                }

                merger.SetCurrentInputIndex(mergerSaveData.InputIndex);
            }
        }

        public void LoadSellers(SaveData iData)
        {
            foreach(SellerSaveData sellerSaveData in iData.SellerDatas)
            {
                NewSeller seller =
                    Instantiate(sellerPrefab, sellerSaveData.Transform.Pos, sellerSaveData.Transform.Rot, _playerSpawnedObjects.transform)
                        .GetComponent<NewSeller>();

                List<Item> itemsInTransfer = new List<Item>();
                foreach(ItemSaveData itemData in sellerSaveData.ItemsInTransfer)
                    itemsInTransfer.Add(PoolManager.instance.SpawnObject(_itemDatas[itemData.Name], itemData.Pos));
                seller.AddItemsInTransfer(itemsInTransfer);

                ItemHandlerManager.instance.ShopCount++;

            }
        }

        public void ResetSpawners(SaveData iData)
        {
            // this code assumes that there is no more than one spawner per item type
            // and that there is none with null spawned item data
            Dictionary<string, NewSpawner> spawners = new Dictionary<string, NewSpawner>();
            foreach(NewSpawner spawner in FindObjectsOfType<NewSpawner>())
            {
                ItemData itemData = spawner.GetItemDataToSpawn();
                Assert.IsNotNull(itemData);
                Assert.IsFalse(spawners.ContainsKey(itemData.Name));
                spawners[itemData.Name] = spawner;
            }

            foreach(SpawnerSaveData spawnerSaveData in iData.SpawnerDatas)
            {
                if(spawnerSaveData.ItemNameToSpawn == "" || spawners.ContainsKey(spawnerSaveData.ItemNameToSpawn))
                    continue;

                if(spawnerSaveData.SpawnedItem.HasValue)
                {
                    spawners[spawnerSaveData.ItemNameToSpawn].InitSpawnTime(0);
                    continue;
                }

                spawners[spawnerSaveData.ItemNameToSpawn].InitSpawnTime(spawnerSaveData.TimeToNextSpawn);
            }
        }

        public void OverideSave()
        {
            File.Delete(_path);
        }
    }
}