using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace XiuXianStory
{
    public class NameCreator
    {
        static string familyNameString;
        static string singleFamlilyNameString;
        static string[] multiFamilyNameArr;
        static string oneWordNameString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static string twoWordFrontNameString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static string twoWordBackNameString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        static bool isInited = false;
        static void init()
        {
            if (isInited)
                return;
            isInited = true;

            var path = Application.dataPath + "/Resources/XiuXianFolder/百家姓.txt";
            familyNameString = File.ReadAllText(path);
            var strArr = familyNameString.Split(new string[1] { "--" }, StringSplitOptions.RemoveEmptyEntries);
            singleFamlilyNameString = strArr[0];
            multiFamilyNameArr = strArr[1].Split(' ');
        }

        public static string GetRandomName()
        {
            init();
            return getRandomFamilyName() + getRandomPersonalName();
        }

        static string getRandomPersonalName()
        {
            float twoWordNameRate = 0.5f;
            if(UnityEngine.Random.Range(0,1f)<twoWordNameRate)
            {
                return oneWordNameString[UnityEngine.Random.Range(0, twoWordFrontNameString.Length)].ToString()
                + oneWordNameString[UnityEngine.Random.Range(0, twoWordBackNameString.Length)].ToString();
            }
            else
            {
                return oneWordNameString[UnityEngine.Random.Range(0, oneWordNameString.Length)].ToString();
            }
        }

        static string getRandomFamilyName()
        {
            init();
            float multiNameRate = 0.1f;
            if (UnityEngine.Random.Range(0, 1f) > multiNameRate)
            {
                var index = UnityEngine.Random.Range(0, singleFamlilyNameString.Length);
                return singleFamlilyNameString[index].ToString();
            }
            else
            {
                var index = UnityEngine.Random.Range(0, multiFamilyNameArr.Length);
                return multiFamilyNameArr[index];
            }
        }

    }
}