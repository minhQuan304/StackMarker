using UnityEngine;

public class Qiao : MonoBehaviour, IObject
{
    [SerializeField] private string _id;
    public string Id => _id;

    [SerializeField] private float _height = 5f;


    protected ObjectData _data = new ObjectData();

    [SerializeField] private Material _material;

    [SerializeField] private GameObject dimian;

    public void Init() { }
    public float GetHeight()
    {
        return _height;
    }

    public ObjectData GetData()
    {
        _data.id = _id;
        _data.gridPos = GridSystem.Instance.WorldToCell(transform.position);
        _data.rotation = transform.eulerAngles;
        return _data;
    }

    public void LoadData(ObjectData data)
    {
        _data = data;

        Vector3 worldPos = GridSystem.Instance.CellToWorld(data.gridPos);


        transform.position = new Vector3(worldPos.x, _height, worldPos.z);
        transform.eulerAngles = data.rotation;
    }
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            dimian.GetComponent<MeshRenderer>().material = _material;
            GetComponent<Collider>().enabled = false;
            EventManager.Raise(new QiaoPlayerCollisionEvent { Qiao = this, Collision = other });
        }
    }
}
