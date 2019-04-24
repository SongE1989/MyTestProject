using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

public class EHCrawler : MonoBehaviour
{
    public string[] UrlArr = new string[] { };

    string url1 = "https://e-hentai.org/lofi/s/8d31e3228c/710037-3";
    const string imgPrefix = "<img src=";
    const string imgPostfix = ".jpg\"";
    const string imgPostfixPNG = ".png\"";
    int imgUrlStartIndex = imgPrefix.Length + 1;
    int imgUrlEndIndex = imgPostfix.Length - imgPrefix.Length - 2;
    int imgUrlEndIndexPNG = imgPostfixPNG.Length - imgPrefix.Length - 2;

    string htmlXmlReg = @"<(\S*?)[^>]*>.*?|<.*? />";
    string httpJpgReg = @"http:[^<>]+?\.jpg";
    string altReg = "alt=\"[^<>]*?\"";
    string titleReg = @"<title>[^<>]*</title>";

    /// <summary>等待</summary>
    IEnumerator DoWait(float waitTime, Action onDone)
    {
        yield return new WaitForSeconds(waitTime);
        onDone();
    }

    //1.加载当前webUrl
    //2.加载成功得到webXml
    //3.解析出下一个webUrl, 和当前页的picUrl
    //3.1 若下一个webUrl == 当前webUrl, => 结束
    //4.加载当前页的picUrl
    //5.等待2秒
    //6.当前页webUrl->下一页webUrl, 执行1

    string getSavePath(string title, string imgName)
    {
        return "D:/emu/pic/EH/" + title + "/" + imgName;
    }

    List<string> tarUrlList;
    int curTaskID = -1;
    [ContextMenu("testLoadAll")]
    void testLoadAll()
    {
        tarUrlList = new List<string>(UrlArr);
        curTaskID = 0;
        loadFirstPage();

    }


    void loadFirstPage()
    {
        curTaskID++;
        if (tarUrlList.Count == 0)
        {
            Debug.Log("The list is empty, all done!");
            return;
        }
        string webUrl = tarUrlList[0];
        if(string.IsNullOrEmpty(webUrl))
        {
            tarUrlList.RemoveAt(0);
            loadFirstPage();
            return;
        }

        Debug.Log("["+ curTaskID+"]Load First Page: \r\n" + webUrl);
        getWeb(webUrl, res =>
        {
            tarUrlList.RemoveAt(0);
            string title = getMatchString(res, titleReg);
            title = title.Substring(7, title.Length - 7);//removeHead
            string titleTail = getMatchString(title, @" Page [0-9]* - E-Hentai Lo-Fi Galleries</title>");
            title = title.Substring(0, title.Length - titleTail.Length);//remove tail
            title = title.Replace("|", "_");
            title = title.Replace("&", "_");
            title = title.Replace("?", "_");
            title = title.Replace("<", "_");
            title = title.Replace(">", "_");
            title = title.Replace("*", "_");
            title = title.Replace(":", "_");
            title = title.Replace(";", "_");
            title = title.Replace("/", "_");
            title = title.Replace("\\", "_");
            Debug.Log("Create Folder: " + title);

            Directory.CreateDirectory(getSavePath(title, ""));

            string hrefUrl;
            string imgUrl;
            string imgName;
            getDataFromWebXml(res, out hrefUrl, out imgUrl, out imgName);
            Debug.Log("getDataFromWebXml: " + hrefUrl + "\r\n" + imgUrl + "\r\n" + imgName);
            StartCoroutine(DoWait(1, () => {

                loadSavePic(imgUrl, getSavePath(title, imgName), flag => {
                    if (webUrl == hrefUrl)
                    {
                        Debug.Log("Load End");
                        loadFirstPage();
                    }
                    else
                    {
                        StartCoroutine(DoWait(1, () => {
                            loadNextPage(hrefUrl, title, true, loadFirstPage);
                        }));
                    }
                });

            }));

        }, () => {
            Debug.Log("loadFirstPageFail. TryAgain");
            loadFirstPage();
        });
    }

