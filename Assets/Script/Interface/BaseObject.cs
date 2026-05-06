//using UnityEngine;

//public class BaseObject : MonoBehaviour, IObject
//{
//    [SerializeField] protected string _id;
//    public string Id => _id;

//    protected ObjectData _data = new ObjectData();

//    public virtual void Init() { }

//    public virtual ObjectData GetData()
//    {
//        _data.id = _id;

//        _data.gridPos = GridSystem.Instance.WorldToCell(transform.position);

//        _data.height = transform.position.y;

//        _data.rotation = transform.eulerAngles;

//        return _data;
//    }

//    public virtual void LoadData(ObjectData data)
//    {
//        _data = data;

//        Vector3 worldPos = GridSystem.Instance.CellToWorld(data.gridPos);

//        worldPos.y = data.height;

//        transform.position = worldPos;

//        transform.eulerAngles = data.rotation;
//    }
//}
