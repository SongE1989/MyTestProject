using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PieGraph : MonoBehaviour
{
    public Image Prefab;
    public List<float> DataList;
    List<Image> goList = new List<Image>();
    

    [ContextMenu("RefreshView")]
    public void RefreshView()
    {
        if(DataList !=null && Prefab != null)
        {
            float total = 0;
            for (int iList = 0, nList = DataList.Count; iList < nList; iList++)
            {
                var data = DataList[iList] ;
                total += data;
            }
            if (total <= 0)
                total = 1;


            if(goList.Count > DataList.Count)
            {
                for (int i = goList.Count-1; i >= DataList.Count; i--)
                {
                    var oldGO = goList[i];
                    goList.Remove(oldGO);
                    Destroy(oldGO);
                }
            }
            else if(goList.Count < DataList.Count)
            {
                for (int i = goList.Count, length = DataList.Count; i < length; i++)
                {
                    
                    goList.Add(createImg());
                }
            }

            float curPercent = 0;
            for (int iList = 0, nList = DataList.Count; iList < nList; iList++)
            {
                var data = DataList[iList];
                var img = goList[iList];
                if (img == null)
                    img = goList[iList] = createImg();
                float percent = data / total;
                setPercent(img, percent, curPercent);
                curPercent += percent;
            }
        }
    }

    Image createImg()
    {
        Image newGO = GameObject.Instantiate(Prefab);

        newGO.transform.parent = transform;
        newGO.gameObject.SetActive(true);
        return newGO;
    }

    void setPercent(Image img, float percent, float curPercent)
    {
        img.fillAmount = percent;
        img.rectTransform.sizeDelta = Vector2.zero;
        img.rectTransform.anchoredPosition = Vector2.zero;
        img.transform.rotation = Quaternion.Euler(0,0,-curPercent*360f);
        img.color = Random.ColorHSV();
    }
}
