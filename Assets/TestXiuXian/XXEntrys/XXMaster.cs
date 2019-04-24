using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO Func SummondStudent(取得空闲对象,记录, 并SendEvent("Follow"))
//TODO Event AllStudentArrive(Stu执行Follow,并在到达时对其SendEvent("Arrive")
//TODO Func CampUp(播放动作)
//TODO Check WeightFull(行为树值检查)
//TODO Func RecallStudent(SendEvent("Follow"))
//TODO Event AllStudentArrive(同上)
public class XXMaster : XXAIEntry
{
    public XXSchool m_kSchool;
    public List<XXStudent> mSummondStudentList;
    public int StudentCapacity = 3;
    public const string Share_School = "School";
    public const string Share_StudentList = "StudentList";
    public const string Action_GiveOrder = "GiveOrder";
    public const string Action_SummondStudent = "SummondStudent";
    public const string Action_DismissStudent = "DismissStudent";
    public const string Action_StudentGoGather = "StudentGoGather";
    public const string Action_CampUp = "CampUp";
    public const string Action_GoHome = "GoHome";

    protected override void Start()
    {
        base.Start();
        CurWeight = 0f;
        TotalWeight = 10f;
        GetBehaviorTree().SetVariableValue(Share_School, m_kSchool.transform);
        GetBehaviorTree().Start();
    }

    public override XXAction DoXXAction(string actionName, XXEntry tar = null, Vector3 deltaPos = default(Vector3))
    {
        if (CurAction != null)
            CurAction.CancelAction();
        switch (actionName)
        {
            case "TargetSchoolPos":
                mMoveTargetPos = m_kSchool.transform.position;
                return null;
            case Action_SummondStudent:
                if (m_kSchool == null)
                    return null;

                if (mMessageSystem == null)
                    new MessageSystem().AddMemeber(this);


                mSummondStudentList = m_kSchool.SummondStudent(this);
                List<Transform> tranList = new List<Transform>();
                for (int i = 0, length = mSummondStudentList.Count; i < length; i++)
                {
                    var stu = mSummondStudentList[i];
                    mMessageSystem.AddMemeber(stu);
                    stu.GetBehaviorTree().SendEvent(Action_GiveOrder, Action_FollowTarget as object, this as object);
                    tranList.Add(stu.transform);
                }
                GetBehaviorTree().SetVariableValue(Share_StudentList, tranList as object);
                return null;
            case Action_DismissStudent:
                for (int i = 0, length = mSummondStudentList.Count; i < length; i++)
                {
                    var stu = mSummondStudentList[i];
                    mMessageSystem.RemoveMemeber(stu);
                    stu.GetBehaviorTree().SendEvent(Action_GiveOrder, Action_DismissStudent as object);
                    stu.IsFree = true;
                }
                CurWeight = 0;//NOTE we clear weight here
                return null;
            case Action_StudentGoGather:
                for (int i = 0, length = mSummondStudentList.Count; i < length; i++)
                {
                    var stu = mSummondStudentList[i];
                    stu.GetBehaviorTree().SendEvent(Action_GiveOrder, Action_StudentGoGather as object);
                    stu.IsFree = true;
                }
                return null;
            case Action_GoHome:

                return null;
            default:
                return base.DoXXAction(actionName, tar, deltaPos);
        }
    }

}
