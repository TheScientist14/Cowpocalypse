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
using File = UnityEngine.Windows.File;

namespace _Scripts.Save_System
{
    public class SaveSystem : Singleton<SaveSystem>
    {
        [SerializeField]
        private GameObject saveIconPrefab;

        [ShowAssetPreview()] [SerializeField]
        private GameObject beltPrefab;

        [ShowAssetPreview()] [SerializeField]
        private GameObject machinePrefab;
        
        [ShowAssetPreview()] [SerializeField]
        private GameObject splitterPrefab;
        
        [ShowAssetPreview()] [SerializeField]
        private GameObject mergerPrefab;

        private const string Filename = "Cowpocalypse.noext";
        private static string _path;

        private Dictionary<string, ItemData> _itemDatas;
        public List<Belt> belts;
        public static List<Belt> beltsStatic;

        public UnityEvent savedGame;
        public UnityEvent loadedGame;

        private JobHandle _jobHandle;
        private GameObject _saveIcon;

        private void Awake()
        {
            var sOs = ItemCreator.LoadAllResourceAtPath<ItemData>();

            _itemDatas = sOs.ToDictionary(i => i.Name);

            _path = Application.persistentDataPath + "/" + Filename;
            
            savedGame.AddListener(OnGameSaved);
            loadedGame.AddListener(OnGameLoaded);
        }

        private void OnGameSaved()
        {
            Debug.Log("Game Saved");
            _saveIcon.SetActive(false);
        }

        private void OnGameLoaded()
        {
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
            if (_saveIcon == null)
            {
                _saveIcon = Instantiate(saveIconPrefab);
            }
            else
            {
                _saveIcon.SetActive(true);
            }
            
            Debug.Log("Game Save async Started : { Thread : " + Thread.CurrentThread.ManagedThreadId + " }");
            SaveData data = new SaveData();
            await Task.Run(() =>
            {
                BinaryFormatter formatter = new BinaryFormatter();

                using (FileStream stream = new FileStream(_path, FileMode.Create))
                {
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
            
            using (FileStream stream = new FileStream(_path, FileMode.Create))
            {
                formatter.Serialize(stream, data);
            }
            savedGame.Invoke();
        }

        [Button("Load Game")]
        public void LoadGame()
        {
            LoadMachines();
            LoadBelts();
            LoadSplitters();
            LoadPlayer();
            LoadMerger();

            loadedGame.Invoke();
        }
        
        public SaveData GetSavedGameData()
        {
            if (File.Exists(_path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(_path, FileMode.Open);

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

        [Button("Load Belts and Items")]
        public void LoadBelts()
        {
            foreach (BeltSaveData beltSaveData in GetSavedGameData().BeltDatas)
            {
                Belt belt = Instantiate(beltPrefab, beltSaveData.GetPos, Quaternion.Euler(beltSaveData.GetRot))
                    .GetComponent<Belt>();

                if (beltSaveData.GetItem.GetValueOrDefault().GetName != null)
                {
                    belt.BeltItem = PoolManager.instance.SpawnObject(
                        _itemDatas[beltSaveData.GetItem.GetValueOrDefault().GetName],
                        beltSaveData.GetItem.GetValueOrDefault().GetPos);
                }
            }
        }

        [Button("Load Machines")]
        public void LoadMachines()
        {
            foreach (MachineSaveData machineSaveData in GetSavedGameData().MachineDatas)
            {
                Machine machine =
                    Instantiate(machinePrefab, machineSaveData.GetPos, Quaternion.Euler(machineSaveData.GetRot))
                        .GetComponent<Machine>();

                machine.Stock = new Dictionary<ItemData, int>(machineSaveData.ItemNames.Zip(
                    machineSaveData.ItemQuantity, (e1, e2) => new KeyValuePair<ItemData, int>(_itemDatas[e1], e2)));
                
                if (machineSaveData.ItemToCraftName != "")
                {
                    machine.SetCrafteditem(_itemDatas[machineSaveData.ItemToCraftName]);
                }

                if (machineSaveData.GetItem.GetValueOrDefault().GetName != null)
                {
                    machine.BeltItem = PoolManager.instance.SpawnObject(
                        _itemDatas[machineSaveData.GetItem.GetValueOrDefault().GetName],
                        machineSaveData.GetItem.GetValueOrDefault().GetPos);
                }
            }
        }

        public void LoadSplitters()
        {
            foreach (SplitterSaveData splitterSaveData in GetSavedGameData().SplitterDatas)
            {
                Splitter splitter =
                    Instantiate(splitterPrefab, splitterSaveData.GetPos, Quaternion.Euler(splitterSaveData.GetRot))
                        .GetComponent<Splitter>();

                if (splitterSaveData.GetItem.GetValueOrDefault().GetName != null)
                {
                    splitter.BeltItem = PoolManager.instance.SpawnObject(
                        _itemDatas[splitterSaveData.GetItem.GetValueOrDefault().GetName],
                        splitterSaveData.GetItem.GetValueOrDefault().GetPos);
                }
            }
        }

        public void LoadMerger()
        {
            foreach (MergerSaveData mergerSaveData in GetSavedGameData().MergerDatas)
            {
                Merger merger =
                    Instantiate(mergerPrefab, mergerSaveData.GetPos, Quaternion.Euler(mergerSaveData.GetRot))
                        .GetComponent<Merger>();
                
                if (mergerSaveData.GetItem.GetValueOrDefault().GetName != null)
                {
                    merger.BeltItem = PoolManager.instance.SpawnObject(
                        _itemDatas[mergerSaveData.GetItem.GetValueOrDefault().GetName],
                        mergerSaveData.GetItem.GetValueOrDefault().GetPos);
                }
                
            }
        }

        public void LoadPlayer()
        {
            PlayerSaveData playerSaveData = GetSavedGameData().PlayerSaveData;

            for (int i = 0; i < playerSaveData.Stats.Count; i++)
            {
                StatManager.instance.Stats[i].CurrentLevel = playerSaveData.Stats[i].CurrentLevel;
            }

            Wallet.instance.Money = playerSaveData.Money;
        }
    }
}