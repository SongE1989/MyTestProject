using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameXiuXian : MonoBehaviour
{
    public static GameXiuXian Instance;

    public const string Layer_Selectable = "Selectable";

    public Transform m_kCameraLookAt;
    public XXPlayer m_kPlayer;
    public float MapWidth = 100;
    public float MapHeight = 100;
    public int DoorNum = 1;
    public int PlantNum = 1;
    public int MonsterNestNum = 3;

    public Transform EnvirLayer;
    public Transform MoveLayer;

    public List<XXHerb> m_kHerbList;
    public List<XXMonsterNest> m_kMonsterNestList;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        for (int i_item = 0, n_item = DoorNum; i_item < n_item; i_item++)
        {
            var comp = EntryManager.Instance.CreateActionEntry<XXSchool>("Door");
            comp.transform.position = new Vector3(Random.Range(-MapWidth, MapWidth), 0, Random.Range(-MapHeight, MapHeight));
        }

        m_kHerbList = new List<XXHerb>();
        for (int i_item = 0, n_item = PlantNum; i_item < n_item; i_item++)
        {
            var comp = EntryManager.Instance.CreateActionEntry<XXHerb>("Grass");
            comp.transform.position = new Vector3(Random.Range(-MapWidth, MapWidth), 0, Random.Range(-MapHeight, MapHeight));
            m_kHerbList.Add(comp);
        }

        m_kMonsterNestList = new List<XXMonsterNest>();
        for (int i_item = 0, n_item = MonsterNestNum; i_item < n_item; i_item++)
        {
            var comp = EntryManager.Instance.CreateActionEntry<XXMonsterNest>("MonsterNest");
            comp.transform.position = new Vector3(Random.Range(-MapWidth, MapWidth), 0, Random.Range(-MapHeight, MapHeight));
            m_kMonsterNestList.Add(comp);
        }
        m_kPlayer.gameObject.SetActive(true);
    }

    [ContextMenu("testAddStudent")]
    void testAddStudent()
    {
        EntryManager.Instance.CreateActionEntry<XXStudent>("Student");
    }

    void Update()
    {
        m_kPlayer.Control();
        m_kCameraLookAt.transform.position = m_kPlayer.transform.position;
    }
}
