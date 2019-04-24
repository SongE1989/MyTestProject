using UnityEngine;
using System.Collections;
using songeP;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using LitJson;

namespace testTownNS
{

    public class TestTown171112 : MonoBehaviour
    {
        public Text MessageBoard;
        public int CurrentDay = 1;
        List<Location> locationList;

        #region Save & Load
        public const string SavePath = "/TestTown171112/SaveData.jd";
        [ContextMenu("初始化存档")]
        void initData()
        {
            ////TODO 整理Group模板, 并制作Group模板选择工具
            //groupEntryList.Clear();
            //GroupEntry f1 = new GroupEntry();
            //GroupManager.Instance.InitGroupType(f1.m_kType, "Farmer");
            //f1.Items.Money = 400;
            //f1.Items.Add("Wheat", 100);
            //groupEntryList.Add(f1);
            //f1 = new GroupEntry();
            //GroupManager.Instance.InitGroupType(f1.m_kType, "Smith");
            //f1.Items.Money = 400;
            //f1.Items.Add("Wheat", 100);
            //groupEntryList.Add(f1);
            //Save();
        }

        public void Save()
        {
            string saveFolderPath = FileUtil.GetFolderPath(Application.streamingAssetsPath + SavePath);
            if(!Directory.Exists(saveFolderPath))
                Directory.CreateDirectory(saveFolderPath);
            JsonData saveJD = JsonUtils.EmptyJsonObject;
            saveJD["Date"] = CurrentDay;
            saveJD["locationList"] = locationList.ToJsonDataList();
            File.WriteAllText(Application.streamingAssetsPath + SavePath, saveJD.ToJson());
            Debug.Log("保存成功");
        }

        public void Load()
        {
            JsonData loadJD = JsonUtils.ReadJsonFile(Application.streamingAssetsPath + SavePath);
            CurrentDay = loadJD.ReadInt("loadJD", 1);
            locationList = loadJD["locationList"].ToItemVOList<Location>();
            Debug.Log("读取成功");
        }
        #endregion

        public void Next()
        {
            CurrentDay++;

            string msg = "Day:" + CurrentDay + "\r\n";
            for (int i_location = 0, n_location = locationList.Count; i_location < n_location; i_location++)
            {
                var location = locationList[i_location];
                location.Next();
                msg += location.GetInfo();
            }
            
            Debug.Log(msg);
            if (MessageBoard != null)
                MessageBoard.text = msg;
        }



    }
}