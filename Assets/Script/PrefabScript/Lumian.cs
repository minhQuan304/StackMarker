using UnityEngine;

public class Lumian : MonoBehaviour, IObject
{
    [SerializeField] protected string _id;
    public string Id => _id;

    protected float _height = 2.5f;
    protected Vector3 _defaultRotation = Vector3.zero;

    protected ObjectData _data = new ObjectData();

    public virtual void Init() { }
    public float GetHeight()
    {
        return _height;
    }

    public virtual ObjectData GetData()
    {
        _data.id = _id;
        _data.gridPos = GridSystem.Instance.WorldToCell(transform.position);
        return _data;
    }

    public virtual void LoadData(ObjectData data)
    {
        _data = data;

        Vector3 worldPos = GridSystem.Instance.CellToWorld(data.gridPos);


        transform.position = new Vector3(worldPos.x, _height, worldPos.z);
        transform.eulerAngles = _defaultRotation;
    }
}
