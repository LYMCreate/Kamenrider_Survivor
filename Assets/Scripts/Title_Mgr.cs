using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title_Mgr : MonoBehaviour
{
    public Text MessageTxt;
    public GameObject Window;
    public GameObject Panel;
    bool Click = false;
    float Timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.LoadData();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Click != true)
        {
            Window.transform.position = Vector3.zero;
            Window.GetComponent<BreakableWindow>().breakWindow();
            Click = true;
            Panel.SetActive(true);
        }

        if (Click == true)
        {
            Timer += Time.deltaTime * 255 / 2;

            if (Timer >= 255)
                Timer = 255;

            Panel.GetComponent<Image>().color = new Color(0, 0, 0, Timer / 255);

            if (Panel.GetComponent<Image>().color.a == 1.0f)
            {
                Invoke("Sceneload", 0.5f);
            }
        }
    }

    void Sceneload()
    {
        SceneManager.LoadScene("Lobby");
    }
}
