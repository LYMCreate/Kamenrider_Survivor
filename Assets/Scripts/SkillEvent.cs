using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEvent : MonoBehaviour
{
    SkillCtrl m_ReSkillCS;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
        m_ReSkillCS = transform.parent.GetComponent<SkillCtrl>();
    }

    void Skill_EventSend(string Type)
    {
        m_ReSkillCS.Skill_Event(Type);

        if (this.gameObject.name.Contains("Faiz_1"))
            animator.SetTrigger("attack");
        
        if (Type == "explosion")
            animator.SetTrigger("explosion");
    }

    void Explosion(string Type)
    {
    }
}
