using UnityEngine;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using File = UnityEngine.Windows.File;

namespace _Scripts.Save_System
{
    public static class SaveSystem
    {
        private static string _filename = "Cowpocalypse.noext";
        private static string _path = Application.persistentDataPath +"/"+ _filename;
        
        public static void SaveGame(SaveData prmSaveData)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            
            FileStream stream = new FileStream(_path, FileMode.Create);

            SaveData data = new SaveData();
            formatter.Serialize(stream, data);
            stream.Close();
        }

        public static SaveData LoadGame()
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
    }
}