using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharName
{
    Decade = 0,
    Decade_Complete,
    Decade_Gekijou,
    DiEnd,
    ZiO,
    Geiz,
    CrCount,
}

public enum SkillName
{
    Kuuga = 0,
    Agito,
    Ryuki,
    Faiz,
    Blade,
    Hibiki,
    Kabuto,
    DenO,
    Kiva,
    Decade,
    CrCount,
}


public class CharInfo  //각 Item 정보
{
    public string Name = "";                    // 캐릭터 이름
    public CharName CrType = CharName.Decade; // 캐릭터 타입
    public int Price = 0;                       // 기본 가격 
    public string BasicWeapon = "";             // 기본 무기 설명
    public string Ability = "";                 // 특수능력 설명
    public Sprite IconImg = null;               // 캐릭터 이미지

    public void SetType(CharName a_CrType)
    {
        CrType = a_CrType;

        if (a_CrType == CharName.Decade)
        {
            Price = 0;
            Name = "DECADE";
            BasicWeapon = "SLASH";
            Ability = "장비슬롯 증가";
            IconImg = Resources.Load("RiderCard/Decade", typeof(Sprite)) as Sprite;
        }
        else if(a_CrType == CharName.Decade_Gekijou)
        {
            Price = 10000;
            Name = "DECADE_GEKIJOU";
            BasicWeapon = "BLAST";
            Ability = "넉백을 가진 기본공격";
            IconImg = Resources.Load("RiderCard/Decade_Gekijou", typeof(Sprite)) as Sprite;
        }
    }
}

public class SkillInfo  //각 스킬 정보
{
    public string Name = "";                    // 스킬 이름
    public string Info = "";                    // 스킬 설명
    public SkillName CrType = SkillName.Kuuga;  // 캐릭터 타입
    public Sprite IconImg = null;               // 캐릭터 이미지
    public int m_Level = 0;

    public void SetType(SkillName a_CrType)
    {
        CrType = a_CrType;

        if (a_CrType == SkillName.Kuuga)
        {
            Name = "쿠우가 (패시브)";
            Info = "최대 체력, 회복, 방어력, 피해량이 증가한다.";
            IconImg = Resources.Load("Icon/Kuuga", typeof(Sprite)) as Sprite;
            m_Level = 0;
        }
        else if (a_CrType == SkillName.Agito)
        {
            Name = "아기토 (액티브)";
            Info = "가장 가까운 적에게 강력한 근접 공격을 한다.";
            IconImg = Resources.Load("Icon/Agito", typeof(Sprite)) as Sprite;
            m_Level = 0;
        }
        else if (a_CrType == SkillName.Ryuki)
        {
            Name = "류우키 (액티브)";
            Info = "드래그레더를 소환하여 화살표 방향으로 발사한다.";
            IconImg = Resources.Load("Icon/Ryuki", typeof(Sprite)) as Sprite;
            m_Level = 0;
        }
        else if (a_CrType == SkillName.Faiz)
        {
            Name = "파이즈 (액티브)";
            Info = "포인터로 가장 가까운 적을 속박한다. 그 후 포인터와 접촉시 강력한 대미지를 입힌다.";
            IconImg = Resources.Load("Icon/Faiz", typeof(Sprite)) as Sprite;
            m_Level = 0;
        }
        else if (a_CrType == SkillName.Blade)
        {
            Name = "블레이드 (액티브)";
            Info = "캐릭터를 중심으로 회전하는 카드를 소환한다.";
            IconImg = Resources.Load("Icon/Blade", typeof(Sprite)) as Sprite;
            m_Level = 0;
        }
        else if (a_CrType == SkillName.Hibiki)
        {
            Name = "히비키 (액티브)";
            Info = "포물선으로 떨어지는 음격봉을 캐릭터의 위쪽으로 던진다.";
            IconImg = Resources.Load("Icon/Hibiki", typeof(Sprite)) as Sprite;
            m_Level = 0;
        }
        else if (a_CrType == SkillName.Kabuto)
        {
            Name = "카부토 (패시브)";
            Info = "이동속도, 공격속도가 증가한다.";
            IconImg = Resources.Load("Icon/Kabuto", typeof(Sprite)) as Sprite;
            m_Level = 0;
        }
        else if (a_CrType == SkillName.DenO)
        {
            Name = "덴오 (액티브)";
            Info = "왼쪽에서 캐릭터를 꿰뚫고 지나가는 덴라이너를 소환한다.";
            IconImg = Resources.Load("Icon/Den-O", typeof(Sprite)) as Sprite;
            m_Level = 0;
        }
        else if (a_CrType == SkillName.Kiva)
        {
            Name = "키바 (액티브)";
            Info = "자신에게서 가장 가까운적을 추적하는 흡혈박쥐를 소환한다.";
            IconImg = Resources.Load("Icon/Kiva", typeof(Sprite)) as Sprite;
            m_Level = 0;
        }
        else if (a_CrType == SkillName.Decade)
        {
            Name = "디케이드 (기본무기)";
            Info = "화살표 방향으로 칼을 휘두른다.";
            IconImg = Resources.Load("Icon/Decade", typeof(Sprite)) as Sprite;
            m_Level = 1;
        }
    }
}

