using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shop_Mgr : MonoBehaviour
{
    enum ButtonOrder
    {
        Hp = 0,
        armor,
        MoveSpeed,
        AttackSpeed,
        Attack,
        Luck,
        Growth,
        Magnet,
        Greed,
        Resurrection,
        Refresh,
        Skip,
        Count
    }

    public Button[] State_Btn;
    public Text GoldTxt;
    public Button ExitBtn;

    public GameObject Buy_Panel;
    public Text Buy_Txt;
    public Button OKBtn;
    public Button NoBtn;

    public GameObject Error_Panel;
    public Button ErrorOKBtn;

    public GameObject Panel;
    float Timer = 255.0f;
    bool EnterScene = false;

    public Sprite[] LevelImg;    

    ButtonOrder Cur_Selet = ButtonOrder.Count;
    int cur_level;
    int cur_price;

    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.LoadData();
        Panel.SetActive(true);

        for (int i = 0; i < (int)ButtonOrder.Count; i++)
        {
            int ii = i;
            State_Btn[ii].onClick.AddListener(() => StateBtnFuc((ButtonOrder)ii));
        }

        OKBtn.onClick.AddListener(() => Buy_Item());
        NoBtn.onClick.AddListener(() => Buy_Panel.SetActive(false));

        ErrorOKBtn.onClick.AddListener(() => Error_Panel.SetActive(false));

        ExitBtn.onClick.AddListener(() =>
        {
            EnterScene = true;
            Panel.SetActive(true);
        });

        Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        if (Panel.activeSelf == true && EnterScene == false)
        {
            Timer -= Time.deltaTime * 255;

            if (Timer <= 0)
                Timer = 0;

            Panel.GetComponent<Image>().color = new Color(0, 0, 0, Timer / 255);

            if (Panel.GetComponent<Image>().color.a == 0.0f)
            {
                Panel.SetActive(false);
            }
        }
        else if (Panel.activeSelf == true && EnterScene == true)
        {
            Timer += Time.deltaTime * 255;

            if (Timer >= 255)
                Timer = 255;

            Panel.GetComponent<Image>().color = new Color(0, 0, 0, Timer / 255);

            if (Panel.GetComponent<Image>().color.a == 1.0f)
            {
                SceneManager.LoadScene("Lobby");
            }
        }
    }

    void StateBtnFuc(ButtonOrder state)
    {
        Cur_Selet = state;

        switch (Cur_Selet)
        {
            case ButtonOrder.Hp:
                cur_level = GlobalValue.Helth_Point;
                break;
            case ButtonOrder.armor:
                cur_level = GlobalValue.Armor_Point;
                break;
            case ButtonOrder.MoveSpeed:
                cur_level = GlobalValue.MoveSpeed_Point;
                break;
            case ButtonOrder.AttackSpeed:
                cur_level = GlobalValue.AttackSpeed_Point;
                break;
            case ButtonOrder.Attack:
                cur_level = GlobalValue.Attack_Point;
                break;
            case ButtonOrder.Luck:
                cur_level = GlobalValue.Luck_Point;
                break;
            case ButtonOrder.Growth:
                cur_level = GlobalValue.Growth_Point;
                break;
            case ButtonOrder.Magnet:
                cur_level = GlobalValue.Magnet_Point;
                break;
            case ButtonOrder.Greed:
                cur_level = GlobalValue.Greed_Point;
                break;
            case ButtonOrder.Resurrection:
                cur_level = GlobalValue.Resurrection_Point;
                break;
            case ButtonOrder.Refresh:
                cur_level = GlobalValue.Refresh_Point;
                break;
            case ButtonOrder.Skip:
                cur_level = GlobalValue.Skip_Point;
                break;
        }

        cur_price = 500 + (cur_level * 1000);

        Buy_Panel.SetActive(true);
        Buy_Txt.text = "해당 아이템의 가격은\n" +
                        cur_price + " Point 입니다.\n" +
                        "정말 구매하시겠습니까?";
    }

    void Buy_Item()
    {
        if (GlobalValue.g_UserGold >= cur_price)
        {
            GlobalValue.g_UserGold -= cur_price;
            PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);
            switch (Cur_Selet)
            {
                case ButtonOrder.Hp:
                    GlobalValue.Helth_Point += 1;
                    PlayerPrefs.SetInt("Helth_Point", GlobalValue.Helth_Point);
                    break;
                case ButtonOrder.armor:
                    GlobalValue.Armor_Point += 1;
                    PlayerPrefs.SetInt("Armor_Point", GlobalValue.Armor_Point);
                    break;
                case ButtonOrder.MoveSpeed:
                    GlobalValue.MoveSpeed_Point += 1;
                    PlayerPrefs.SetInt("MoveSpeed_Point", GlobalValue.MoveSpeed_Point);
                    break;
                case ButtonOrder.AttackSpeed:
                    GlobalValue.AttackSpeed_Point += 1;
                    PlayerPrefs.SetInt("AttackSpeed_Point", GlobalValue.AttackSpeed_Point);
                    break;
                case ButtonOrder.Attack:
                    GlobalValue.Attack_Point += 1;
                    PlayerPrefs.SetInt("Attack_Point", GlobalValue.Attack_Point);
                    break;
                case ButtonOrder.Luck:
                    GlobalValue.Luck_Point += 1;
                    PlayerPrefs.SetInt("Luck_Point", GlobalValue.Luck_Point);
                    break;
                case ButtonOrder.Growth:
                    GlobalValue.Growth_Point += 1;
                    PlayerPrefs.SetInt("Growth_Point", GlobalValue.Growth_Point);
                    break;
                case ButtonOrder.Magnet:
                    GlobalValue.Magnet_Point += 1;
                    PlayerPrefs.SetInt("Magnet_Point", GlobalValue.Magnet_Point);
                    break;
                case ButtonOrder.Greed:
                    GlobalValue.Greed_Point += 1;
                    PlayerPrefs.SetInt("Greed_Point", GlobalValue.Greed_Point);
                    break;
                case ButtonOrder.Resurrection:
                    GlobalValue.Resurrection_Point += 1;
                    PlayerPrefs.SetInt("Resurrection_Point", GlobalValue.Resurrection_Point);
                    break;
                case ButtonOrder.Refresh:
                    GlobalValue.Refresh_Point += 1;
                    PlayerPrefs.SetInt("Refresh_Point", GlobalValue.Refresh_Point);
                    break;
                case ButtonOrder.Skip:
                    GlobalValue.Skip_Point += 1;
                    PlayerPrefs.SetInt("Skip_Point", GlobalValue.Skip_Point);
                    break;
            }
            Buy_Panel.SetActive(false);
            Refresh();
        }
        else
        {
            Buy_Panel.SetActive(false);
            Error_Panel.SetActive(true);
        }
    }

    void Refresh()
    {
        Transform StateLevelImg;
        int temp = 0;

        for (int i = 0; i < (int)ButtonOrder.Count; i++)
        {
            StateLevelImg = State_Btn[i].gameObject.transform.Find("Level_Img");

            switch (i)
            {
                case (int)ButtonOrder.Hp:
                    temp = GlobalValue.Helth_Point;
                    break;
                case (int)ButtonOrder.armor:
                    temp = GlobalValue.Armor_Point;
                    break;
                case (int)ButtonOrder.MoveSpeed:
                    temp = GlobalValue.MoveSpeed_Point;
                    break;
                case (int)ButtonOrder.AttackSpeed:
                    temp = GlobalValue.AttackSpeed_Point;
                    break;
                case (int)ButtonOrder.Attack:
                    temp = GlobalValue.Attack_Point;
                    break;
                case (int)ButtonOrder.Luck:
                    temp = GlobalValue.Luck_Point;
                    break;
                case (int)ButtonOrder.Growth:
                    temp = GlobalValue.Growth_Point;
                    break;
                case (int)ButtonOrder.Magnet:
                    temp = GlobalValue.Magnet_Point;
                    break;
                case (int)ButtonOrder.Greed:
                    temp = GlobalValue.Greed_Point;
                    break;
                case (int)ButtonOrder.Resurrection:
                    temp = GlobalValue.Resurrection_Point;
                    break;
                case (int)ButtonOrder.Refresh:
                    temp = GlobalValue.Refresh_Point;
                    break;
                case (int)ButtonOrder.Skip:
                    temp = GlobalValue.Skip_Point;
                    break;
            }

            StateLevelImg.gameObject.GetComponent<Image>().sprite = LevelImg[temp];
        }

        GoldTxt.text = GlobalValue.g_UserGold.ToString();
    }
}
