using System;
using UnityEngine;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using _Scripts.Pooling_System;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine.Events;
using File = UnityEngine.Windows.File;

namespace _Scripts.Save_System
{
    public class SaveSystem : Singleton<SaveSystem>
    {
        private static string _filename = "Cowpocalypse.noext";
        private string _path;
        public UnityEvent savedGame;
        public UnityEvent loadedGame;

        private void Awake()
        {
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

        public void LoadBelts()
        {

            foreach (BeltSaveData beltSaveData in GetSavedGameData().BeltDatas)
            {
                Instantiate(new GameObject().AddComponent<Belt>(), beltSaveData.GetPos(), Quaternion.Euler(beltSaveData.GetRot()));
                //PoolManager.instance.SpawnObject(,beltSaveData.GetItem().GetPos());
            }
            
        }
        
        
    }
}