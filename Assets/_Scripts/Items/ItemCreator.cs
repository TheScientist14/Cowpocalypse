using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

public class ItemCreator : MonoBehaviour
{
    [SerializeField] TextAsset _csv;

#if UNITY_EDITOR
    [ContextMenu("Read CSV")]
    public void ReadCSV()
    {
        var text = _csv.text;

        foreach (var line in text.Split("\n"))
        {
            var item = ScriptableObject.CreateInstance<ItemData>();
            var values = line.Split(", ");

            item.Price = int.Parse(values[0]);
            item.Name = values[1];
            item.Tier = FindAsset<ItemTier>($"Tier {values[2]}");
            item.Sprite = FindAsset<Sprite>(item.Name);
            item.CraftDuration = float.Parse(values[3], CultureInfo.InvariantCulture);

            for (var i = 4; i < values.Length; i++)
            {
                if (int.TryParse(values[i], out var value))
                {
                    item.AddAmount(value);
                    continue;
                }

                item.AddParent(FindAsset<ItemData>(values[i]));
            }

            AssetDatabase.CreateAsset(item, $"Assets/Items/{item.Name.Trim()}.asset");
            AssetDatabase.SaveAssets();
        }
    }

    T FindAsset<T>(string name) where T : Object
    {
        var guid = AssetDatabase.FindAssets($"t:{typeof(T).Name} {name.Trim()}").FirstOrDefault();

        if (guid == null)
        {
            Debug.LogWarning($"Search : \"t:{typeof(T).Name} {name.Trim()}\" returned no results.");
            return null;
        }

        var path = AssetDatabase.GUIDToAssetPath(guid);
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }
    [Obsolete("For editor only, be careful")]
    public static T[] LoadAllItemsAtPath<T>(string path) where T : UnityEngine.Object
    {
        string[] itemAssetPaths = AssetDatabase.FindAssets("t:" + typeof(T).ToString(), new[] { path });
        T[] items = new T[itemAssetPaths.Length];
        for (int i = 0; i < itemAssetPaths.Length; i++)
        {
            string itemAssetPath = AssetDatabase.GUIDToAssetPath(itemAssetPaths[i]);
            items[i] = AssetDatabase.LoadAssetAtPath<T>(itemAssetPath);
        }
        return items;
    }
#endif
    public static IEnumerable<T> LoadAllResourceAtPath<T>(string path = "Scriptable objects/Items/") where T : UnityEngine.Object
    {
        var ressources = Resources.LoadAll<T>(/*"Assets/Resources/"+ */path);
        return ressources;
    }
}