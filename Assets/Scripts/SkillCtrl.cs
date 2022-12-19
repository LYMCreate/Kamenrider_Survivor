using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCtrl : MonoBehaviour
{
    bool rush = false;
    bool istrigger = false;

    public GameObject target_monster;
    public Vector2 target_Dir;

    MonsterCtrl ref_Mon;

    Vector3 dir;
    float distance;
    float attack_del = 1.0f;

    Vector3 size = new Vector3(20.0f, 20.0f, 20.0f);

    public float Skill_Damage = 0.0f;
    float speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.name.Contains("Den-O"))
        {
            rush = true;
            speed = 2f;
        }
    }

    private void FixedUpdate()
    {
        if (InGame_Mgr.s_GameState != GameState.GamePlay)
            return;

        if (rush == true)
        {
            this.transform.Translate(Vector3.right * 1.5f * speed);
        }
        else if (this.gameObject.name.Contains("Blade"))
        {
            Blade_Rotate();
        }
        else if (this.gameObject.name.Contains("Hibiki"))
        {
            transform.Rotate(new Vector3(0, 0, 200.0f * Time.deltaTime));
        }
        else if (this.gameObject.name.Contains("Kiva"))
        {
            Kiva_batmove();
            attack_del -= Time.deltaTime;
            istrigger = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Blade_Rotate()
    {
        transform.RotateAround(CharCtrl.Inst.transform.position, Vector3.forward, Time.deltaTime * 200.0f);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void Kiva_batmove()
    {
        Skill_Damage = 20 + (GlobalValue.m_SkDataList[8].m_Level * 10);

        dir = CharCtrl.Inst.target_monster.transform.position - this.transform.position;
        distance = Vector3.Distance(CharCtrl.Inst.target_monster.transform.position, this.transform.position);

        if (distance > 30.0f)
        {
            dir = ((CharCtrl.Inst.transform.position + new Vector3(0.0f, 4.0f, 0.0f)) - this.transform.position).normalized;
            distance = Vector3.Distance(CharCtrl.Inst.transform.position, this.transform.position);
        }

        Vector2 a_StepVec = (dir.normalized *
                                Time.deltaTime * 10.0f);

        transform.Translate(a_StepVec, Space.World);
    }

    public void Skill_Event(string Type)
    {
        if (Type == "attack")
        {
            if (this.gameObject.name.Contains("Ryuki"))
            {
                rush = true;
                ObjectDestroy(1.0f);
            }

            if (this.gameObject.name.Contains("Faiz"))
            {
                istrigger = true;
                Invoke("ColliderEnabled", 1.0f);
                ObjectDestroy(4.0f);
                return;
            }

            ColliderEnabled();
        }
        else if (Type == "explosion")
        {
            target_monster.GetComponent<MonsterCtrl>().Lock = false;
        }
        else if (Type == "destroy")
        {
            ObjectDestroy();
        }
    }

    void OnDestroy()
    {
        
    }

    void TakeDamage(float Damage)
    {
        if (ref_Mon != null)
            ref_Mon.TakeDamage(Damage);
    }

    void ColliderEnabled()
    {
        this.gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }

    void ColliderEnabled(bool type)
    {
        this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    void ObjectDestroy()
    {
        Destroy(this.gameObject, 0.1f);
    }

    public void ObjectDestroy(float time)
    {
        Destroy(this.gameObject, time);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "MONSTER" && this.gameObject.name.Contains("Kiva") && istrigger == true)
        {
            if (attack_del <= 0)
            {
                ref_Mon = col.gameObject.GetComponent<MonsterCtrl>();
                TakeDamage(Skill_Damage);
                CharCtrl.Inst.Vampire(Skill_Damage / 3);
                attack_del = 1.0f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player" && istrigger == true)
        {
            col.gameObject.transform.Translate(target_Dir * 20.0f);

            Collider2D[] hitColl = Physics2D.OverlapBoxAll(this.transform.position, size, 0, 1 << 10);

            for (int i = 0; hitColl.Length > i; i++)
            {
                ref_Mon = hitColl[i].gameObject.GetComponent<MonsterCtrl>();
                TakeDamage(Skill_Damage);
            }

            GameObject Explosion = Instantiate(Resources.Load<GameObject>("Prefab/Attack/Faiz_Explosion"));
            Explosion.GetComponent<SkillCtrl>().target_monster = this.target_monster;
            Explosion.transform.position = target_monster.transform.position;

            ObjectDestroy();
        }

        if (col.gameObject.tag == "MONSTER" && istrigger == false)
        {
            ref_Mon = col.gameObject.GetComponent<MonsterCtrl>();
            TakeDamage(Skill_Damage);
        }
    }
}
