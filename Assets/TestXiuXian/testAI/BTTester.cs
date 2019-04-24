using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTTester : MonoBehaviour {

    public string MessageToSend;
    [ContextMenu("SendMessage")]
    void SendMessage()
    {
        GetComponent<BehaviorTree>().SendEvent(MessageToSend);
    }
    public Transform xxTran;
    [ContextMenu("SendMessage2")]
    void SendMessage2()
    {
        GetComponent<BehaviorTree>().SendEvent(MessageToSend, MessageToSend as object);
    }
    public string OrderName;
    [ContextMenu("GiveOrder")]
    void GiveOrder()
    {
        GetComponent<BehaviorTree>().SendEvent("GiveOrder", OrderName as object);
    }

    [ContextMenu("testObjList")]
    void testObjList()
    {
        HashSet<XXEntry> testList = new HashSet<XXEntry>();
        GetComponent<BehaviorTree>().SetVariableValue("enemyList", testList as object);
    }
}
