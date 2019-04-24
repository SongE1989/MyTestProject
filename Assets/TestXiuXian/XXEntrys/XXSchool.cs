using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XXSchool : XXEntry
{
    public XXMaster m_kMaster;
    public List<XXStudent> m_kStudentList = new List<XXStudent>();

    public float m_kMasterRegerateTime = 3f;
    XXUtil.Timer m_kMasterRegenTimer;

    public float m_kStudentRegerateTime = 0.1f;
    XXUtil.Timer m_kStudentRegenTimer;
    public int m_kMaxApprenticeNum = 5;

    protected override void Start()
    {
        base.Start();
        m_kMasterRegenTimer = new XXUtil.Timer(m_kMasterRegerateTime, regenerateMaster);
        m_kStudentRegenTimer = new XXUtil.Timer(m_kStudentRegerateTime, regenerateApprentice);
    }

    protected override void Update()
    {
        base.Update();
        if (m_kMaster == null)
            m_kMasterRegenTimer.Tick();
        else if(m_kStudentList.Count < m_kMaxApprenticeNum)
            m_kStudentRegenTimer.Tick();
    }

    void regenerateMaster()
    {
        m_kMaster = EntryManager.Instance.CreateActionEntry<XXMaster>("Master");
        m_kMaster.m_kSchool = this;
        m_kMaster.transform.position = transform.position;
    }

    void regenerateApprentice()
    {
        if(m_kMaster != null)
        {
            var comp = EntryManager.Instance.CreateActionEntry<XXStudent>("Student");
            m_kStudentList.Add(comp);
            comp.m_kSchool = this;
            comp.m_kMaster = m_kMaster;
            comp.transform.position = transform.position;
        }
    }

    public List<XXStudent> SummondStudent(XXMaster master)
    {
        List<XXStudent> incallList = new List<XXStudent>();
        int studentStillNeed = master.StudentCapacity;
        for (int i = 0,length = m_kStudentList.Count; i < length; i++)
        {
            var stu = m_kStudentList[i];
            if(stu.IsFree)
            {
                stu.IsFree = false;//TODO 记得返回时释放
                incallList.Add(stu);
                studentStillNeed--;
                if (studentStillNeed == 0)
                    break;
            }
        }
        return incallList;
    }
}