    string newTitle = "Title2";
    void loadNextPage(string webUrl, string title, bool loadNext, Action onLoadEnd)
    {
        Debug.Log("[" + curTaskID + "]loadNextPage\r\n" + webUrl);
        getWeb(webUrl, res =>
        {
            string hrefUrl;
            string imgUrl;
            string imgName;
            getDataFromWebXml(res, out hrefUrl, out imgUrl, out imgName);
            Directory.CreateDirectory(getSavePath(title, ""));
            Debug.Log("getDataFromWebXml: " + hrefUrl + "\r\n" + imgUrl + "\r\n" + imgName);
            StartCoroutine(DoWait(1, () => {
                //Directory.CreateDirectory(Application.streamingAssetsPath + "/" + title);
                loadSavePic(imgUrl, getSavePath(title, imgName), flag => {
                    if (webUrl == hrefUrl)
                    {
                        Debug.Log("LastPage");
                        if (onLoadEnd != null)
                            onLoadEnd();
                    }
                    else
                    {
                        StartCoroutine(DoWait(1, () => {
                            if (loadNext)
                                loadNextPage(hrefUrl, title, true, onLoadEnd);
                        }));
                    }
                });

            }));

        });

    }

    void getDataFromWebXml(string xml, out string hrefUrl, out string imgUrl, out string imgName)
    {
        string regStr_href = @"<a href[^<>]*?\b[\s\t\r\n]*[^<>]*?/?[\s\t\r\n]*>";
        string regStr_img = @"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>";

        string hrefHead = "<a href=\"";
        string hrefTail = "\">";
        string imgHead = "<img id=\"sm\" src=\"";
        string imgTail = "\">";

        //string pageXml = File.ReadAllText(Application.streamingAssetsPath + "/testEHPage.txt");
        string total = getMatchString(xml, regStr_href + regStr_img);
        string nextPageHref = getMatchString(total, regStr_href);
        hrefUrl = nextPageHref.Substring(hrefHead.Length, nextPageHref.Length - hrefHead.Length - hrefTail.Length);
        imgUrl = getMatchString(total, httpJpgReg);
        imgName = getMatchString(total, altReg);
        imgName = imgName.Substring(5, imgName.Length - 6);
        Debug.Log(imgName);
    }


    public string reg = "";
    [ContextMenu("测试正则 (输入Reg)")]
    void test2()
    {
        string pageXml = File.ReadAllText(Application.streamingAssetsPath + "/testEHPage.txt");
        Regex regImg = new Regex(reg);
        MatchCollection matches = regImg.Matches(pageXml);
        System.Collections.IEnumerator enu = matches.GetEnumerator();
        while (enu.MoveNext() && enu.Current != null)
        {
            Match match = (Match)(enu.Current);
            string tarStr = match.Value;
            Debug.Log(tarStr);
        }
    }

    #region 通用

    /// <summary>获取页面内容</summary>
    void getWeb(string url, Action<string> callback = null, Action failCallback = null, int retryTimes = 5)
    {
        StartCoroutine(SendGet(url,
            msg =>
            {
                callback(msg);
            },
            (error, msg) =>
            {
                Debug.LogError(error + "\r\n" + msg);
                if (retryTimes <= 1)
                {
                    if (failCallback != null)
                        failCallback();
                }
                else
                {
                    Debug.Log("retry: " + (retryTimes - 1).ToString());
                    getWeb(url, callback, failCallback, retryTimes - 1);
                }
            }
        ));
    }

    void loadSavePic(string url, string path, Action<bool> onDone, int retryTimes = 5)
    {
        StartCoroutine(SendGetByte(url, bytes => {
            Debug.Log("Pic loaded, saved to: " + path);
            File.WriteAllBytes(path, bytes);
            onDone(true);
        }, (error, text) => {
            if (retryTimes <= 1)
            {
                Debug.Log(error + "\r\n" + text);
                onDone(false);
            }
            else
            {
                Debug.Log("retry: " + (retryTimes - 1).ToString());
                loadSavePic(url, path, onDone, retryTimes - 1);
            }
        }));

    }

    string getMatchString(string str, string regStr)
    {
        Regex reg = new Regex(regStr);
        MatchCollection matches = reg.Matches(str);
        IEnumerator enu = matches.GetEnumerator();
        while (enu.MoveNext() && enu.Current != null)
        {
            Match match = (Match)(enu.Current);
            return match.Value;
        }
        return null;
    }

    #endregion

    #region HTTP

    IEnumerator SendGetByte(string _url, Action<byte[]> callback = null, Action<string, string> failCallback = null)
    {
        WWW getData = new WWW(_url);
        yield return getData;
        if (getData.error != null)
        {
            //Debug.Log(getData.error);
            if (failCallback != null)
                failCallback(getData.error, getData.text);
        }
        else
        {
            //Debug.Log(getData.text);
            callback(getData.bytes);
        }
    }
    IEnumerator SendGet(string _url, Action<string> callback = null, Action<string, string> failCallback = null)
    {
        WWW getData = new WWW(_url);
        yield return getData;
        if (getData.error != null)
        {
            //Debug.Log(getData.error);
            if (failCallback != null)
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
