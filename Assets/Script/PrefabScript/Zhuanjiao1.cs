using UnityEngine;

public class Zhuanjiao1 : Lumian
{

    public override ObjectData GetData()
    {
        base.GetData();

        _data.rotation = transform.eulerAngles;

        return _data;
    }

    public override void LoadData(ObjectData data)
    {
        base.LoadData(data);

        transform.eulerAngles = data.rotation;
    }
}