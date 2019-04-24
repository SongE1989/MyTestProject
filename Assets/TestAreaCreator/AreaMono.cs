using UnityEngine;
using System.Collections;

public class AreaMono : MonoBehaviour {

    public string AreaName;

    GameObject land;
    GameObject water;
    Material waterMat;
    Material landMat;

    #region 水深度(相对于地面)

    public int WaterNum = 0;
    public void refreshWaterView()
    {
        if (WaterNum > 0)
        {
            water.SetActive(true);
            water.transform.localPosition = new Vector3(0, WaterNum/10f, 0);
            waterMat.color = new Color(0, 138f/256f, 1, 0.5f);
        }
        else
            water.SetActive(false);
    }

    /// <summary>水平面高度</summary>
    public int WaterHeight
    {
        get { return AreaHeight + WaterNum; }
    }

    #endregion

    #region 地面高度
    public int AreaHeight;
    public void refreshAreaHeightView()
    {
        transform.localPosition = new Vector3(transform.position.x, AreaHeight/10f, transform.position.z);
    }
    #endregion

    #region 地块位置
    public Vector2 Pos;

    public void RefreshPosView()
    {
        AreaName = ((int)Pos.x).ToString("D3") + "_" + ((int)Pos.y).ToString("D3");
        gameObject.name = AreaName;
        transform.localPosition = new Vector3(Pos.x, transform.localPosition.y, Pos.y);
    }
    #endregion

    #region 启动与运行

    public void Init()
    {
        land = GameObject.CreatePrimitive(PrimitiveType.Quad);
        land.transform.localEulerAngles = new Vector3(90f, 0, 0);
        land.transform.parent = transform;

        water = GameObject.CreatePrimitive(PrimitiveType.Quad);
        water.transform.localEulerAngles = new Vector3(90f, 0, 0);
        water.transform.parent = transform;

        landMat = land.GetComponent<Renderer>().sharedMaterial;// = Resources.Load("AreaMaerial") as Material;
        landMat.color = Color.white;
        waterMat = water.GetComponent<Renderer>().sharedMaterial = Resources.Load("AreaMaerial") as Material;
        waterMat.color = Color.white;

        refreshAreaHeightView();
        RefreshPosView();
        refreshWaterView();
    }
    #endregion

}
