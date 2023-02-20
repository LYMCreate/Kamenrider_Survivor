using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum GameState
{
    GamePlay,
    GamePause,
    GameEnd,
}

public class InGame_Mgr : MonoBehaviour
{
    public static GameState s_GameState = GameState.GamePause;

    bool isGameOver = false;

    // --- SpawnPoint 관련 변수
    Transform SpawnPointGroup;
    public GameObject SpawnPointPrefab;
    public Transform[] points;
    int PointNumber = 0;
    // --- SpawnPoint 관련 변수

    int m_MonCurNum = 0;
    int maxMonster = 150;
    float spontime = 0;
    public GameObject[] monsterPrefabs;
    Transform MonsterSpawnGroup;

    // --- DamageText 관련 변수
    public Transform Damage_Canvas = null;
    public GameObject Damage_Txt_Obj = null;
    // --- DamageText 관련 변수

    public Text Game_TimerTxt;
    public float Game_Timer = 0;

    // --- 레벨관련 변수
    [Header("---- Level ----")]
    public Image Exp_bar;
    public Text Char_LevelTxt;
    public int Char_Level = 1;
    float Next_Exp = 5;
    float Cur_Exp = 0;
    // --- 레벨관련 변수

    // --- 게임오버 관련
    [Header("---- GameOver ----")]
    public GameObject GameOver_Pannel;
    public GameObject GameOverImage;
    public Button ResultBtn;
    float GameOverImage_time = 0;

    public GameObject Result_Panel;
    public Text Result_Text;
    public Button Result_LobyBtn;
    public GameObject FadePanel;
    float PanelTimer = 255.0f;
    // --- 게임오버 관련

    // --- 레벨업 & 메뉴 관련
    [Header("---- LevelUp & Menu ----")]
    public GameObject Status_Img;
    public Text Status_Txt;
    public GameObject LevelUpPanel;
    public Button SkillGetBtn_1;
    public Button SkillGetBtn_2;
    public Button SkillGetBtn_3;
    public Button Skill_RefreshBtn;
    public Button Skill_SkipBtn;
    public Text Refresh_Txt;
    public Text Skip_Txt;
    public GameObject SkillInfo;
    public Text SkillInfo_Title;
    public Text SkillInfo_Info;

    public GameObject Menu_Panel;
    public Button ResumeBtn;
    public Button LobbyBtn;
    public Button ExitBtn;
    bool LobbyBtnOn = false;
    // --- 레벨업 & 메뉴 관련

    // 인벤토리 관련
    [Header("---- Inventory ----")]
    public GameObject Inventory;
    public GameObject Item_Node;

    int MaxInven = 5;
    int CrSkNum = 1;

    [HideInInspector] public int Cur_Gold = 0;
    [HideInInspector] public int Kill_Enemy = 0;

    float temp_Eneny = 0;
    float temp_Gold = 0;
    float temp_Level = 0;
    float temp_Timer = 0.0f;

    CameraCtrl RefCamCtrl = null;

    public static InGame_Mgr Inst;

    private void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.LoadData();
        s_GameState = GameState.GamePause;
        isGameOver = false;

        GlobalValue.SkInitData();

        RefCamCtrl = FindObjectOfType<CameraCtrl>();

        MonsterSpawnGroup = GameObject.Find("MonsterSpawnGroup").GetComponent<Transform>();
        SpawnPointGroup = GameObject.Find("PointGroup").GetComponent<Transform>();
        //for (int i = -220; i <= 220; i += 20)
        //{
        //    for (int j = -220; j <= 220; j += 20)
        //    {
        //        PointNumber++;
        //        GameObject SpawnPoint = (GameObject)Instantiate(SpawnPointPrefab);
        //        SpawnPoint.name = "Point_" + PointNumber.ToString();
        //        SpawnPoint.transform.position = new Vector3(i, j, 1);
        //        SpawnPoint.transform.parent = SpawnPointGroup;
        //    }
        //}

        points = GameObject.Find("PointGroup").GetComponentsInChildren<Transform>();

        if (points.Length > 0)
        {
            print(1);
            //몬스터 생성 코루틴 함수 호출
            StartCoroutine(this.CreateMonster());
        }

