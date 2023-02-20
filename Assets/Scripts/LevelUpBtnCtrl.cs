using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelUpBtnCtrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public SkillName m_CrType = SkillName.CrCount;  //초기화
    public Text m_LvText;
    public Image m_CrIconImg;
    public Text m_NameText;
    string m_SkillInfo = "";
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
        m_SkillInfo = GlobalValue.m_SkDataList[(int)m_CrType].Info;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        InGame_Mgr.Inst.SkillInfo.SetActive(true);
        InGame_Mgr.Inst.SkillInfo_Title.text = m_NameText.text;
        InGame_Mgr.Inst.SkillInfo_Info.text = m_SkillInfo;
        print("enter호출");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InGame_Mgr.Inst.SkillInfo.SetActive(false);
        print("exit호출");
    }


    public void DummyData()
    {
        m_CrType = SkillName.CrCount;
        m_CrIconImg.sprite = Resources.Load("Icon/NP_2", typeof(Sprite)) as Sprite;
        m_NameText.text = "+ 10 Point";
        m_LvText.text = "";
    }

}
