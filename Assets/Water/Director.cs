using UnityEngine;
using System.Collections;

public enum EnumDirStr
{
    无 = 0,
    北 = 0,
    东北 = 45,
    东 = 90,
    东南 = 135,
    南 = 180,
    西南 = 225,
    西 = 270,
    西北 = 315
}

public class Director : MonoBehaviour
{
    public const float Degree6 = 45f / 6f;

    public EnumDirStr 朝向;
    /// <summary>左减右加</summary>
    [Range(-6,6)]
    public float 偏向度数;

    public float Dir;
    public float Distance;
    float HorizontalDistance;
    public float Depth;
    public GameObject Home;
    
    //360 45/6

    [ContextMenu("SetPos")]
    void setPos()
    {
        Dir = (int)朝向 + 偏向度数 * Degree6;
        Debug.Log("Dir:"+Dir);
        HorizontalDistance = Mathf.Sqrt(Distance * Distance - Depth * Depth);
        float px = HorizontalDistance * Mathf.Sin(Dir);
        float pz = HorizontalDistance * Mathf.Cos(Dir);
        transform.position = new Vector3(px, -Depth, pz);
    }
}
