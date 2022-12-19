using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpBtnCtrl : MonoBehaviour
{
    [HideInInspector] public SkillName m_CrType = SkillName.CrCount;  //초기화
    public Text m_LvText;
    public Image m_CrIconImg;
    public Text m_NameText;
    public bool m_Dummy = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitData(SkillName a_CrType)
    {
        if (a_CrType < SkillName.Kuuga || SkillName.CrCount <= a_CrType)
            return;

        m_CrType = a_CrType;
        m_CrIconImg.sprite = GlobalValue.m_SkDataList[(int)m_CrType].IconImg;
        m_NameText.text = GlobalValue.m_SkDataList[(int)m_CrType].Name;
        m_LvText.text = "LV " + (GlobalValue.m_SkDataList[(int)m_CrType].m_Level + 1);

        if (GlobalValue.m_SkDataList[(int)m_CrType].m_Level == 0)
        {
            m_LvText.text = "NEW !!";
        }
        else if (GlobalValue.m_SkDataList[(int)m_CrType].m_Level == 4)
        {
            m_LvText.text = "LV MAX";
        }
    }

    public void DummyData()
    {
        m_CrType = SkillName.CrCount;
        m_CrIconImg.sprite = Resources.Load("Icon/NP_2", typeof(Sprite)) as Sprite;
        m_NameText.text = "+ 10 Point";
        m_LvText.text = "";
    }
}
