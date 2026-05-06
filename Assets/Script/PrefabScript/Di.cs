using UnityEngine;

public class Di : MonoBehaviour, IObject
{
    [SerializeField] private string _id;
    public string Id => _id;

    [SerializeField] private float _height = 2.5f;
    [SerializeField] private Vector3 _defaultRotation = new Vector3(-90,0,0);

    private ObjectData _data = new ObjectData();

    public void Init() { }

    public float GetHeight()
    {
        return _height;
    }

    public ObjectData GetData()
    {
        _data.id = _id;
        _data.gridPos = GridSystem.Instance.WorldToCell(transform.position);
        return _data;
    }

    public void LoadData(ObjectData data)
    {
        _data = data;

        Vector3 worldPos = GridSystem.Instance.CellToWorld(data.gridPos);


        transform.position = new Vector3(worldPos.x, _height, worldPos.z);
        transform.eulerAngles = _defaultRotation;
    }
}
