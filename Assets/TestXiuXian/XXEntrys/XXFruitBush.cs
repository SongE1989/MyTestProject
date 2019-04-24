using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XXFruitBush : XXHerb
{
    public List<GameObject> mFruitList = new List<GameObject>();

    protected override void Start()
    {
        base.Start();
    }

    protected override void setState(EnumHerbState state)
    {
        m_kState = state;
        if (m_kState == EnumHerbState.Growing)
        {
            foreach (var fruit in mFruitList)
            {
                fruit.transform.DOScale(new Vector3(0f, 0f, 0f), 0.15f);
            }

        }
        else if (m_kState == EnumHerbState.Fruited)
        {
            foreach (var fruit in mFruitList)
            {
                fruit.transform.DOScale(new Vector3(2f, 2f, 1f), 0.15f);
            }
        }
    }

    public override void GetPicked()
    {
        base.GetPicked();
        if (mAnimator)
            mAnimator.SetTrigger("getCollect");
    }
}
