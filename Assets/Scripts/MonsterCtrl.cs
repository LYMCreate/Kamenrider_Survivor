using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterState
{
    Idle,
    Attack,
    Die
};

public enum MonsterType
{
    enemy1,
    enemy2,
    enemy3,
    Golem,
    Knight,
}

public class MonsterCtrl : MonoBehaviour
{
    public MonsterState monsterState = MonsterState.Idle;
    MonsterState CurState = MonsterState.Idle;
    public MonsterType montype;

    private Transform monsterTr;
    private Transform playerTr;
    private Animator animator;

    public float m_MoveVelocity = 5.0f;

    private bool isDie = false;
    private float hp = 50;
    public bool SkillTaget = false;

    float Mon_color = 255.0f;

    Rigidbody m_Rigid;

    SpriteRenderer spriteRenderer;

    private InGame_Mgr gameMgr;

    float Attack_col = 0;
    float Mon_Damage;

    public bool Lock = false;

    void Awake()
    {
        //몬스터의 Transform 할당
        monsterTr = this.gameObject.GetComponent<Transform>();
        //추적 대상인 Player의 Transform 할당
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        //Animator 컴포넌트 할당
        animator = this.gameObject.GetComponentInChildren<Animator>();

        gameMgr = GameObject.Find("InGame_Mgr").GetComponent<InGame_Mgr>();

        this.m_Rigid = GetComponent<Rigidbody>();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        MonStatSetting();

        monsterState = MonsterState.Attack;
    }

    // Update is called once per frame
    void Update()
    {
        if (InGame_Mgr.s_GameState != GameState.GamePlay)
            return;

        if (0 <= Attack_col)
            Attack_col -= Time.deltaTime;

        if (playerTr.gameObject.activeSelf == false)
            playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        MonActionUpdate();

        ColorChange();
    }

    void MonStatSetting()
    {
        float plusDamage = (InGame_Mgr.Inst.Game_Timer / 30) + (InGame_Mgr.Inst.Char_Level / 5);
        m_MoveVelocity += (InGame_Mgr.Inst.Game_Timer / 60) + (InGame_Mgr.Inst.Char_Level / 10);
        hp += (InGame_Mgr.Inst.Game_Timer / 20) + (InGame_Mgr.Inst.Char_Level * 2);

        if (plusDamage <= 1.0f)
            plusDamage = 0.0f;

        if (montype == MonsterType.enemy1)
        {
            Mon_Damage = 10.0f + (int)plusDamage;
        }
        else if (montype == MonsterType.enemy2)
        {
            Mon_Damage = 15.0f + (int)plusDamage;
            hp *= 1.5f;
        }
        else if (montype == MonsterType.enemy3)
        {
            Mon_Damage = 20.0f + (int)plusDamage;
            hp *= 2.0f;
        }
    }

    void MonActionUpdate()
    {
        if (isDie == true)
            return;

        if (Lock == true)
            return;

        switch (monsterState)
        {
            //idle 상태
            case MonsterState.Idle:
                {
                    if (CurState != monsterState)
                    {
                        animator.StartPlayback();
                        CurState = monsterState;
                    }                    
                }
                break;

            //추적 상태
            case MonsterState.Attack:
                {
                    if (CurState != monsterState)
                    {
                        animator.StopPlayback();
                        CurState = monsterState;
                    }

                    //-------- 이동 구현
                    Vector2 m_MoveDir = Vector2.zero;
                    m_MoveDir = playerTr.position - this.transform.position;

                    Vector2 a_StepVec = (m_MoveDir.normalized *
                                            Time.deltaTime * m_MoveVelocity);
                    transform.Translate(a_StepVec, Space.World);

                    //캐릭터 회전 
                    if (m_MoveDir.x > 0.0f)
                    {
                        spriteRenderer.flipX = true;
                    }
                    else
                    {
                        spriteRenderer.flipX = false;
                    }
                    //캐릭터 회전               
                    //-------- 이동 구현

                }
                break;
        }
    }

    void MonsterDie()
    {
        gameObject.tag = "Untagged";
        gameObject.layer = 8;
        spriteRenderer.sortingOrder = 1;

        isDie = true;
        monsterState = MonsterState.Die;

        animator.StartPlayback();
        GameObject Boom = Instantiate(Resources.Load<GameObject>("Prefab/Effect/Boom"));
        Boom.transform.position = this.transform.position;
        Destroy(Boom, 1.0f);

        InGame_Mgr.Inst.Kill_Enemy += 1;

        Destroy(this.gameObject, 3.0f);
    }

    void ColorChange()
    {
        if (Mon_color == 255)
            return;

        Mon_color += 4 * Time.deltaTime;

        if (255 <= Mon_color)
        {
            Mon_color = 255;
        }

        spriteRenderer.color = new Color(255, Mon_color, Mon_color);
    }

    public void TakeDamage(float Damage)
    {
        if (monsterState != MonsterState.Die)
        {
            hp -= Damage;
            Mon_color = 0;
            InGame_Mgr.Inst.DamageTxt(Damage, this.gameObject.transform);
        }

        if (hp <= 0)
        {
            GameObject ExpItem = Instantiate(Resources.Load<GameObject>("Prefab/Item/ExpItem"));
            ExpItem.transform.position = this.transform.position;
            hp = 0;
            MonsterDie();
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (0 >= Attack_col)
            {
                col.gameObject.GetComponent<CharCtrl>().TakeDamage(Mon_Damage);
                Attack_col = 1.0f;
            }
        }
    }
}
