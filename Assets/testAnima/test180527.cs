using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test180527 : MonoBehaviour {

    [ContextMenu("test")]
    void test()
    {
        SongeUtil.forAllChildren(gameObject, tar => {
            SpriteRenderer sr = tar.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                //Debug.Log(sr.name + "," + sr.renderingLayerMask+" "+sr.sortingOrder);
                tar.transform.position = new Vector3(tar.transform.position.x, tar.transform.position.y, -sr.sortingOrder / 100f);
                sr.sortingOrder = 0;
            }
        });
    }
    //[ContextMenu("test2")]
    //void test2()
    //{
    //    SongeUtil.forAllChildren(gameObject, tar => {
    //        SpriteRenderer sr = tar.GetComponent<SpriteRenderer>();
    //        if (sr != null)
    //        {
    //            //Debug.Log(sr.name + "," + sr.renderingLayerMask+" "+sr.sortingOrder);
    //            //transform.position = new Vector3(transform.position.x, transform.position.y, -sr.sortingOrder / 100f);
    //            sr.sortingOrder = 0;
    //        }
    //    });
    //}
    Animator anima;
    // Use this for initialization
    void Start () {
        //anima = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Debug.Log(anima.deltaPosition + "" + anima.deltaRotation);
        //transform.LookAt(Camera.main.transform);
    }
}
