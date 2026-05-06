using UnityEngine;


public interface IObject
{

    string Id { get; }  
    GameObject gameObject { get; }

    void Init();                   

    ObjectData GetData();          
    void LoadData(ObjectData data);

    float GetHeight();

}



[System.Serializable]
public class ObjectData
{
    public string id;
    public Vector2Int gridPos;
    public Vector3 rotation = Vector3.zero; 
}

