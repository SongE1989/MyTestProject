using UnityEngine;
using System.Collections;

public class SunLightMove : MonoBehaviour
{
    public Vector3 RotateSpeed = new Vector3();

    void Start()
    {

    }

    void Update()
    {
        //transform.localRotation += RotateSpeed;
        transform.Rotate(RotateSpeed.x, RotateSpeed.y, RotateSpeed.z);
    }
}
