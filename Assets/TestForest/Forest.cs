using UnityEngine;
using System.Collections;

public class Forest : MonoBehaviour
{
    #region Prefabs
    GameObject grassPrefab;
    GameObject brushPrefab;
    GameObject treePrefab;

    #endregion


    void Start()
    {
        grassPrefab = Resources.Load<GameObject>("ForestPrefabs/Grass");
        brushPrefab = Resources.Load<GameObject>("ForestPrefabs/Brush");
        treePrefab = Resources.Load<GameObject>("ForestPrefabs/Tree");
    }

    void Update()
    {

    }
}
