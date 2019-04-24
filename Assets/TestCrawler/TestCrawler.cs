using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Net;
using System.Text.RegularExpressions;
using LitJson;
using System.IO;

class Class1
{
}


public class TestCrawler : MonoBehaviour
{
    const string web1 = "http://www.roufan.cc/";
    const string web2 = "http://www.roufan.cc/roufan/6565_14.html";
    const string web3 = "http://www.roufan.cc/roufan/1000.html";
    const string webHead = "http://www.roufan.cc/roufan/";
    const string local1 = "file:///D:/UnitySpace/NewPro/Assets/TestCrawler/xmlTest.txt";
    //string x = "<!DOCTYPE html PUBLIC " -//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">";

    string webListPath;// = Application.streamingAssetsPath + "/webList.txt";

    const string imgPrefix = "<img src=";
    const string imgPostfix = ".jpg\"";
    const string imgPostfixPNG = ".png\"";

    void Awake()
    {
        //webListPath = Application.streamingAssetsPath + "/webList.txt";
        webListPath = Application.streamingAssetsPath + "/webList";
    }

    void Start()
    {
        //getWeb(anaWeb1);
        //tryAnalise2("");
        //Main();
        //findRightWeb();
        //startWork();
        //startWork();
        work161122();
    }

    #region 161122新方案
    void work161122()
    {
        getWeb(getUrlPostfix(6565, 18), data=> {
            Debug.Log(data);
            anaWebStr2(data);
        });
    }

    string getUrlPostfix(int a, int b)
    {
        string url;
        if (b == 1)
            url = webHead+a + "_" + ".html";
        else
            url = webHead+a + "_" + b + ".html";
        return url;
    }


    void anaWebStr2(string webStr)
    {
        Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>");
        MatchCollection matches = regImg.Matches(webStr);
        System.Collections.IEnumerator enu = matches.GetEnumerator();
        while (enu.MoveNext() && enu.Current != null)
        {
            Match match = (Match)(enu.Current);
            string rawUrl = match.Value;
            string cookedUrl = cookUrl(rawUrl);
            Debug.Log(cookedUrl);
            File.AppendAllText(Application.streamingAssetsPath+"/test161122.txt",cookedUrl+"\r\n");
        }
    }

    int imgUrlStartIndex = imgPrefix.Length+1;
    int imgUrlEndIndex = imgPostfix.Length - imgPrefix.Length-2;
    int imgUrlEndIndexPNG = imgPostfixPNG.Length - imgPrefix.Length-2;

    string cookUrl(string rawUrl)
    {
        string cookedUrl=null;
        int preIndex = rawUrl.IndexOf(imgPrefix);

        if(preIndex == 0)
        {
            int postIndex1 = rawUrl.LastIndexOf(imgPostfix);
            //int postIndex2 = rawUrl.LastIndexOf(imgPostfixPNG);
            if (postIndex1 != -1)
                cookedUrl = rawUrl.Substring(imgUrlStartIndex, postIndex1 + imgUrlEndIndex);
            //else if(postIndex2 != -1)
            //    cookedUrl = rawUrl.Substring(imgUrlStartIndex, postIndex2 + imgUrlEndIndexPNG);
        }
        return cookedUrl;
    }

    #endregion

    #region 依次查找页面并下载

    int theStartIndex = 5000;
    int theEndIndex = 5500;
    int theCurIndex = 5000;

    void startWork()
    {
        //检查当前页面
        //right-> 检查后续_2页面-> 下载图片
        //anaWeb(onSucc, onFail);
        //for (int i = 1; i <= 90; i++)
        //{
        //    theStartIndex = i * 100;
        //    theEndIndex = theStartIndex + 100;
        //    theCurIndex = theStartIndex;
        //}
        anaWeb2(theCurIndex, new JsonData(),()=> {

        });
    }


