using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XXMonsterNest : XXEntry
{
    public float m_kMonsterRegerateTime = 5f;
    public float m_kMaxMonsterNum = 3f;
    public float m_kMonsterRegerateTimer = 0f;
    public List<XXMonster> m_kMonsterList = new List<XXMonster>();

    protected override void Update()
    {
        if (m_kMonsterList.Count < m_kMaxMonsterNum)
        {
            m_kMonsterRegerateTimer += Time.deltaTime;
            if (m_kMonsterRegerateTimer > m_kMonsterRegerateTime)
            {
                m_kMonsterRegerateTimer = 0;
                regerateMonster();
            }
        }
    }

    void regerateMonster()
    {
        var monsterComp = EntryManager.Instance.CreateActionEntry<XXMonster>("Monster");
        monsterComp.m_kNest = this;
        m_kMonsterList.Add(monsterComp);
        monsterComp.transform.position = transform.position;
    }
}
