using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;

public class SpawnLevel : MonoBehaviour
{
    [Header("Data References")]
    private LevelData levelData;       // Kéo file LevelData vào đây
    public PrefabRegistry registry;  // Kéo file PrefabRegistry vào đây
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineCamera cinemachineCamera;

    //[Header("Grid Settings")]
    //public int gridWidth ;
    //public int gridHeight ;
    //public float cellSize ;
    //public Material groundMaterial;

    //private GameObject ground;

    [Button("Start Level Spawn")]
    private void StartLevelSpawnButton()
    {
        StartLevelSpawn(4); 
    }
    public void StartLevelSpawn(int levelIndex)
    {
        LevelData data = Resources.Load<LevelData>($"Data/Level{levelIndex}");

        if (data == null)
        {
            Debug.LogError($"Không tìm thấy LevelData tại: Resources/Data/Level{levelIndex}");
            return;
        }

        SpawnMap(data);
    }
    //public void CreateGround()
    //{
    //    if (ground != null)
    //    {
    //        DestroyImmediate(ground);
    //    }

    //    ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
    //    ground.name = "EditorGround";
    //    ground.transform.parent = this.transform;

    //    float scaleX = (gridWidth * cellSize) / 10f;
    //    float scaleZ = (gridHeight * cellSize) / 10f;

    //    ground.transform.localScale = new Vector3(scaleX, 1, scaleZ);

    //    float sizeX = gridWidth * cellSize;
    //    float sizeZ = gridHeight * cellSize;

    //    ground.transform.position = new Vector3(sizeX / 2f, 0, sizeZ / 2f);

    //    ground.layer = LayerMask.NameToLayer("Ground");
    //    Renderer renderer = ground.GetComponent<Renderer>();
    //    if (renderer != null && groundMaterial != null)
    //    {
    //        renderer.sharedMaterial = groundMaterial;
    //    }
    //}
    public void CreateGround()
    {
        GameObject ground = GameObject.Find("InfiniteWaterBackground");

        if (ground == null)
        {
            ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "InfiniteWaterBackground";

            bool hasCollider = ground.GetComponent<MeshCollider>() != null;
            MeshCollider col;

            if (hasCollider)
            {
                col = ground.GetComponent<MeshCollider>();
                Destroy(col);
            }

            Renderer rend = ground.GetComponent<Renderer>();
            rend.sharedMaterial = Resources.Load<Material>("Material/Water_Mat");

            // 5. Thêm hiệu ứng cuộn nước
            //ground.AddComponent<TextureScroller>();
        }


        ground.transform.localScale = new Vector3(1000f, 1f, 1000f);
        ground.transform.position = new Vector3(0, -0.1f, 0);
    }

    public void SpawnMap(LevelData data)
    {
        if (data == null || registry == null)
        {
            Debug.LogError("Thiếu LevelData hoặc PrefabRegistry!");
            return;
        }


        foreach (var objData in data.objects)
        {
            GameObject prefab = registry.GetPrefabByName(objData.id);

            if (prefab != null)
            {
 

                GameObject spawnedObj = Instantiate(prefab, transform);

                var iObject = spawnedObj.GetComponent<IObject>();
                if (iObject != null)
                {
                    iObject.LoadData(objData);
                }
                if (objData.id == "Di" && playerPrefab != null)
                {
                    // Tính toán vị trí: cùng vị trí của Di, nhưng cao hơn 3 đơn vị
                    Vector3 spawnPos = spawnedObj.transform.position + Vector3.up * 3f;

                    // Khởi tạo Player
                    GameObject player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
                    if (cinemachineCamera != null)
                    {
                        cinemachineCamera.Target.TrackingTarget = player.transform;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Không tìm thấy Prefab cho ID: {objData.id}");
            }
        }
    }
    [Button("Clear Map")]
    public void ClearMap()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }

        GameObject existingPlayer = GameObject.FindWithTag("Player");
        if (existingPlayer != null)
        {
            if (Application.isPlaying) Destroy(existingPlayer);
            else DestroyImmediate(existingPlayer);
        }

      
    }
}