    //TODO 161011 按顺序遍历所有故事, 并记录
    void anaWeb2(int _curIndex, JsonData outputJD, Action onDone)
    {
        string webUrl = webHead + _curIndex + ".html";
        getWeb(webUrl, webStr =>
        {
            if (checkWeb(webStr))
            {
                Debug.Log("获取到故事页面:" + _curIndex);
                JsonData pageJD = new JsonData();
                pageJD["index"] = _curIndex;
                pageJD["url"] = webUrl;
                pageJD["child"] = new JsonData();
                JsonData page1 = new JsonData();
                page1["info"] = "page1";
                pageJD["child"].Add(page1);
                //pageJD
                outputJD.Add(pageJD);
                anaWebStr(webStr, page1);

                //从第二页开始解析子页面
                anaChildPage2(_curIndex, 2, pageJD, () =>
                {
                    if ((_curIndex + 1) <= theEndIndex)
                        anaWeb2(_curIndex + 1, outputJD, onDone);
                    else
                    {
                        if (!Directory.Exists(Application.streamingAssetsPath))
                            Directory.CreateDirectory(Application.streamingAssetsPath);
                        File.WriteAllText(webListPath+theStartIndex+"_"+theEndIndex+".txt", outputJD.ToJson());
                        onDone();
                    }
                });

            }
            else
            {
                Debug.Log("当前index获取页面失败!" + _curIndex);
                if ((_curIndex + 1) <= theEndIndex)
                    anaWeb2(_curIndex + 1, outputJD,onDone);
                else
                {
                    if (!Directory.Exists(Application.streamingAssetsPath))
                        Directory.CreateDirectory(Application.streamingAssetsPath);
                    File.WriteAllText(webListPath + theStartIndex + "_" + theEndIndex + ".txt", outputJD.ToJson());
                    onDone();
                }
            }

        });
    }

    void anaChildPage2(int _curIndex, int _curChildIndex, JsonData pageJD, Action onDone)
    {
        string webUrl = webHead + _curIndex + "_" + _curChildIndex + ".html";

        getWeb(webUrl, webStr =>
        {
            if (checkWeb(webStr))
            {
                JsonData childPage = new JsonData();
                childPage["info"] = _curChildIndex;
                pageJD["child"].Add(childPage);
                Debug.Log(webUrl);
                anaWebStr(webStr, childPage);
                anaChildPage2(_curIndex, _curChildIndex + 1, pageJD, onDone);
            }
            else
            {
                onDone();
            }
        });


    }

    void anaWebStr(string webStr, JsonData childPageJD)
    {
        Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>");
        MatchCollection matches = regImg.Matches(webStr);
        System.Collections.IEnumerator enu = matches.GetEnumerator();
        childPageJD["data"] = new JsonData();
        while (enu.MoveNext() && enu.Current != null)
        {
            Match match = (Match)(enu.Current);

            //Console.Write(match.Value + "\r\n");
            Debug.Log(match.Value);
            childPageJD["data"].Add(match.Value);
            //childPageJD
        }
    }

    void findNextImg(string totalStr, int startIndex)
    {
        int p1 = totalStr.IndexOf(imgPrefix, startIndex);
        if (p1 != -1)
        {
            int p2Png = totalStr.IndexOf(imgPostfixPNG, startIndex + imgPrefix.Length);
            int p2 = totalStr.IndexOf(imgPostfix, startIndex + imgPrefix.Length);
            if (p2Png < p2)
            {
                findNextImg(totalStr, p2Png);
            }
            else
            {
                string img = totalStr.Substring(p1, p2 - 1);
                Debug.Log(p1 + " " + p2);
                Debug.Log(img);
                findNextImg(totalStr, p2);

            }

            //return img;
        }
        else
        {
            //return null;
        }

    }






















    /// <summary>确定当前故事首页正确</summary>
    void onSucc(string webStr)
    {
        //建立文件夹
        //解析后续页面, 并保持
        anaChildWeb(webStr);

        //loadNextChildPage();

    }

