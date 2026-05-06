using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabRegistry", menuName = "GameData/PrefabRegistry")]
public class PrefabRegistry:ScriptableObject
{
    [System.Serializable]
    public class PrefabEntry
    {
        public string name;
        public GameObject prefab;
    }

    public List<PrefabEntry> prefabs;

    public GameObject GetPrefabByName(string name)
    {
        var entry = prefabs.Find(e => e.name == name);
        if (entry != null)
        {
            return entry.prefab;
        }
        return null;
    }
    public List<string> GetAllPrefabNames()
    {
        List<string> names = new List<string>();
        foreach (var entry in prefabs)
        {
            names.Add(entry.name);
        }
        return names;
    }
}
