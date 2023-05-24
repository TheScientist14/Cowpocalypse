using UnityEngine;

[CreateAssetMenu(menuName = "Tier")]
public class ItemTier : ScriptableObject
{
    [field: SerializeField] public int Level { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public int UnlockPrice { get; private set; }
}