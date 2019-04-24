using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class AreaCreator : MonoBehaviour {

    public Dictionary<Vector2, AreaMono> areaDic;
    int lx;
    int lz;
    int minY = -20;
    int maxY = 20;
    GameObject cubeFolder;

    void Awake ()
    {
        areaDic = new Dictionary<Vector2, AreaMono>();
        lx = lz = 32;

        cubeFolder = new GameObject();
        //cubeFolder = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubeFolder.name = "cubeFolder";
        
        for (int i = 0; i < lx * lz; i++)
        {
            int ix = i % lx;
            int iz = i / lx;
            GameObject go = new GameObject();
            AreaMono area = go.AddComponent<AreaMono>();
            area.transform.parent = cubeFolder.transform;
            area.Pos = new Vector2(ix, iz);
            areaDic.Add(new Vector2(ix, iz), area);
            area.Init();
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            randomHeight((int)(lx * lz / 128f / 128f * 400f));
            addRain(1, 1);
            addRiver(6);
            setSeaLevel(0.1f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            randomHeight(400);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            addRain(1, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            addRiver(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            foreach (var area in areaDic.Values)
            {
                area.WaterNum = 0;
                area.refreshWaterView();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            windEffect(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            setSeaLevel(0.1f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            combineMesh1();
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            combineMesh();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            combineMesh2();
        }
    }

    void combineMesh2()
    {
        AreaMono[] areaArr = new AreaMono[areaDic.Values.Count];
        areaDic.Values.CopyTo(areaArr, 0);
        List<MeshFilter> mfList = new List<MeshFilter>();
        for (int i = 0, length = areaArr.Length; i < length; i++)
        {
            mfList.AddRange(areaArr[i].GetComponentsInChildren<MeshFilter>());
            //mrList.AddRange(areaArr[i].GetComponentsInChildren<MeshRenderer>());
        }
        for (int i = 0, length = mfList.Count; i < length-1; i++)
        {
            mergerCombine(mfList[i].gameObject, mfList[i + 1].gameObject);
        }
    }

    void combineMesh1()
    {
        int step = 300;//步进值
        AreaMono[] areaArr = new AreaMono[areaDic.Values.Count];
        areaDic.Values.CopyTo(areaArr, 0);

        List<MeshFilter> mfList = new List<MeshFilter>();
        List<MeshRenderer> mrList = new List<MeshRenderer>();
        for (int i = 0, length = areaArr.Length; i < length; i++)
        {
            mfList.AddRange(areaArr[i].GetComponentsInChildren<MeshFilter>());
            mrList.AddRange(areaArr[i].GetComponentsInChildren<MeshRenderer>());
        }

        MeshFilter[] mfArr = cubeFolder.GetComponentsInChildren<MeshFilter>();//mfList.ToArray();
        Material[] mats = new Material[mrList.Count];
        for (int i = 0; i < mrList.Count; i++)
            mats[i] = mrList[i].sharedMaterial;
        Debug.Log(mfArr.Length + "~" + mats.Length);


        //为新的整体新建一个mesh 
        if (cubeFolder.GetComponent<MeshFilter>() == null)
            cubeFolder.AddComponent<MeshFilter>();
        MeshFilter totalMF = cubeFolder.GetComponent<MeshFilter>();

        int lastMesh = mfArr.Length;
        while(lastMesh > 0)
        {
            int curLen = lastMesh > step ? step : lastMesh;
            int curIndex = mfArr.Length - lastMesh;
            CombineInstance[] combine = new CombineInstance[curLen + 1];
            for (int i = 0; i < curLen; i++)
            {
                MeshFilter mf = mfArr[curIndex].GetComponent<MeshFilter>();
                combine[i].mesh = mf.sharedMesh;
                combine[i].transform = mf.transform.localToWorldMatrix;
            }
            Debug.Log(curLen +"~"+ curIndex+"~"+ combine.Length);
            Debug.Log("[" + cubeFolder.GetComponent<MeshFilter>().mesh.triangles.Length);
            combine[curLen].mesh = cubeFolder.GetComponent<MeshFilter>().sharedMesh;
            combine[curLen].transform = cubeFolder.GetComponent<MeshFilter>().transform.localToWorldMatrix;

            int c = 0;
            for (int i = 0,length = combine.Length; i < length; i++)
            {
                c += combine[i].mesh.triangles.Length;
            }
            Debug.Log("c: "+c);

            cubeFolder.GetComponent<MeshFilter>().mesh = new Mesh();
            cubeFolder.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, false);
            Debug.Log(totalMF.mesh.triangles.Length+"]");
            lastMesh -= curLen;
            //Debug.Log(curLen + "~" + curIndex + "~" + lastMesh);
        }

        //为合并后的新Mesh指定材质 ------------------------------  
        if (cubeFolder.GetComponent<MeshRenderer>() == null)
            cubeFolder.AddComponent<MeshRenderer>();
        cubeFolder.GetComponent<MeshRenderer>().sharedMaterials = mats;
        for (int i = 0, length = cubeFolder.transform.childCount; i < length; i++)
        {
            Destroy(cubeFolder.transform.GetChild(i).gameObject);
        }
    }

    void mergerCombine(GameObject ori, GameObject tar)
    {
        MeshFilter[] meshFilters1 = ori.GetComponents<MeshFilter>();
        MeshFilter[] meshFilters2 = tar.GetComponents<MeshFilter>();

        MeshFilter[] meshFilters = new MeshFilter[meshFilters1.Length + meshFilters2.Length];
        meshFilters1.CopyTo(meshFilters, 0);
        meshFilters2.CopyTo(meshFilters, meshFilters1.Length);
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }
        ori.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        ori.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
    }

    void combineMesh()
    {
        MeshRenderer[] meshRenderers = cubeFolder.GetComponentsInChildren<MeshRenderer>();
        Material[] mats = new Material[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++)
            mats[i] = meshRenderers[i].sharedMaterial;
        //List<Material> matList = new List<Material>();
        //for (int i = 0; i < meshRenderers.Length; i++)
        //{
        //    if (!matList.Contains(meshRenderers[i].sharedMaterial))
        //        matList.Add(meshRenderers[i].sharedMaterial);
        //}
        //Material[] mats = matList.ToArray();


        MeshFilter[] meshFilters = cubeFolder.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            //矩阵(Matrix)自身空间坐标的点转换成世界空间坐标的点   
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            //Debug.Log("combine[i].subMeshIndex: " + combine[i].subMeshIndex);
            //meshFilters[i].gameObject.SetActive(false);
            meshFilters[i].gameObject.GetComponent<Renderer>().enabled = false;
        }


        //为新的整体新建一个mesh 
        if (cubeFolder.GetComponent<MeshFilter>() == null)
            cubeFolder.AddComponent<MeshFilter>();
        cubeFolder.GetComponent<MeshFilter>().mesh = new Mesh();
        //合并Mesh. 第二个false参数, 表示并不合并为一个网格, 而是一个子网格列表  
        List<CombineInstance[]> combineList = new List<CombineInstance[]>();
        int meshLast = combine.Length;
        while (meshLast > 0)
        {
            int curMesh = combine.Length - meshLast;
            int curLen = meshLast > 300 ? 300 : meshLast;
            CombineInstance[] comb = new CombineInstance[curLen];
            for (int i = curMesh, length = curMesh + curLen; i < length; i++)
            {
                int curIndex = i - curMesh;
                comb[curIndex] = combine[i];
            }
            combineList.Add(comb);
            meshLast -= meshLast > 300 ? 300 : meshLast;
        }
        Mesh mesh = cubeFolder.GetComponent<MeshFilter>().mesh;



        mesh.CombineMeshes(combine, false);
        if (cubeFolder.GetComponent<MeshRenderer>() == null)
            cubeFolder.AddComponent<MeshRenderer>();
        cubeFolder.GetComponent<MeshRenderer>().sharedMaterials = mats;

        //for (int i = 0, length = combineList.Count; i < length; i++)
        //{
        //    mesh.CombineMeshes(combineList[i], false);
        //    //为合并后的新Mesh指定材质 ------------------------------  
        //    if (cubeFolder.GetComponent<MeshRenderer>() == null)
        //        cubeFolder.AddComponent<MeshRenderer>();
        //    cubeFolder.GetComponent<MeshRenderer>().sharedMaterials = mats;
        //}

        cubeFolder.SetActive(true);

        for (int i = 0, length = cubeFolder.transform.childCount; i < length; i++)
        {
            Destroy(cubeFolder.transform.GetChild(i).gameObject);
        }
    }

    IEnumerator combineEnum(List<CombineInstance[]> combineList, Mesh mesh, Material[] mats)
    {
        for (int i = 0, length = combineList.Count; i < length; i++)
        {
            Debug.Log("<----"+mesh.triangles.Length);
            mesh.CombineMeshes(combineList[i], false);
            Debug.Log("combine!");
            //为合并后的新Mesh指定材质 ------------------------------  
            if (cubeFolder.GetComponent<MeshRenderer>() == null)
                cubeFolder.AddComponent<MeshRenderer>();
            cubeFolder.GetComponent<MeshRenderer>().sharedMaterials = mats;
            Debug.Log(mesh.triangles.Length+ "---->");
            yield return new WaitForSeconds(1); 
        }
        Debug.Log("over");
        //cubeFolder.SetActive(true);

        for (int i = 0, length = cubeFolder.transform.childCount; i < length; i++)
        {
            Destroy(cubeFolder.transform.GetChild(i).gameObject);
        }
    }

    //NOTE 160107 并不需要让所有地块的水位保持一致, 有水位差才有水流即河流
    //CHECK 水流作用造河效果不佳
    //目前 最佳 1*造山 + 1*均匀降雨 + 3*水流冲蚀 + 1*风化作用

    /// <summary>设置海平面</summary>
    /// <param name="seaPercent">海洋百分比0~1f</param>
    void setSeaLevel(float seaPercent)
    {
        seaPercent = Mathf.Clamp(seaPercent, 0f, 1f);
        List<AreaMono> areaList = new List<AreaMono>();
        foreach (var area in areaDic.Values)
        {
            areaList.Add(area);
        }
        areaList.Sort((x, y) => {
            if (x.AreaHeight > y.AreaHeight)
                return 1;
            else if (x.AreaHeight < y.AreaHeight)
                return -1;
            else
                return 0;
        });
        int seaLevelIndex = (int)(areaList.Count * seaPercent);
        int seaLevel = areaList[seaLevelIndex].AreaHeight;
        foreach (var area in areaDic.Values)
        {
            //area.WaterNum = 0;
            if (area.AreaHeight < seaLevel)
                area.WaterNum = seaLevel - area.AreaHeight;
            area.refreshWaterView();
        }
    }

    /// <summary>均匀降雨</summary>
    void addRain(int times, int waterNum)
    {
        foreach (var area in areaDic.Values)
        {
            area.WaterNum += 1;
            area.refreshWaterView();
        }
    }

    /// <summary>风化作用</summary>
    void windEffect(int times)
    {
        for (int t = 0; t < times; t++)
        {
            foreach (var pos in areaDic.Keys)
            {
                //确认该地块有水
                AreaMono curArea = areaDic[pos];
                AreaMono tarArea = null;
                Vector2 tarPos = pos;
                //找到附近水平面最低的
                AreaMono lowestNearbyArea = null;
                for (int i = 0, length = 9; i < length; i++)
                {
                    int ix = i % 3 - 1 + (int)pos.x;
                    int iz = i / 3 - 1 + (int)pos.y;
                    tarPos = new Vector2(ix, iz);
                    if (tarPos == pos)
                        continue;
                    if (!areaDic.ContainsKey(tarPos))
                        continue;
                    tarArea = areaDic[tarPos];
                    if (lowestNearbyArea == null)
                        lowestNearbyArea = tarArea;
                    else if (lowestNearbyArea.AreaHeight > tarArea.AreaHeight)
                        lowestNearbyArea = tarArea;
                }
                int delta = curArea.AreaHeight - lowestNearbyArea.AreaHeight;
                curArea.AreaHeight -= (int)(delta / 3f);
                lowestNearbyArea.AreaHeight += (int)(delta / 3f);
            }
        }
        foreach (var area in areaDic.Values)
        {
            //area.refreshWaterView();
            area.refreshAreaHeightView();
        }
    }

    /// <summary>造河</summary>
    void addRiver(int times)
    {
        for (int t = 0; t < times; t++)
        {
            //TODO 
            /**
            1.一次均匀降雨
            2.若干次水往低处流
            3.水流冲刷地面, 地面高度降低
            
            */


            //雨水流动 向水位更低处流动, 若无则保留
            foreach (var pos in areaDic.Keys)
            {
                //确认该地块有水
                AreaMono curArea = areaDic[pos];
                AreaMono tarArea = null;
                if (curArea.WaterNum <= 0)
                    continue;
                Vector2 tarPos = pos ;
                //找到附近水平面最低的
                AreaMono lowestNearbyArea = null;
                Action<int, int> waterFlow = (int ix, int iy) => {
                    tarPos = new Vector2(ix, iy);
                    if (tarPos == pos)
                        return;
                    if (!areaDic.ContainsKey(tarPos))
                        return;
                    tarArea = areaDic[tarPos];
                    if (lowestNearbyArea == null)
                        lowestNearbyArea = tarArea;
                    else if (lowestNearbyArea.WaterHeight > tarArea.WaterHeight)
                        lowestNearbyArea = tarArea;
                };
                ////9方向流动
                //for (int i = 0, length = 9; i < length; i++)
                //{
                //    int ix = i % 3 - 1 + (int)pos.x;
                //    int iz = i / 3 - 1 + (int)pos.y;
                //    waterFlow(ix, iz);
                //}
                //4方向流动
                waterFlow((int)pos.x + 1, (int)pos.y + 1);
                waterFlow((int)pos.x + 1, (int)pos.y - 1);
                waterFlow((int)pos.x - 1, (int)pos.y + 1);
                waterFlow((int)pos.x - 1, (int)pos.y - 1);



                //水平面比较
                int delta = curArea.WaterHeight - lowestNearbyArea.WaterHeight;
                int deltaWater = 0;

                if (delta > 3)
                    deltaWater = curArea.WaterNum >= delta ? Mathf.Max(1, (int)(delta / 3f)) : Mathf.Max(1, (int)(curArea.WaterNum / 3f));//本次转移的水量 1/3水位差
                else if (delta > 0)
                    deltaWater = 1;

                //if(tarArea != null && pos != tarPos)
                //    Debug.Log(pos+"->"+tarPos+" ["+ deltaWater + "] "+curArea.WaterNum+"->"+tarArea.WaterNum);
                if(deltaWater != 0)
                {
                    lowestNearbyArea.WaterNum += deltaWater;
                    curArea.WaterNum -= deltaWater;

                    lowestNearbyArea.AreaHeight -= 1;
                }
            }
        }

        foreach (var area in areaDic.Values)
        {
            area.refreshWaterView();
            area.refreshAreaHeightView();
        }

    }

    /// <summary>造山(随机地点，随机强度，创建山脉)</summary>
    void randomHeight(int times)
    {
        for (int i = 0; i < times; i++)
        {
            int cx = UnityEngine.Random.Range(0, lx);
            int cz = UnityEngine.Random.Range(0, lz);
            int range = UnityEngine.Random.Range(2, 15);

            int range2 = range * range;
            int power = UnityEngine.Random.Range(minY, maxY);
            for (int ix = cx - range, lenX = cx + range; ix < lenX; ix++)
            {
                for (int iz = cz - range, lenZ = cz + range; iz < lenZ; iz++)
                {
                    Vector2 curPos = new Vector2(ix, iz);
                    if (!areaDic.ContainsKey(curPos))
                        continue;
                    AreaMono area = areaDic[curPos];
                    int dis2 = (ix - cx) * (ix - cx) + (iz - cz) * (iz - cz);

                    if (dis2 < range2)
                    {
                        int curPower = (int)(power * (range2 - dis2) / range2);
                        area.AreaHeight += curPower;
                    }
                }
            }
        }
        foreach (var area in areaDic.Values)
        {
            area.refreshAreaHeightView();
        }
    }

}
