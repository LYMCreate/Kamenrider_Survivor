using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldItemCtrl : MonoBehaviour
{
    Vector3 dir;
    float distance;
    float area = 10.0f;
    bool isMove = false;
    bool isBackMove = false;
    Vector3 vector_temp;

    // Update is called once per frame
    void Update()
    {
        if (InGame_Mgr.s_GameState != GameState.GamePlay)
            return;

        CoinMove();
    }

    void CoinMove()
    {
        dir = (CharCtrl.Inst.transform.position - this.transform.position).normalized;
        distance = Vector3.Distance(CharCtrl.Inst.transform.position, this.transform.position);

        if (distance < area + CharCtrl.Inst.Magnet && isBackMove == false)
        {
            vector_temp = this.gameObject.transform.position;
            isBackMove = true;
        }

        if (isBackMove == true && isMove == false)
        {
            distance = Vector3.Distance(vector_temp, this.transform.position);

            Vector2 a_StepVec = (-dir.normalized *
                                    Time.deltaTime * 20.0f);

            transform.Translate(a_StepVec, Space.World);

            if (distance >= 3.0f)
            {
                isMove = true;
            }
        }

        if (isMove == true)
        {
            Vector2 a_StepVec = (dir.normalized *
                                    Time.deltaTime * 30.0f);

            transform.Translate(a_StepVec, Space.World);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            InGame_Mgr.Inst.Cur_Gold += 10 + (5 * (int)CharCtrl.Inst.Greed);
            GlobalValue.g_UserGold += 10 + (5 * (int)CharCtrl.Inst.Greed);
            PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);
            Destroy(this.gameObject);
        }
    }
}
