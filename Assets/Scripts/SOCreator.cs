using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SOCreator : MonoBehaviour
{
    [SerializeField] TextAsset _csv;

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
        var guid = AssetDatabase.FindAssets($"t:{typeof(T).Name} {name.Trim()}").First();
        var path = AssetDatabase.GUIDToAssetPath(guid);
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }
}
