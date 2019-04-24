using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {
    public uint width = 3;
    public uint height = 3;
    [ContextMenu("t1")]
    public void t1()
    {
        for (uint i = 0; i < 50; i++)
        {
            uint px = i % height;
            uint py = i / height;
            Debug.Log(px+","+ py);
        }
    }
}