public class GlobalValue : MonoBehaviour
{
    public static int g_UserGold = 0;       //게임머니

    public static int Helth_Point = 0;
    public static int Armor_Point = 0;
    public static int MoveSpeed_Point = 0;
    public static int AttackSpeed_Point = 0;
    public static int Attack_Point = 0;
    public static int Luck_Point = 0;
    public static int Growth_Point = 0;
    public static int Magnet_Point = 0;
    public static int Greed_Point = 0;
    public static int Resurrection_Point = 0;
    public static int Refresh_Point = 0;
    public static int Skip_Point = 0;

    //캐릭터 아이템 데이터 리스트
    public static List<CharInfo> m_CrDataList = new List<CharInfo>();
    public static List<SkillInfo> m_SkDataList = new List<SkillInfo>();

    public static void LoadData()
    {
        g_UserGold = PlayerPrefs.GetInt("UserGold", 0);

        Helth_Point = PlayerPrefs.GetInt("Helth_Point", 0);
        Armor_Point = PlayerPrefs.GetInt("Armor_Point", 0);
        MoveSpeed_Point = PlayerPrefs.GetInt("MoveSpeed_Point", 0);
        AttackSpeed_Point = PlayerPrefs.GetInt("AttackSpeed_Point", 0);
        Attack_Point = PlayerPrefs.GetInt("Attack_Point", 0);
        Luck_Point = PlayerPrefs.GetInt("Luck_Point", 0);
        Growth_Point = PlayerPrefs.GetInt("Growth_Point", 0);
        Magnet_Point = PlayerPrefs.GetInt("Magnet_Point", 0);
        Greed_Point = PlayerPrefs.GetInt("Greed_Point", 0);
        Resurrection_Point = PlayerPrefs.GetInt("Resurrection_Point", 0);
        Refresh_Point = PlayerPrefs.GetInt("Refresh_Point", 0);
        Skip_Point = PlayerPrefs.GetInt("Skip_Point", 0);
    }

    public static void CrInitData()
    {
        if (0 < m_CrDataList.Count)
            return;

        CharInfo a_CrItemNd;
        for (int ii = 0; ii < (int)CharName.CrCount; ii++)
        {
            a_CrItemNd = new CharInfo();
            a_CrItemNd.SetType((CharName)ii);
            m_CrDataList.Add(a_CrItemNd);
        }
    }//public static void InitData()

    public static void SkInitData()
    {
        m_SkDataList.Clear();

        SkillInfo a_SkItemNd;
        for (int ii = 0; ii < (int)SkillName.CrCount; ii++)
        {
            a_SkItemNd = new SkillInfo();
            a_SkItemNd.SetType((SkillName)ii);
            m_SkDataList.Add(a_SkItemNd);
        }
    }//public static void InitData()
}
