using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                // Thay vì FindObjectOfType, dùng FindFirstObjectByType (nhanh và đúng chuẩn mới)
                _instance = Object.FindFirstObjectByType<GridSystem>();
            }
            return _instance;
        }
        private set { _instance = value; }
    }
    private static GridSystem _instance;

    [Header("Cấu hình lưới")]
    public int width ;       // Số ô theo trục X
    public int height ;      // Số ô theo trục Z
    public float cellSize ;  // Kích thước mỗi ô


   
    public Vector3 CellToWorld(Vector2Int cell)
    {
        return transform.position +
               new Vector3(
                   cell.x * cellSize + cellSize * 0.5f,
                   0,
                   cell.y * cellSize + cellSize * 0.5f
               );
    }


    public Vector2Int WorldToCell(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - transform.position.x) / cellSize);
        int z = Mathf.FloorToInt((worldPosition.z - transform.position.z) / cellSize);

        return new Vector2Int(x, z);
    }


}
