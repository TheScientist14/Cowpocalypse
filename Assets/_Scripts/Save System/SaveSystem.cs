using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using _Scripts.Pooling_System;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine.Serialization;
using File = UnityEngine.Windows.File;

namespace _Scripts.Save_System
{
    public class SaveSystem : Singleton<SaveSystem>
    {
        [SerializeField] private GameObject beltPrefab;
        
        private static string _filename = "Cowpocalypse.noext";
        private string _path;

        private Dictionary<string, ItemData> _itemDatas;

        public UnityEvent savedGame;
        public UnityEvent loadedGame;

        private void Awake()
        {
            var _sOs = ItemCreator.LoadAllItemsAtPath<ItemData>("Assets/Scriptable objects/Items/");

            _itemDatas = _sOs.ToDictionary(i => i.Name);
            
            _path = Application.persistentDataPath + "/" + _filename;
        }
        
        [Button("Save Game")]
        public void SaveGame()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(_path, FileMode.Create);

            SaveData data = new SaveData();
            formatter.Serialize(stream, data);
            stream.Close();
        }

        [Button("Get Saved Game Data")]
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
                var belt = Instantiate(beltPrefab, beltSaveData.GetPos(), Quaternion.Euler(beltSaveData.GetRot())).GetComponent<Belt>();

                if (beltSaveData.GetItem().GetValueOrDefault().GetName() != null)
                {
                   belt.BeltItem = PoolManager.instance.SpawnObject(_itemDatas[beltSaveData.GetItem().GetValueOrDefault().GetName()],beltSaveData.GetItem().GetValueOrDefault().GetPos());
                }
                
            }
            
        }
    }
}