using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemNodeCtrl : MonoBehaviour
{
    [HideInInspector] public SkillName m_CrType = SkillName.CrCount;  //초기화
    public Image Icon_Img;
    public Image Lavel1_Img;
    public Sprite[] Level_Sprite;

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

        if (m_CrType == SkillName.CrCount)
        {
            m_CrType = a_CrType;
            Icon_Img.sprite = GlobalValue.m_SkDataList[(int)m_CrType].IconImg;
        }

        Lavel1_Img.sprite = Level_Sprite[GlobalValue.m_SkDataList[(int)m_CrType].m_Level - 1];
    }

    public void InitData()
    {
        Lavel1_Img.sprite = Level_Sprite[GlobalValue.m_SkDataList[(int)m_CrType].m_Level - 1];
    }

}
