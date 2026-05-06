using NaughtyAttributes;
using System.Collections.Generic;
using System.Net.WebSockets;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;


public class LevelEditor : MonoBehaviour
{
    public bool isEditing = false;

    [Header("References")]
    public PrefabRegistry registry;


    [Header("Grid Settings")]
    public int gridWidth = 20;
    public int gridHeight = 20;
    public float cellSize = 1f;
    public Color gridColor = Color.green;

    [Header("Editor Settings")]
    public string currentPrefabId;
    public LayerMask groundLayer;
    [SerializeField]private GameObject mapContainer; 

    [Header("Level Creation")]
    public string newLevelName = "NewLevel"; // Tên file level mới

    private float currentRotationY = 0f;
    public Plane groundPlane = new Plane(Vector3.up, Vector3.zero);


    private Dictionary<Vector2Int, GameObject> _spawnedObjects = new Dictionary<Vector2Int, GameObject>();

    public void Editing()
    {
        isEditing=!isEditing;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gridColor;
        Vector3 origin = transform.position + new Vector3(0, -0.01f, 0);

        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = origin + new Vector3(x * cellSize, 0, 0);
            Vector3 end = origin + new Vector3(x * cellSize, 0, gridHeight * cellSize);
            Gizmos.DrawLine(start, end);
        }

        for (int z = 0; z <= gridHeight; z++)
        {
            Vector3 start = origin + new Vector3(0, 0, z * cellSize);
            Vector3 end = origin + new Vector3(gridWidth * cellSize, 0, z * cellSize);
            Gizmos.DrawLine(start, end);
        }
    }



    public void PlaceObject(Vector3 point)
    {
        
            Vector2Int cell = GridSystem.Instance.WorldToCell(point);
            if (point == null)
            {
            Debug.LogError("point:"+point);
            return;
             }
            if (_spawnedObjects.ContainsKey(cell)) 
            {
                Debug.Log("Already got prefab");
                return;
            }
            ;
            Vector3 spawnPosition = GridSystem.Instance.CellToWorld(cell);

            GameObject prefab = registry.GetPrefabByName(currentPrefabId);
            if (prefab != null)
            {

                GameObject obj = Instantiate(prefab, spawnPosition, Quaternion.identity);
                obj.transform.SetParent(mapContainer.transform);

                var iObj = obj.GetComponent<IObject>();

                float height = 0f;

                if (iObj != null)
                {
                    height = iObj.GetHeight();
                    Debug.Log($"Height: {height}");
                }

                Vector3 finalPos = new Vector3(spawnPosition.x, height, spawnPosition.z);

                obj.transform.position = finalPos;

                Vector3 baseRot = prefab.transform.eulerAngles;

                Quaternion finalRot = Quaternion.Euler(
                    baseRot.x,
                    baseRot.y + currentRotationY,
                    baseRot.z
                );

                obj.transform.rotation = finalRot;
                _spawnedObjects[cell] = obj;
            }
        
    }
    public void RemoveObject(GameObject clickedObject)
    {
        var iObject = clickedObject.GetComponentInParent<IObject>();

        if (iObject != null)
        {
            GameObject root = iObject.gameObject;

            Vector2Int cell = GridSystem.Instance.WorldToCell(root.transform.position);

            if (_spawnedObjects.ContainsKey(cell))
            {
                _spawnedObjects.Remove(cell);
            }

            DestroyImmediate(root);
        }
        else
        {
            Debug.LogWarning("Đối tượng này không có IObject, không thể xóa qua hệ thống Editor!");
        }
    }
    public void SaveLevel()
    {
        string path = $"Assets/Resources/Data/{newLevelName}.asset";

        LevelData levelAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<LevelData>(path);
        bool isNew = false;

        if (levelAsset == null)
        {
            levelAsset = ScriptableObject.CreateInstance<LevelData>();
            isNew = true;
        }

        levelAsset.levelName = newLevelName;
        levelAsset.objects = new List<ObjectData>();

        foreach (Transform child in mapContainer.transform)
        {
            var script = child.GetComponent<IObject>();
            if (script != null)
            {
                levelAsset.objects.Add(script.GetData());
            }
        }

        if (isNew)
        {
            UnityEditor.AssetDatabase.CreateAsset(levelAsset, path);
        }
        else
        {
            UnityEditor.EditorUtility.SetDirty(levelAsset);
        }

  
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log($"<color=green>Đã lưu Level: {newLevelName} tại {path}</color>");
    }
    public void RotateObject(GameObject obj)
    {
        var iObject = obj.GetComponentInParent<IObject>();

        if (iObject != null)
        {
            GameObject root = iObject.gameObject;

            root.transform.Rotate(Vector3.up, 90f);
        }
        else
        {
           Debug.LogWarning("Đối tượng này không có IObject, không thể xoay qua hệ thống Editor!");
        }

    }
    public void ClearObjects()
    {
        for (int i = mapContainer.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(mapContainer.transform.GetChild(i).gameObject);
        }

        _spawnedObjects.Clear();
    }
    public void LoadLevel(LevelData data)
    {
        if (data == null) return;

        ClearObjects();
        newLevelName = data.levelName;

        foreach (var objData in data.objects)
        {
            GameObject prefab = registry.GetPrefabByName(objData.id);

            if (prefab != null)
            {


                GameObject spawnedObj = Instantiate(prefab, mapContainer.transform);

                var iObject = spawnedObj.GetComponent<IObject>();
                if (iObject != null)
                {
                    iObject.LoadData(objData);
                    Vector2Int cell = objData.gridPos;
                    _spawnedObjects[cell] = spawnedObj;
                }                
            }
            else
            {
                Debug.LogWarning($"Không tìm thấy Prefab cho ID: {objData.id}");
            }
        }
    }
}