        // --- 버튼들
        // --- 결과창 버튼
        ResultBtn.onClick.AddListener(() => Result_Panel.SetActive(true));
        Result_LobyBtn.onClick.AddListener(() => LobbyBtnFuc());

        // --- 스킬 획득 버튼
        SkillGetBtn_1.onClick.AddListener(() => SkillGet(SkillGetBtn_1.GetComponent<LevelUpBtnCtrl>().m_CrType));
        SkillGetBtn_2.onClick.AddListener(() => SkillGet(SkillGetBtn_2.GetComponent<LevelUpBtnCtrl>().m_CrType));
        SkillGetBtn_3.onClick.AddListener(() => SkillGet(SkillGetBtn_3.GetComponent<LevelUpBtnCtrl>().m_CrType));
        Skill_RefreshBtn.onClick.AddListener(() =>
        {
            if (CharCtrl.Inst.Item_Refresh >= 1)
            {
                LevelUp();
                CharCtrl.Inst.Item_Refresh -= 1;
                Refresh_Txt.text = CharCtrl.Inst.Item_Refresh + "회 남음";
            }
        });
        Skill_SkipBtn.onClick.AddListener(() => SkipBtnFunc());

        // --- 메뉴창 버튼
        ResumeBtn.onClick.AddListener(() =>
        {
            Menu_Panel.SetActive(false);
            Status_Img.SetActive(false);
            Time.timeScale = 1.0f;
            InGame_Mgr.s_GameState = GameState.GamePlay;
        });

        LobbyBtn.onClick.AddListener(() =>
        {
            LobbyBtnOn = true;
            Time.timeScale = 1.0f;
            FadePanel.SetActive(true);
        });
        ExitBtn.onClick.AddListener(() => Application.Quit());
        // --- 버튼들

        CrSkNum = 1;

        GameObject a_ItemNode = Instantiate(Item_Node) as GameObject;
        ItemNodeCtrl a_ItemNodeCtrl = a_ItemNode.GetComponent<ItemNodeCtrl>();
        a_ItemNodeCtrl.InitData(SkillName.Decade);
        a_ItemNode.transform.SetParent(Inventory.transform, false);

        FadePanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (FadePanel.activeSelf == true)
        {
            FadeInOut();
            return;
        }

        //SpawnPointActive();
        Timer_Refresh();
        MenuOpen();

        if (s_GameState == GameState.GameEnd)
        {
            isGameOver = true;
            GameOver();
        }

