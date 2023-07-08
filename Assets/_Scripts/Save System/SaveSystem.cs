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
using Unity.VisualScripting;
using System.Text;
using UnityEngine.Analytics;

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

        [SerializeField] private float loadTime;
        private GameObject _playerSpawnedObjects;

        private Dictionary<string, ItemData> _itemDatas;

        public UnityEvent savedGame;
        public UnityEvent loadedGame;

        private GameObject _saveIcon;

        protected override void Awake()
        {
            base.Awake();

            if(savedGame == null)
                savedGame = new UnityEvent();
            if(loadedGame == null)
                loadedGame = new UnityEvent();
        }

        private void Start()
        {
            var sOs = ItemCreator.LoadAllResourceAtPath<ItemData>();
            _itemDatas = sOs.ToDictionary(i => i.Name);

            _path = Application.persistentDataPath + "/" + Filename;

            if(_saveIcon == null)
                _saveIcon = Instantiate(saveIconPrefab);
            _saveIcon.SetActive(false);

            _playerSpawnedObjects = GameObject.FindWithTag("Map");

            if(_loadOnStartup)
            {
                LoadGame();
                _loadOnStartup = false;
            }
        }

        private void OnGameSaved()
        {
            _saveIcon.SetActive(false);
        }

        private void OnGameLoaded()
        {
            _saveIcon.transform.GetChild(0).gameObject.SetActive(false);
            _saveIcon.SetActive(false);
            InputStateMachine.instance.SetState(new FreeViewState());
        }

        public void SaveGame()
        {
#pragma warning disable CS4014 // Don't want to await, it should block gameplay;
            SaveGameAsync();
#pragma warning restore CS4014
        }

        private async Task SaveGameAsync()
        {
            _saveIcon.SetActive(true);

            SaveData data = new SaveData(true);
            bool success = false;
            await Task.Run(() => success = _SaveData(data));

            OnGameSaved();

            if(!success)
                return;

            savedGame.Invoke();
        }

        public bool SaveNonAsync()
        {
            SaveData data = new SaveData(true);
            if(!_SaveData(data))
                return false;

            savedGame.Invoke();
            return true;
        }

        public bool _SaveData(SaveData iData)
        {
            bool success = false;
            FileStream stream = null;
            BinaryWriter writer = null;
            try
            {
                stream = File.Create(_path);
                writer = new BinaryWriter(stream, Encoding.UTF8, false);

                iData.WriteBin(writer);
                success = true;
            }
            catch(Exception)
            {
                success = false;
            }
            finally
            {
                if(stream != null)
                    stream.Dispose();
                if(writer != null)
                    writer.Dispose();
            }

            return success;
        }

        public void LoadGame()
        {
            _saveIcon.SetActive(true);
            _saveIcon.transform.GetChild(0).gameObject.SetActive(true);
            InputStateMachine.instance.SetState(new PauseState());

            StartCoroutine(Load());
        }

        private IEnumerator Load()
        {
            string errMsg = "";
            SaveData data = GetSavedGameData(out errMsg);
            if(!errMsg.Equals(""))
                Debug.LogWarning(errMsg);
            if(data == null)
            {
                OnGameLoaded();
                yield break;
            }

            foreach(Transform child in _playerSpawnedObjects.transform)
                Destroy(child.gameObject);

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

            LoadPlayer(data.PlayerSaveData);

            // avoid errors about negative money when spawning machines & extractors
            Wallet.instance.Money = int.MaxValue / 2;

            OnGameLoaded();

            // reset money after all starts done
            yield return new WaitForEndOfFrame();
            Wallet.instance.Money = data.PlayerSaveData.Money;

            loadedGame.Invoke();
        }

        public bool CheckForSave()
        {
            return File.Exists(_path);
        }

        public SaveData GetSavedGameData(out string oMsg)
        {
            SaveData data = null;
            oMsg = "";
            if(CheckForSave())
            {
                FileStream stream = null;
                BinaryReader reader = null;
                try
                {
                    stream = File.OpenRead(_path);
                    reader = new BinaryReader(stream, Encoding.UTF8, false);

                    data = new SaveData(reader, out oMsg);
                }
                catch(Exception)
                {
                    data = null;
                    if(oMsg.Equals(""))
                    {
                        oMsg = "Unknown error, loading aborted.\n" +
                            "Might be a file access or corruption issue.";
                    }
                }
                finally
                {
                    if(stream != null)
                        stream.Dispose();
                    if(reader != null)
                        reader.Dispose();
                }
            }
            else
                oMsg = "Save file not found.";

            return data;
        }

        public void LoadPlayer(PlayerSaveData iData)
        {
            StatManager.instance.SetLevelToStat(StatManager.ExtractSpeedIndex, iData.ExtractLevel);
            StatManager.instance.SetLevelToStat(StatManager.BeltSpeedIndex, iData.BeltLevel);
            StatManager.instance.SetLevelToStat(StatManager.CraftSpeedIndex, iData.CraftLevel);
            Wallet.instance.Money = iData.Money;
            MapGenerator.instance.InitMap(iData.Seed);
            Camera camera = Camera.main;
            Vector3 camPos2D = iData.CameraTransform.Pos;
            camera.transform.position = new Vector3(camPos2D.x, camPos2D.y, camera.transform.position.z);
            camera.orthographicSize = iData.CameraOrthoSize;
            foreach(ItemData _item in _itemDatas.Values)
                _item.Unlocked = false;
            foreach(KeyValuePair<string, bool> itemUnlock in iData.ItemUnlocks)
                _itemDatas[itemUnlock.Key].Unlocked = itemUnlock.Value;
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

            splitter.UnsafeSetCurrentOutputIndex(iData.OutputIndex);

            return splitter;
        }

        public Merger LoadMerger(MergerSaveData iData)
        {
            Merger merger = Instantiate(mergerPrefab, iData.Transform.Pos, iData.Transform.Rot, _playerSpawnedObjects.transform)
                    .GetComponent<Merger>();

            if(iData.Item.HasValue)
                merger.SetItemInTransfer(LoadItem(iData.Item.Value));

            merger.UnsafeSetCurrentInputIndex(iData.InputIndex);

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