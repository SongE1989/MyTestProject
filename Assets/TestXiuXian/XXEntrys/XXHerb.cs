using UnityEngine;
using System.Collections;
using DG.Tweening;

public class XXHerb : XXEntry
{
    public enum EnumHerbState
    {
        Growing,
        Fruited,
    }

    public GameObject FruitModel;

    public EnumHerbState m_kState;
    public bool IsPicking = false;

    protected override void Start()
    {
        initState();
    }

    float m_kTimeInCurState = 0f;
    float m_kTime2Fruited = 5f;

    protected override void Update ()
    {
        base.Update();
	    if(m_kState == EnumHerbState.Growing)
        {
            m_kTimeInCurState += Time.deltaTime;
            if(m_kTimeInCurState > m_kTime2Fruited)
            {
                setState(EnumHerbState.Fruited);
                m_kTimeInCurState = 0;
            }
        }
	}

    protected void initState()
    {
        if (Random.Range(0f, 1f) > 0.5f)
            setState(EnumHerbState.Fruited);
        else
            setState(EnumHerbState.Growing);
    }

    protected virtual void setState(EnumHerbState state)
    {
        m_kState = state;
        if(m_kState == EnumHerbState.Growing)
        {
            FruitModel.transform.DOScale(new Vector3(1f, 0.2f, 2f),0.15f);
            //FruitModel.transform.localScale = new Vector3(1f, 0.2f, 2f);
            
        }
        else if(m_kState == EnumHerbState.Fruited)
        {
            FruitModel.transform.DOScale(new Vector3(5f, 0.2f, 2f), 1f);
            //FruitModel.transform.localScale = new Vector3(5f, 0.2f, 2f);
        }
    }
    
    public bool CanPick()
    {
        return m_kState == EnumHerbState.Fruited && !IsPicking;
    }

    public virtual void GetPicked()
    {
        setState(EnumHerbState.Growing);
        IsPicking = false;
    }
}
