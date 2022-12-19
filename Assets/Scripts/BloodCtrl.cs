using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodCtrl : MonoBehaviour
{
    float blood_Time = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        blood_Time = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        blood_Time -= Time.deltaTime;

        if (blood_Time <= 0)
        {
            this.gameObject.SetActive(false);
            blood_Time = 0.5f;
        }
    }
}
