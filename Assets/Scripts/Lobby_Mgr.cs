using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby_Mgr : MonoBehaviour
{
    public GameObject Panel;
    public Button Startbtn;
    public Button Shopbtn;
    public Button Exitbtn;
    public Text GoldTxt;

    float Timer = 255.0f;
    bool EnterScene = false;
    string movescene = "";

    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.LoadData();
        Panel.SetActive(true);
        Startbtn.onClick.AddListener(()=> Scenename("InGame"));
        Shopbtn.onClick.AddListener(()=> Scenename("Shop"));
        Exitbtn.onClick.AddListener(()=>Application.Quit());
        GoldTxt.text = GlobalValue.g_UserGold.ToString();
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
                Sceneload();
            }
        }
    }

    void Scenename(string a_movescene)
    {
        movescene = a_movescene;
        EnterScene = true;
        Panel.SetActive(true);
    }

    void Sceneload()
    {
        SceneManager.LoadScene(movescene);
    }
}
