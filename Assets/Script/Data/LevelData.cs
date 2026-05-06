using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "GameData/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public List<ObjectData> objects = new List<ObjectData>();
}