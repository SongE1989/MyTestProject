using UnityEngine;
using System.Collections;

public class Trader180415 : MonoBehaviour
{
    public Town180415 CurPos;
    public Town180415 TarPos;


    // Use this for initialization
    void Start()
    {

    }



    // Update is called once per frame
    void Update()
    {
        if(TarPos == null)
        {
            TarPos = getNextTarget(TradeWorld.Instance.towns);
        }
        Vector3 nextPos = Vector3.zero;
        SongeUtil.MoveToPoint(transform.position, TarPos.transform.position, 0.02f, ref nextPos);
        transform.position = nextPos;
        if(nextPos == TarPos.transform.position)
        {
            TarPos = getNextTarget(TradeWorld.Instance.towns);
        }
    }

    Town180415 getNextTarget(Town180415[] townArr)
    {
        if (townArr.Length == 1)
            return townArr[0];
         var res = UnityEngine.Random.Range(0, townArr.Length - 1);

        if (townArr[res] != CurPos)
            return townArr[res];
        else
            return getNextTarget(townArr);
    }
}