        if (Result_Panel.activeSelf == true)
        {
            Result_Play();
        }
    }

    void FadeInOut()
    {
        if (s_GameState == GameState.GamePause && LobbyBtnOn == false)
        {
            PanelTimer -= Time.deltaTime * 255;

            if (PanelTimer <= 0)
                PanelTimer = 0;

            FadePanel.GetComponent<Image>().color = new Color(0, 0, 0, PanelTimer / 255);

            if (FadePanel.GetComponent<Image>().color.a == 0.0f)
            {
                FadePanel.SetActive(false);
                s_GameState = GameState.GamePlay;
            }
        }
        else if (s_GameState == GameState.GameEnd || LobbyBtnOn == true)
        {
            PanelTimer += Time.deltaTime * 255;

            if (PanelTimer >= 255)
                PanelTimer = 255;

            FadePanel.GetComponent<Image>().color = new Color(0, 0, 0, PanelTimer / 255);

            if (FadePanel.GetComponent<Image>().color.a == 1.0f)
            {
                SceneManager.LoadScene("Lobby");
            }
        }
    }

    void MenuOpen()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (InGame_Mgr.s_GameState == GameState.GamePlay && Menu_Panel.activeSelf == false)
            {
                Menu_Panel.SetActive(true);
                StatusOpen();
                Time.timeScale = 0.0f;
                InGame_Mgr.s_GameState = GameState.GamePause;
            }
            else if (InGame_Mgr.s_GameState == GameState.GamePause && Menu_Panel.activeSelf == true)
            {
                Menu_Panel.SetActive(false);
                Status_Img.SetActive(false);
                Time.timeScale = 1.0f;
                InGame_Mgr.s_GameState = GameState.GamePlay;
            }
        }
    }

    void SkipBtnFunc()
    {
        if (CharCtrl.Inst.Item_Skip >= 1)
        {
            Time.timeScale = 1.0f;
            s_GameState = GameState.GamePlay;
            LevelUpPanel.SetActive(false);
            Status_Img.SetActive(false);
            CharCtrl.Inst.Item_Skip -= 1;
        }
    }

    void StatusOpen()
    {
        Status_Img.SetActive(true);
        CharCtrl.Inst.StatusRefresh();
    }

    void Timer_Refresh()
    {
        if (s_GameState != GameState.GamePlay)
            return;

        Game_Timer += Time.deltaTime;

        Game_TimerTxt.text = ((int)Game_Timer / 60).ToString() + " : " + ((int)Game_Timer % 60).ToString("D2");
    }

    public void ExpUp(float Exp)
    {
        if (s_GameState == GameState.GameEnd)
            return;

        Cur_Exp += Exp;

        if (Cur_Exp >= Next_Exp)
        {
            Char_Level += 1;
            Cur_Exp -= Next_Exp;
            Next_Exp += 1.5f;
            Char_LevelTxt.text = "LV " + Char_Level.ToString();
            LevelUp();
        }

        Exp_bar.fillAmount = (float)Cur_Exp / (float)Next_Exp;
    }

    void LevelUp()
    {
        if (LevelUpPanel.activeSelf == false)
        {
            Time.timeScale = 0.0f;
            s_GameState = GameState.GamePause;

            LevelUpPanel.SetActive(true);
            StatusOpen();

            Refresh_Txt.text = CharCtrl.Inst.Item_Refresh + "회 남음";
            Skip_Txt.text = CharCtrl.Inst.Item_Skip + "회 남음";
        }

        List<int> RandomNumList = new List<int>();

        if (CrSkNum < MaxInven)
        {
            int currentNumber = Random.Range(0, (int)SkillName.CrCount);

            for (int i = 0; i < 3;)
            {
                if (RandomNumList.Contains(currentNumber) || GlobalValue.m_SkDataList[currentNumber].m_Level == 5)
                {
                    currentNumber = Random.Range(0, (int)SkillName.CrCount);
                }
                else
                {
                    RandomNumList.Add(currentNumber);
                    i++;
                }
            }

            SkillGetBtn_1.GetComponent<LevelUpBtnCtrl>().InitData((SkillName)RandomNumList[0]);
            SkillGetBtn_2.GetComponent<LevelUpBtnCtrl>().InitData((SkillName)RandomNumList[1]);
            SkillGetBtn_3.GetComponent<LevelUpBtnCtrl>().InitData((SkillName)RandomNumList[2]);
        }
        else
        {
            List<int> RandomNumList2 = new List<int>();

            for (int i = 0; i < GlobalValue.m_SkDataList.Count; i++)
            {
                if (0 != GlobalValue.m_SkDataList[i].m_Level && GlobalValue.m_SkDataList[i].m_Level != 5)
                {
                    RandomNumList.Add(i);
                }
            }

            if (3 >= RandomNumList.Count)
            {
                if (RandomNumList.Count == 3)
                {
                    SkillGetBtn_1.GetComponent<LevelUpBtnCtrl>().InitData((SkillName)RandomNumList[0]);
                    SkillGetBtn_2.GetComponent<LevelUpBtnCtrl>().InitData((SkillName)RandomNumList[1]);
                    SkillGetBtn_3.GetComponent<LevelUpBtnCtrl>().InitData((SkillName)RandomNumList[2]);
                }
                else if (RandomNumList.Count == 2)
                {
                    SkillGetBtn_1.GetComponent<LevelUpBtnCtrl>().InitData((SkillName)RandomNumList[0]);
                    SkillGetBtn_2.GetComponent<LevelUpBtnCtrl>().InitData((SkillName)RandomNumList[1]);
                    SkillGetBtn_3.GetComponent<LevelUpBtnCtrl>().DummyData();
                }
                else if (RandomNumList.Count == 1)
                {
                    SkillGetBtn_1.GetComponent<LevelUpBtnCtrl>().InitData((SkillName)RandomNumList[0]);
                    SkillGetBtn_2.GetComponent<LevelUpBtnCtrl>().DummyData();
                    SkillGetBtn_3.GetComponent<LevelUpBtnCtrl>().DummyData();
                }
                else if (RandomNumList.Count == 0)
                {
                    SkillGetBtn_1.GetComponent<LevelUpBtnCtrl>().DummyData();
                    SkillGetBtn_2.GetComponent<LevelUpBtnCtrl>().DummyData();
                    SkillGetBtn_3.GetComponent<LevelUpBtnCtrl>().DummyData();
                }
            }
            else
            {
                for (int i = 0; i < 3;)
                {
                    int currentNumber = Random.Range(0, RandomNumList.Count);

                    if (RandomNumList2.Contains(RandomNumList[currentNumber]))
                    {
                        currentNumber = Random.Range(0, RandomNumList.Count);
                    }
                    else
                    {
                        RandomNumList2.Add(RandomNumList[currentNumber]);
                        i++;
                    }
                }

                SkillGetBtn_1.GetComponent<LevelUpBtnCtrl>().InitData((SkillName)RandomNumList2[0]);
                SkillGetBtn_2.GetComponent<LevelUpBtnCtrl>().InitData((SkillName)RandomNumList2[1]);
                SkillGetBtn_3.GetComponent<LevelUpBtnCtrl>().InitData((SkillName)RandomNumList2[2]);
            }
        }

        for (int i = 0; i < RandomNumList.Count; i++)
        {
            print((SkillName)RandomNumList[i]);
        }
    }

    void SkillGet(SkillName a_CrType)
    {
        for (int i = 0; i < GlobalValue.m_SkDataList.Count; i++)
        {
            if (GlobalValue.m_SkDataList[i].CrType == a_CrType)
            {
                if (GlobalValue.m_SkDataList[i].m_Level == 0)
                {
                    GlobalValue.m_SkDataList[i].m_Level += 1;

                    GameObject a_ItemNode = Instantiate(Item_Node) as GameObject;
                    ItemNodeCtrl a_ItemNodeCtrl = a_ItemNode.GetComponent<ItemNodeCtrl>();
                    a_ItemNodeCtrl.InitData(a_CrType);
                    a_ItemNode.transform.SetParent(Inventory.transform, false);

                    CrSkNum++;

                }
                else
                {
                    GlobalValue.m_SkDataList[i].m_Level += 1;

                    ItemNodeCtrl[] a_ItemNodeCtrl = Inventory.GetComponentsInChildren<ItemNodeCtrl>();

                    for (int j = 0; j < a_ItemNodeCtrl.Length; j++)
                    {
                        if (a_ItemNodeCtrl[j].m_CrType == a_CrType)
                            a_ItemNodeCtrl[j].InitData();

                    }
                }

                if (a_CrType == SkillName.Kabuto || a_CrType == SkillName.Kuuga)
                {
                    CharCtrl.Inst.PasiveOn(a_CrType);
                }
            }
        }

        if (a_CrType == SkillName.CrCount)
        {
            Cur_Gold += 10 + (5 * (int)CharCtrl.Inst.Greed);
            GlobalValue.g_UserGold += 10 + (5 * (int)CharCtrl.Inst.Greed);
            PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);
        }

        Time.timeScale = 1.0f;
        s_GameState = GameState.GamePlay;
        LevelUpPanel.SetActive(false);
        Status_Img.SetActive(false);
        SkillInfo.SetActive(false);
    }

    // --- DamageTxt
    public void DamageTxt(float a_Value, Transform a_OwnerTr, bool IsChar = false)
    {
        GameObject a_DamClone = (GameObject)Instantiate(Damage_Txt_Obj);
        if (a_DamClone != null && Damage_Canvas != null)
        {
            if (IsChar == true)
                a_DamClone.GetComponent<Text>().color = new Color(1.0f, 0.5f, 0.5f);

            Vector3 a_StCacPos
                = new Vector3(a_OwnerTr.position.x,
                              a_OwnerTr.position.y + 5.0f, 1f);
            a_DamClone.GetComponent<Text>().text = a_Value.ToString("N0");
            a_DamClone.transform.SetParent(Damage_Canvas);
            a_DamClone.transform.position = a_StCacPos;
        }
    }

    // --- SpawnPoint 온오프
    //void SpawnPointActive()
    //{
    //    if (s_GameState != GameState.GamePlay)
    //        return;

    //    for (int i = 1; i < PointNumber; i++)
    //    {
    //        if (RefCamCtrl.m_CamWMin.x - 5.0f < points[i].transform.position.x &&
    //            RefCamCtrl.m_CamWMin.y - 5.0f < points[i].transform.position.y)
    //        {
    //            if (points[i].transform.position.x < RefCamCtrl.m_CamWMax.x + 5.0f &&
    //                points[i].transform.position.y < RefCamCtrl.m_CamWMax.y + 5.0f &&
    //                points[i].gameObject.activeSelf == true)
    //            {
    //                points[i].gameObject.SetActive(false);
    //            }
    //            else if ((points[i].transform.position.x > RefCamCtrl.m_CamWMax.x + 5.0f ||
    //                    points[i].transform.position.y > RefCamCtrl.m_CamWMax.y + 5.0f) &&
    //                    points[i].gameObject.activeSelf == false)
    //            {
    //                points[i].gameObject.SetActive(true);
    //            }
    //        }
    //        else if ((RefCamCtrl.m_CamWMin.x - 5.0f > points[i].transform.position.x ||
    //                RefCamCtrl.m_CamWMin.y - 5.0f > points[i].transform.position.y) &&
    //                points[i].gameObject.activeSelf == false)
    //        {
    //            points[i].gameObject.SetActive(true);
    //        }
    //    }
    //}
    // --- SpawnPoint 온오프

    //몬스터 생성 코루틴 함수

    IEnumerator CreateMonster()
    {
        //게임 종료 시까지 무한 루프
        while (!isGameOver)
        {
            //플레이어가 사망했을 때 코루틴을 종료해 다음 루틴을 진행하지 않음
            if (InGame_Mgr.s_GameState == GameState.GameEnd)
                yield break;

            //현재 생성된 몬스터 개수 산출
            int monsterCount = (int)GameObject.FindGameObjectsWithTag("MONSTER").Length;

            if (2.0f >= (InGame_Mgr.Inst.Game_Timer / 60))
            {
                spontime = 1.5f;
            }
            else if (4.0f >= (InGame_Mgr.Inst.Game_Timer / 60))
            {
                spontime = 1.2f;
            }
            else if (6.0f >= (InGame_Mgr.Inst.Game_Timer / 60))
            {
                spontime = 0.9f;
            }
            else if (8.0f >= (InGame_Mgr.Inst.Game_Timer / 60))
            {
                spontime = 0.6f;
            }
            else if (10.0f >= (InGame_Mgr.Inst.Game_Timer / 60))
            {
                spontime = 0.2f;
            }

            yield return new WaitForSeconds(spontime);

            //몬스터의 최대 생성 개수보다 작을 때만 몬스터 생성
            if (monsterCount < maxMonster)
            {
                print("spwan");
                //불규칙적인 위치 산출
                int idx = Random.Range(1, points.Length);

                if (points[idx].gameObject.activeSelf == false)
                    continue;

                int Num = 0;

                //몬스터의 동적 생성
                if (2.0f >= (InGame_Mgr.Inst.Game_Timer / 60))
                {
                    Num = 0;
                }
                else if (4.0f >= (InGame_Mgr.Inst.Game_Timer / 60))
                {
                    Num = Random.Range(0, 2);
                }
                else if (6.0f >= (InGame_Mgr.Inst.Game_Timer / 60))
                {
                    Num = Random.Range(0, 3);
                }

                GameObject monster = Instantiate(monsterPrefabs[Num]);
                monster.transform.position = points[idx].position;
                monster.transform.parent = MonsterSpawnGroup;
            }
            else
            {
                yield return null;
            }

        } //while (!isGameOver)
    } //IEnumerator CreateMonster()

    public void GameOver()
    {
        if (GameOver_Pannel.activeSelf == false)
            GameOver_Pannel.SetActive(true);

        if (GameOverImage_time < 0.4f) //특정 위치에서 원점으로 이동
        {
            GameOverImage.transform.localPosition -= new Vector3(0, 400, 0) * Time.deltaTime;
        }
        else if (GameOverImage_time < 0.5f) // 튕기고
        {
            GameOverImage.transform.localPosition += new Vector3(0, 200, 0) * Time.deltaTime;
        }
        else if (GameOverImage_time < 0.6f) //다시 제자리로
        {
            GameOverImage.transform.localPosition -= new Vector3(0, 200, 0) * Time.deltaTime;
        }
        else if (GameOverImage_time < 0.7f) //튕기고
        {
            GameOverImage.transform.localPosition += new Vector3(0, 100, 0) * Time.deltaTime;
        }
        else if (GameOverImage_time < 0.8f) //다시 제자리
        {
            GameOverImage.transform.localPosition -= new Vector3(0, 50, 0) * Time.deltaTime;
        }
        else
        {
            if (ResultBtn.gameObject.activeSelf == false)
                ResultBtn.gameObject.SetActive(true);

            ResultBtn.GetComponent<CanvasGroup>().alpha += 2 * Time.deltaTime;

            if (ResultBtn.GetComponent<CanvasGroup>().alpha >= 1)
            {
                ResultBtn.GetComponent<Button>().enabled = true;
            }
        }

        GameOverImage_time += Time.deltaTime / 2;
    }

    void Result_Play()
    {
        if (GameOver_Pannel.activeSelf == true)
            GameOver_Pannel.SetActive(false);

        if (temp_Timer < Game_Timer)
        {
            temp_Timer += 0.5f;
            Result_Text.text = ((int)temp_Timer / 60).ToString() + " : " + ((int)temp_Timer % 60).ToString("D2") +
                "\n0" +
                "\n0" +
                "\n0";
        }
        else if (temp_Gold < Cur_Gold)
        {
            if (temp_Timer > Game_Timer)
                temp_Timer = Game_Timer;

            temp_Gold += 0.5f;
            Result_Text.text = ((int)temp_Timer / 60).ToString() + " : " + ((int)temp_Timer % 60).ToString("D2") +
                "\n" + (int)temp_Gold +
                "\n0" +
                "\n0";
        }
        else if (temp_Level < Char_Level)
        {
            if (temp_Gold > Cur_Gold)
                temp_Gold = Cur_Gold;

            temp_Level += 0.5f;
            Result_Text.text = ((int)temp_Timer / 60).ToString() + " : " + ((int)temp_Timer % 60).ToString("D2") +
                "\n" + (int)temp_Gold +
                "\n" + (int)temp_Level +
                "\n0";
        }
        else if (temp_Eneny < Kill_Enemy)
        {
            if (temp_Level > Char_Level)
                temp_Level = Char_Level;

            temp_Eneny += 0.5f;
            Result_Text.text = ((int)temp_Timer / 60).ToString() + " : " + ((int)temp_Timer % 60).ToString("D2") +
                "\n" + (int)temp_Gold +
                "\n" + (int)temp_Level +
                "\n" + (int)temp_Eneny;
        }
        else
        {
            temp_Timer = Game_Timer;
            temp_Gold = Cur_Gold;
            temp_Level = Char_Level;
            temp_Eneny = Kill_Enemy;

            Result_Text.text = ((int)temp_Timer / 60).ToString() + " : " + ((int)temp_Timer % 60).ToString("D2") +
                "\n" + (int)temp_Gold +
                "\n" + (int)temp_Level +
                "\n" + (int)temp_Eneny;
        }
    }

    void LobbyBtnFuc()
    {
        if (temp_Timer != Game_Timer || temp_Eneny != Kill_Enemy)
        {
            temp_Timer = Game_Timer;
            temp_Gold = Cur_Gold;
            temp_Level = Char_Level;
            temp_Eneny = Kill_Enemy;

            Result_Text.text = ((int)temp_Timer / 60).ToString() + " : " + ((int)temp_Timer % 60).ToString("D2") +
                "\n" + temp_Gold +
                "\n" + temp_Level +
                "\n" + temp_Eneny;
        }
        else
        {
            FadePanel.SetActive(true);
        }
    }
}
