using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharCtrl : MonoBehaviour
{
    //---------- 키보드 입력값 변수 선언
    float h, v;                 //키보드 입력값을 받기 위한 변수
    float m_MoveSpeed = 10.0f;
    bool Charflip = false;
    //초당 10픽셀을 이동해라 라는 속도 (이동속도)
    Vector3 MoveNextStep;       //보폭을 계산해 주기 위한 변수
    Vector3 MoveHStep;          //가로 이동 보폭 계산용 변수
    Vector3 MoveVStep;          //세로 이동 보폭 계산용 변수
    //---------- 키보드 입력값 변수 선언

    //------------------- LimitMove (주인공 캐릭터가 지형을 벗어나지 못하게 막기)
    Transform m_HeroMeshTr = null;
    CameraCtrl RefCamCtrl = null;
    Vector3 HalfSize = Vector3.zero;

    float a_LmtBdLeft = 0;
    float a_LmtBdTop = 0;
    float a_LmtBdRight = 0;
    float a_LmtBdBottom = 0;

    Vector3 m_CacCurPos = Vector3.zero;
    //------------------- LimitMove (주인공 캐릭터가 지형을 벗어나지 못하게 막기)

    Vector3 m_DirVec;           // 이동하려는 방향 벡터
    Vector3 m_AttackDist;       // 화살표와 캐릭터의 거리 (공격하려는 방향을 구하기 위한 변수)
    Vector3 m_AttackDir;        // 공격하려는 방향 벡터
    Quaternion m_AttackRot;

    SpriteRenderer spriteRenderer;      // 캐릭터 스프라이트 랜더러
    public Animator animator;           // 캐릭터 애니메이터
    public GameObject Arrow;            // 화살표

    float ArrowMoveDelay = 0.01f;

    float AttackDelay = 2.0f;

    public Image Hp_Ber;
    float Char_color = 255.0f;

    public GameObject Damage_Blood;

    // ---- 스킬들
    public GameObject Skill_Slash;
    public GameObject Agito_Attack;
    public GameObject Ryuki_Attack;
    public GameObject Faiz_Attack;
    public GameObject Blade_Attack;
    public GameObject Hibiki_Attack;
    public GameObject Deno_Attack;
    public GameObject Kiva_Attack;
    // ---- 스킬들

    // ---- 스탯들
    float max_hp = 100;         // 최대 체력
    float cur_hp = 100;         // 현재 체력
    float Recovery_hp = 0.1f;      // 체력 회복속도
    float armor = 0;            // 방어력
    float plus_MoveSpeed = 0;   // 추가 이동속도

    float Attack_Point = 0;     // 추가 공격력
    float Attack_Speed = 0;     // 추가 공격속도

    float Luck = 0;             // 행운 (아이템 획득 확률 증가)
    public float Growth = 0;    // 성장 (경험치 획득량 증가)
    public float Magnet = 0;    // 자석 (획득 반경 증가)
    public float Greed = 0;     // 탐욕 (골드 획득량 증가)

    public int Resurrection = 0;       // 추가 부활 횟수
    public int Item_Refresh = 1;       // 아이템 새로고침 횟수
    public int Item_Skip = 1;          // 아이템 스킵 횟수
    // ---- 스탯들

    float Second_1 = 1.0f;

    int blade_card = 0;
    int kiva_bat = 0;
    GameObject kabuto_ps;

    Vector3 effectpos;
    Vector2 target_Dist;
    Vector2 target_Dir;
    float angle;
    float distance;

    float Immune_Time = 0.0f;
    int Immune_Count = 0;

    public GameObject target_monster = null;

    public static CharCtrl Inst;

    private void Awake()
    {
        Inst = this;
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        kabuto_ps = GameObject.Find("CharRoot").transform.Find("Afterimage").gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        RefCamCtrl = FindObjectOfType<CameraCtrl>();

        m_AttackDist = Arrow.transform.position - this.transform.position;
        m_AttackDir = m_AttackDist.normalized;

        SetState();
    }

    // Update is called once per frame
    void Update()
    {
        if (InGame_Mgr.s_GameState != GameState.GamePlay)
            return;

        target_monster = FindNearestObjectByTag();
        KeyBDMove();
        LimitMove();
        HpBar_Refresh();
        ColorChange();
        Recovery();
        Immune();
    }

    void KeyBDMove()   //키보드 이동처리
    {
        //-------------- 가감속 없이 이동 처리 하는 방법
        //화살표키 좌우키를 눌러주면 -1.0f, 0.0f, 1.0f 사이값을 리턴해 준다.
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        //화살표 위쪽 : +1.0f, 화살표 아래쪽 : -1.0f
        //-------------- 가감속 없이 이동 처리 하는 방법


        if (Input.GetAxisRaw("Horizontal") == -1)
        {
            Charflip = true;
            if (kabuto_ps.gameObject.activeSelf == true)
                kabuto_ps.transform.localScale = new Vector3(-4.0f, 4.0f, 4.0f);
        }
        else if(Input.GetAxisRaw("Horizontal") == 1)
        {
            Charflip = false;
            if (kabuto_ps.gameObject.activeSelf == true)
                kabuto_ps.transform.localScale = new Vector3(4.0f, 4.0f, 4.0f);
        }

        if (0.0f != h || 0.0f != v) //키보드 이동처리
        {
            ArrowMove(h, v);

            MoveHStep = Vector3.right * h;
            MoveVStep = Vector3.up * v;

            spriteRenderer.flipX = Charflip;
            animator.SetBool("Run", true);

            MoveNextStep = MoveHStep + MoveVStep;
            m_DirVec = MoveNextStep.normalized;
            MoveNextStep = (m_DirVec * m_MoveSpeed * Time.deltaTime) * (1 + plus_MoveSpeed);
            transform.Translate(MoveNextStep); //월드축 기준으로의 이동
            //transform.position = transform.position + MoveNextStep;
        }
        else
        {
            animator.SetBool("Run", false);
        }

        Active_Attack();

    }//void KeyBDMove()


    void SetState()
    {
        max_hp += (GlobalValue.Helth_Point * 10);
        cur_hp = max_hp;
        armor += (GlobalValue.Armor_Point);
        plus_MoveSpeed += ((float)GlobalValue.MoveSpeed_Point * 0.05f);

        Attack_Point += GlobalValue.Attack_Point;
        Attack_Speed += ((float)GlobalValue.AttackSpeed_Point * 0.05f);

        Luck += GlobalValue.Luck_Point;             // 행운 (아이템 획득 확률 증가)
        Growth += GlobalValue.Growth_Point;    // 성장 (경험치 획득량 증가)
        Magnet += GlobalValue.Magnet_Point;    // 자석 (획득 반경 증가)
        Greed += GlobalValue.Greed_Point;     // 탐욕 (골드 획득량 증가)

        Resurrection += GlobalValue.Resurrection_Point;
        Item_Refresh += GlobalValue.Refresh_Point;
        Item_Skip += GlobalValue.Skip_Point;
    }

    void Immune()
    {
        if (Immune_Count <= 0)
            return;

        Immune_Time -= Time.deltaTime;

        if (Immune_Time <= 0)
        {
            Immune_Count -= 1;
            Immune_Time = 0.3f;
        }

        if (Immune_Count % 2 == 0)
        {
            spriteRenderer.color = new Color32(255, 255, 255, 90);
        }
        else
        {
            spriteRenderer.color = new Color32(255, 255, 255, 180);
        }

        if (Immune_Count == 0)
        {
            spriteRenderer.color = new Color32(255, 255, 255, 255);
        }
    }

    public void TakeDamage(float Damage)
    {
        if (Immune_Count > 0)
            return;

        cur_hp -= Damage - armor;
        Debug.Log("캐릭터 대미지");
        Char_color = 0;
        InGame_Mgr.Inst.DamageTxt(Damage, this.gameObject.transform, true);
        if (Damage_Blood.gameObject.activeSelf == false)
            Damage_Blood.SetActive(true);

        if (cur_hp <= 0)
        {
            if (Resurrection <= 0)
            {
                InGame_Mgr.s_GameState = GameState.GameEnd;
            }
            else if (Resurrection != 0)
            {
                Resurrection -= 1;
                cur_hp = max_hp / 3;
                Immune_Count = 10;
                Immune_Time = 0.3f;
            }
        }
    }

    void ColorChange()
    {
        if (Char_color == 255)
            return;

        Char_color += 4 * Time.deltaTime;

        if (255 <= Char_color)
        {
            Char_color = 255;
        }

        spriteRenderer.color = new Color(255, Char_color, Char_color);
    }

    void Recovery()
    {
        Second_1 -= Time.deltaTime;

        if (Second_1 <= 0.0f)
        {
            cur_hp += Recovery_hp;

            if (cur_hp >= max_hp)
                cur_hp = max_hp;

            Second_1 = 1.0f;
        }
    }

    public void Vampire(float point)
    {
        cur_hp += point;

        if (cur_hp >= max_hp)
            cur_hp = max_hp;
    }

    void HpBar_Refresh()
    {
        Hp_Ber.fillAmount = cur_hp / max_hp;
    }

    void ArrowMove(float h, float v)        // 화살표 이동
    {
        if (-1 < ArrowMoveDelay)
            ArrowMoveDelay -= Time.deltaTime;

        if (ArrowMoveDelay < 0.0f)          // 대각선 방향의 화살표가 조금 더 고정이 잘 되도록
        {
            Arrow.transform.localPosition = (m_DirVec * 5.0f);
            m_AttackDist = Arrow.transform.position - this.transform.position;
            m_AttackDir = m_AttackDist.normalized;
            float angle = Mathf.Atan2(m_AttackDir.y, m_AttackDir.x) * Mathf.Rad2Deg;
            Arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            ArrowMoveDelay = 0.05f;
        }
    }

    void Active_Attack()
    {
        AttackDelay -= Time.deltaTime * (1 + Attack_Speed);

        if (AttackDelay < 0.0f)
        {
            for (int i = 0; i < GlobalValue.m_SkDataList.Count; i++)
            {
                if (GlobalValue.m_SkDataList[i].m_Level != 0)
                {
                    if (GlobalValue.m_SkDataList[i].CrType == SkillName.Decade)
                    {
                        GameObject SkillObj = (GameObject)Instantiate(Skill_Slash);
                        SkillObj.transform.position = this.transform.position;
                        SkillObj.transform.Translate(m_AttackDir * 10.0f);
                        SkillObj.transform.rotation = Arrow.transform.rotation;
                        SkillObj.GetComponent<SkillCtrl>().Skill_Damage = 10 + (GlobalValue.m_SkDataList[i].m_Level * 10);
                    }

                    if (GlobalValue.m_SkDataList[i].CrType == SkillName.Agito)
                    {
                        if (target_Dist == null)
                            return;

                        target_Dist = target_monster.transform.position - this.transform.position;
                        target_Dir = target_Dist.normalized;
                        angle = Mathf.Atan2(target_Dir.y, target_Dir.x) * Mathf.Rad2Deg;

                        GameObject agito_attack = (GameObject)Instantiate(Agito_Attack);
                        agito_attack.transform.position = this.transform.position;
                        agito_attack.transform.Translate(target_Dir * 5.0f);
                        agito_attack.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                        agito_attack.GetComponent<SkillCtrl>().Skill_Damage = 20 + (GlobalValue.m_SkDataList[i].m_Level * 10);
                    }

                    if (GlobalValue.m_SkDataList[i].CrType == SkillName.Ryuki)
                    {
                        GameObject ryuki_attack = (GameObject)Instantiate(Ryuki_Attack);
                        ryuki_attack.transform.position = this.transform.position;
                        ryuki_attack.transform.rotation = Arrow.transform.rotation;
                        ryuki_attack.GetComponent<SkillCtrl>().Skill_Damage = 20 + (GlobalValue.m_SkDataList[i].m_Level * 5);
                    }

                    if (GlobalValue.m_SkDataList[i].CrType == SkillName.Faiz)
                    {
                        if (target_Dist == null)
                            return;

                        distance = Vector3.Distance(target_monster.transform.position, this.transform.position);

                        if (distance <= 30.0f)
                        {
                            target_Dist = target_monster.transform.position - this.transform.position;
                            target_Dir = target_Dist.normalized;
                            angle = Mathf.Atan2(target_Dir.y, target_Dir.x) * Mathf.Rad2Deg;

                            GameObject faiz_attack = (GameObject)Instantiate(Faiz_Attack);
                            faiz_attack.GetComponent<SkillCtrl>().target_monster = target_monster;
                            faiz_attack.GetComponent<SkillCtrl>().target_Dir = target_Dir;
                            target_monster.GetComponent<MonsterCtrl>().Lock = true;

                            faiz_attack.transform.position = target_monster.transform.position;
                            faiz_attack.transform.Translate(-target_Dir * 6.0f);
                            faiz_attack.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                            faiz_attack.GetComponent<SkillCtrl>().Skill_Damage = 20 + (GlobalValue.m_SkDataList[i].m_Level * 15);
                        }

                    }

                    if (GlobalValue.m_SkDataList[i].CrType == SkillName.Blade)
                    {
                        if (GlobalValue.m_SkDataList[i].m_Level != blade_card)
                        {
                            GameObject[] Find_blade = GameObject.FindGameObjectsWithTag("CharAttack");

                            for (int jj = 0; jj < Find_blade.Length; jj++)
                            {
                                if (Find_blade[jj].name.Contains("Blade"))
                                {
                                    Find_blade[jj].GetComponent<SkillCtrl>().ObjectDestroy(0.0f);
                                }

                                blade_card = 0;
                            }

                            for (int jj = 0; jj < GlobalValue.m_SkDataList[i].m_Level; jj++)
                            {
                                GameObject blade_attack = (GameObject)Instantiate(Blade_Attack);

                                float angle = 360.0f / GlobalValue.m_SkDataList[i].m_Level;
                                float newX = Mathf.Sin(jj * angle * Mathf.Deg2Rad);
                                float newY = Mathf.Cos(jj * angle * Mathf.Deg2Rad);
                                newX = (newX * 10.0f) + this.transform.position.x;
                                newY = (newY * 10.0f) + this.transform.position.y;
                                blade_attack.transform.position = new Vector3(newX, newY, 1.0f);
                                blade_attack.transform.parent = this.transform;
                                blade_attack.GetComponent<SkillCtrl>().Skill_Damage = 5 + (GlobalValue.m_SkDataList[i].m_Level * 1);
                                blade_card++;
                            }
                        }
                    }

                    if (GlobalValue.m_SkDataList[i].CrType == SkillName.Hibiki)
                    {
                        GameObject hibiki_attack = (GameObject)Instantiate(Hibiki_Attack);
                        hibiki_attack.transform.position = this.transform.position;
                        hibiki_attack.GetComponent<Rigidbody2D>().AddForce((new Vector3(Random.Range(-0.4f, 0.4f), 0, 0) + transform.up) * 35.0f, ForceMode2D.Impulse);
                        hibiki_attack.GetComponent<SkillCtrl>().Skill_Damage = 10 + (GlobalValue.m_SkDataList[i].m_Level * 10);
                        hibiki_attack.GetComponent<SkillCtrl>().ObjectDestroy(2.2f);
                    }

                    if (GlobalValue.m_SkDataList[i].CrType == SkillName.DenO)
                    {
                        GameObject deno_attack = (GameObject)Instantiate(Deno_Attack);
                        effectpos = this.transform.position;
                        effectpos.x += -50.0f;
                        effectpos.y += Random.Range(-30.0f, 30.0f);
                        deno_attack.transform.position = effectpos;

                        target_Dist = this.transform.position - deno_attack.transform.position;
                        target_Dir = target_Dist.normalized;
                        angle = Mathf.Atan2(target_Dir.y, target_Dir.x) * Mathf.Rad2Deg;

                        deno_attack.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                        deno_attack.GetComponent<SkillCtrl>().Skill_Damage = 10 + (GlobalValue.m_SkDataList[i].m_Level * 10);
                        deno_attack.GetComponent<SkillCtrl>().ObjectDestroy(1.0f);
                    }

                    if (GlobalValue.m_SkDataList[i].CrType == SkillName.Kiva)
                    {
                        if (1 != kiva_bat)
                        {
                            GameObject kiva_attack = (GameObject)Instantiate(Kiva_Attack);
                            kiva_attack.transform.position = this.transform.position + new Vector3(0.0f, 4.0f, 0.0f);
                            kiva_bat++;
                        }
                    }
                }
            }

            AttackDelay = 2.0f;
        }

    }

    public void PasiveOn(SkillName Type)
    {
        if (Type == SkillName.Kuuga)
        {
            max_hp += 10f;
            Recovery_hp += 0.5f;
            armor += 1;
            Attack_Point += 2;
        }
        else if(Type == SkillName.Kabuto)
        {
            if (kabuto_ps.gameObject.activeSelf == false)
            {
                kabuto_ps.gameObject.SetActive(true);
            }
            Attack_Speed += 0.2f;
            plus_MoveSpeed += 0.3f;
        }
    }

    public GameObject FindNearestObjectByTag()
    {
        // 탐색할 오브젝트 목록을 List 로 저장합니다.
        var objects = GameObject.FindGameObjectsWithTag("MONSTER").ToList();

        // LINQ 메소드를 이용해 가장 가까운 적을 찾습니다.
        var neareastObject = objects
            .OrderBy(obj =>
            {
                return Vector3.Distance(transform.position, obj.transform.position);
            })
        .FirstOrDefault();

        return neareastObject;
    }

    void LimitMove()
    {
        if (RefCamCtrl == null)
            return;

        m_CacCurPos = transform.position;
        
        a_LmtBdLeft = RefCamCtrl.m_GroundMin.x + 1.0f + HalfSize.x;
        a_LmtBdTop = RefCamCtrl.m_GroundMin.y + 1.0f + HalfSize.y;
        a_LmtBdRight = RefCamCtrl.m_GroundMax.x - 1.0f - HalfSize.x;
        a_LmtBdBottom = RefCamCtrl.m_GroundMax.y - 1.0f - HalfSize.y;

        if (m_CacCurPos.x < a_LmtBdLeft)
            m_CacCurPos.x = a_LmtBdLeft;

        if (a_LmtBdRight < m_CacCurPos.x)
            m_CacCurPos.x = a_LmtBdRight;

        if (m_CacCurPos.y < a_LmtBdTop)
            m_CacCurPos.y = a_LmtBdTop;

        if (a_LmtBdBottom < m_CacCurPos.y)
            m_CacCurPos.y = a_LmtBdBottom;

        transform.position = m_CacCurPos;
    }

    public void StatusRefresh()
    {
        InGame_Mgr.Inst.Status_Txt.text = max_hp + "\n";
        InGame_Mgr.Inst.Status_Txt.text += Recovery_hp + "\n";
        InGame_Mgr.Inst.Status_Txt.text += armor + "\n";
        InGame_Mgr.Inst.Status_Txt.text += (plus_MoveSpeed * 100) + "%\n";
        InGame_Mgr.Inst.Status_Txt.text += Attack_Point + "\n";
        InGame_Mgr.Inst.Status_Txt.text += (Attack_Speed * 100) + "%\n\n";

        InGame_Mgr.Inst.Status_Txt.text += Luck + "\n";
        InGame_Mgr.Inst.Status_Txt.text += Growth + "\n";
        InGame_Mgr.Inst.Status_Txt.text += Magnet + "\n";
        InGame_Mgr.Inst.Status_Txt.text += Greed + "\n\n";

        InGame_Mgr.Inst.Status_Txt.text += Resurrection + "\n";
        InGame_Mgr.Inst.Status_Txt.text += Item_Refresh + "\n";
        InGame_Mgr.Inst.Status_Txt.text += Item_Skip + "\n";
    }
}