    void onFail()
    {
        anaWeb(onSucc, onFail);
    }


    /// <summary>获取并解析故事开头页面</summary>
    void anaWeb(Action<string> succ, Action fail)
    {
        string webUrl = webHead + theCurIndex + ".html";
        getWeb(webUrl, webStr =>
        {
            if (checkWeb(webStr))
            {
                succ(webStr);
            }
            else
            {
                fail();
            }
        });
    }

    void loadNextChildPage(string url, int pageIndex)
    {

    }

    /// <summary>分析包含图片的页面</summary>
    void anaChildWeb(string webStr)
    {

    }


    #endregion

    #region 通用

    /// <summary>获取页面内容</summary>
    void getWeb(string url, Action<string> callback = null)
    {
        StartCoroutine(SendGet(url,
            msg =>
            {
                callback(msg);
            },
            (error, msg) =>
            {
                Debug.LogError(error + "\r\n" + msg);
            }
        ));
    }

    /// <summary>检查页面是否正常</summary>
    bool checkWeb(string msg)
    {
        //Debug.Log(msg);

        if (msg.IndexOf("指定的页面不存在") == -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region 查找起始页面Index
    int ff_startIndex = 2000;
    int ff_endIndex = 4000;
    int ff_curIndex = 2000;
    void findRightWeb()
    {

        findNextWeb();

    }

    //start---------ans-------end
    void findNextWeb()
    {
        string webHead = "http://www.roufan.cc/roufan/";
        string webToCheck = webHead + ff_curIndex + ".html";
        getWeb(webToCheck, msg =>
        {

            if (checkWeb(msg))
            {
                Debug.Log("找到地址: " + ff_curIndex + "");
                Debug.Log(ff_startIndex + "----" + ff_curIndex + "----" + ff_endIndex);

                ff_endIndex = ff_curIndex;
                int delta = ff_endIndex - ff_startIndex;
                int halfDelta = delta / 2;
                ff_curIndex = ff_startIndex + halfDelta;
                findNextWeb();
            }
            else
            {
                Debug.Log("错误地址: " + ff_curIndex + "");
                Debug.Log(ff_startIndex + "----" + ff_curIndex + "----" + ff_endIndex);

                ff_startIndex = ff_curIndex;
                int delta = ff_endIndex - ff_startIndex;
                int halfDelta = delta / 2;
                ff_curIndex = ff_startIndex + halfDelta;

                findNextWeb();
            }

        });
    }
    #endregion

    #region XMLTEST
    [STAThread]
    void check()
    {
        System.Net.WebClient client = new WebClient();
        byte[] page = client.DownloadData(web1);
        string content = System.Text.Encoding.UTF8.GetString(page);
        string regex = "href=[\\\"\\\'](http:\\/\\/|\\.\\/|\\/)?\\w+(\\.\\w+)*(\\/\\w+(\\.\\w+)?)*(\\/|\\?\\w*=\\w*(&\\w*=\\w*)*)?[\\\"\\\']";
        Regex re = new Regex(regex);
        MatchCollection matches = re.Matches(content);

        System.Collections.IEnumerator enu = matches.GetEnumerator();
        while (enu.MoveNext() && enu.Current != null)
        {
            Match match = (Match)(enu.Current);
            Console.Write(match.Value + "\r\n");
            Debug.Log(match.Value);
        }
    }

    void tryAnalise1(string msg)
    {
        string xmlStr = msg;
        string newXmlStr = "";
        int firstLineEnd = xmlStr.IndexOf("\r\n");
        xmlStr = xmlStr.Remove(0, firstLineEnd);
        //xmlStr = xmlStr.Replace("=", "");

        //xmlStr.Replace()

        string[] ss = xmlStr.Split(new char[2] { '\r', '\n' });
        for (int i = 0; i < ss.Length; i++)
        {
            string line = ss[i];

            if (line.StartsWith("<"))
            {
                if (line.StartsWith("<meta "))
                {
                    Debug.Log("start: " + line);
                    if (line.EndsWith("/>"))
                    {
                        Debug.Log("end1: " + line);
                        newXmlStr += "\r\n" + line;
                    }
                    else if (line.EndsWith(">"))
                    {
                        Debug.Log("end2: " + line);
                    }
                }
                else
                    newXmlStr += "\r\n" + line;

            }

        }


        Debug.Log(newXmlStr);

        XmlDocument xml = new XmlDocument();
        xml.LoadXml(newXmlStr);

    }

    void tryAnalise2(string msg)
    {

        XmlDocument xml = new XmlDocument();
        XmlReaderSettings set = new XmlReaderSettings();
        set.IgnoreComments = true;//这个设置是忽略xml注释文档的影响。有时候注释会影响到xml的读取
        xml.Load(XmlReader.Create((local1), set));
        Debug.Log(xml.InnerXml);

    }

    void LoadXml()
    {
        //创建xml文档
        XmlDocument xml = new XmlDocument();
        XmlReaderSettings set = new XmlReaderSettings();
        set.IgnoreComments = true;//这个设置是忽略xml注释文档的影响。有时候注释会影响到xml的读取
        xml.Load(XmlReader.Create((Application.dataPath + "/data.xml"), set));
        //得到objects节点下的所有子节点
        XmlNodeList xmlNodeList = xml.SelectSingleNode("objects").ChildNodes;
        //遍历所有子节点
        foreach (XmlElement xl1 in xmlNodeList)
        {

            if (xl1.GetAttribute("id") == "1")
            {
                //继续遍历id为1的节点下的子节点
                foreach (XmlElement xl2 in xl1.ChildNodes)
                {
                    //放到一个textlist文本里
                    //textList.Add(xl2.GetAttribute("name") + ": " + xl2.InnerText);
                    //得到name为a的节点里的内容。放到TextList里
                    if (xl2.GetAttribute("name") == "a")
                    {
                        //Adialogue.Add(xl2.GetAttribute("name") + ": " + xl2.InnerText);
                    }
                    //得到name为b的节点里的内容。放到TextList里
                    else if (xl2.GetAttribute("name") == "b")
                    {
                        //Bdialogue.Add(xl2.GetAttribute("name") + ": " + xl2.InnerText);
                    }
                }
            }
        }
        print(xml.OuterXml);

    }
    #endregion

    #region HTTP
    IEnumerator SendGet(string _url, Action<string> callback = null, Action<string, string> failCallback = null)
    {
        WWW getData = new WWW(_url);
        yield return getData;
        if (getData.error != null)
        {
            //Debug.Log(getData.error);
            failCallback(getData.error, getData.text);
        }
        else
        {
            //Debug.Log(getData.text);
            callback(getData.text);
        }
    }
    IEnumerator SendPost(string _url, Dictionary<string, string> dic, Action<string> callback = null, Action<string, string> failCallback = null)
    {
        WWWForm _wForm = new WWWForm();
        foreach (var key in dic.Keys)
        {
            _wForm.AddField(key, dic[key]);
        }


        WWW postData = new WWW(_url, _wForm);
        yield return postData;
        if (postData.error != null)
        {
            //Debug.Log(postData.error);
            failCallback(postData.error, postData.text);
        }
        else
        {
            //Debug.Log(postData.text);
            callback(postData.text);
        }
    }
    void testHttpSend()
    {
        //测试GET方法
        StartCoroutine(SendGet("http://kun.show.ghostry.cn/?int=5", msg => { }, (error, msg) => { }));

        //测试POST方法
        //WWWForm form = new WWWForm();
        //form.AddField("int", "6");
        Dictionary<string, string> postData = new Dictionary<string, string>();
        postData["int"] = "6";
        StartCoroutine(SendPost("http://kun.show.ghostry.cn/", postData));
    }

    #endregion

}
