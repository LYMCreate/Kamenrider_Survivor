using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    //-------------- 카메라가 주인공을 따라다니게 하기 위한 변수
    GameObject m_HeroObj = null;
    Vector3 newPosition = Vector3.zero;
    private float smoothTime = 0.2f;
    private float xVelocity = 0.0f;
    private float yVelocity = 0.0f;
    //-------------- 카메라가 주인공을 따라다니게 하기 위한 변수

    //------------------- LimitMoveCam(카메라가 지형 밖으로 나갈 수 없도록 막기)
    [HideInInspector] public Vector3 m_GroundMin = Vector3.zero;
    [HideInInspector] public Vector3 m_GroundMax = Vector3.zero;

    [HideInInspector] public Vector3 m_CamWMin = Vector3.zero;
    [HideInInspector] public Vector3 m_CamWMax = Vector3.zero;
    Vector3 m_ScWdHalf = Vector3.zero;

    float a_LmtBdLeft = 0;
    float a_LmtBdTop = 0;
    float a_LmtBdRight = 0;
    float a_LmtBdBottom = 0;
    //------------------- LimitMoveCam(카메라가 지형 밖으로 나갈 수 없도록 막기)

    Camera RefCam = null;

    // Start is called before the first frame update
    void Start()
    {
        m_HeroObj = GameObject.Find("CharRoot");

        RefCam = this.GetComponent<Camera>();

        //1, 지형의 사이즈
        GameObject a_GroundObj = GameObject.Find("Field");
        SpriteRenderer a_GroundSprite = a_GroundObj.GetComponent<SpriteRenderer>();

        Vector3 a_GrdHalfSize = Vector3.zero;
        a_GrdHalfSize.x = (a_GroundObj.transform.localScale.x * a_GroundSprite.size.x) / 2.0f;
        a_GrdHalfSize.y = (a_GroundObj.transform.localScale.y * a_GroundSprite.size.y) / 2.0f;

        //--좌측하단 (전체 지형의 꼭지점 구하기)
        m_GroundMin.x = (a_GroundObj.transform.position.x * a_GroundSprite.size.x) - a_GrdHalfSize.x;
        m_GroundMin.y = (a_GroundObj.transform.position.y * a_GroundSprite.size.y) - a_GrdHalfSize.y;

        //--우측상단 (전체 지형의 꼭지점 구하기)
        m_GroundMax.x = (a_GroundObj.transform.position.x * a_GroundSprite.size.x) + a_GrdHalfSize.x;
        m_GroundMax.y = (a_GroundObj.transform.position.y * a_GroundSprite.size.y) + a_GrdHalfSize.y;
    }

    // Update is called once per frame
    void Update()
    {
        //카메라 화면 좌측하단 코너의 월드 좌표
        m_CamWMin = Camera.main.ViewportToWorldPoint(Vector3.zero);
        //MinX : m_CamWMin.x,  MinZ : m_CamWMin.z

        //카메라 화면 우측상단 코너의 월드 좌표
        m_CamWMax = Camera.main.ViewportToWorldPoint(Vector3.one);
        //MaxX : m_CamWMax.x,  MaxZ : m_CamWMax.z  
    }

    void LateUpdate()
    {
        if (m_HeroObj == null)
            return;

        //this.transform.position =
        //                new Vector3(m_HeroObj.transform.position.x,
        //                            this.transform.position.y,
        //                            m_HeroObj.transform.position.z);

        newPosition = transform.position;
        newPosition.x = Mathf.SmoothDamp(transform.position.x,
                    m_HeroObj.transform.position.x, ref xVelocity, smoothTime);
        newPosition.y = Mathf.SmoothDamp(transform.position.y,
                    m_HeroObj.transform.position.y, ref yVelocity, smoothTime);

        //------------------- LimitMoveCam(카메라가 지형 밖으로 나갈 수 없도록 막기)
        //카메라 화면 좌측하단 코너의 월드 좌표
        m_CamWMin = Camera.main.ViewportToWorldPoint(Vector3.zero);
        //MinX : m_CamWMin.x,  MinZ : m_CamWMin.z

        //카메라 화면 우측상단 코너의 월드 좌표
        m_CamWMax = Camera.main.ViewportToWorldPoint(Vector3.one);
        //MaxX : m_CamWMax.x,  MaxZ : m_CamWMax.z 

        m_ScWdHalf.x = (m_CamWMax.x - m_CamWMin.x) / 2.0f;
        m_ScWdHalf.y = (m_CamWMax.y - m_CamWMin.y) / 2.0f;

        a_LmtBdLeft = m_GroundMin.x + m_ScWdHalf.x;
        a_LmtBdTop = m_GroundMin.y + m_ScWdHalf.y;
        a_LmtBdRight = m_GroundMax.x - m_ScWdHalf.x;
        a_LmtBdBottom = m_GroundMax.y - m_ScWdHalf.y;

        if (newPosition.x < a_LmtBdLeft)
            newPosition.x = a_LmtBdLeft;

        if (a_LmtBdRight < newPosition.x)
            newPosition.x = a_LmtBdRight;

        if (newPosition.y < a_LmtBdTop)
            newPosition.y = a_LmtBdTop;

        if (a_LmtBdBottom < newPosition.y)
            newPosition.y = a_LmtBdBottom;
        //------------------- LimitMoveCam(카메라가 지형 밖으로 나갈 수 없도록 막기)

        transform.position = newPosition;
    }//void LateUpdate()
}