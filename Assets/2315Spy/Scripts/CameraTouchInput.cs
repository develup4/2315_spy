using UnityEngine;
using System.Collections;

public class CameraTouchInput : MonoBehaviour
{
    // 카메라 모드 상수
    public const int    GAME_MODE               = 0;
    public const int    MAGNIFY_MODE            = 1;
    public const int    REDUCE_MODE             = 2;
    public const int    MINIMAP_MODE            = 3;

    // 카메라 범위 상수
    public const float  CAMERA_RANGE_WIDTH      = 2360.0f;
    public const float  CAMERA_RANGE_HEIGHT     = 1440.0f;

    // Inspector창 편집가능 변수
    public float        fScrollSpeed;
    public float        fMagnifySpeed;
    public float        fMagnifyZoomFactor;                         // 게임모드(확대)시 카메라 ZoomFactor
    public float        fReduceZoomFactor;                          // 미니맵모드(축소)시 카메라 ZoomFactor

    // 멤버 변수
    private float       m_fFirstTouchDistance   = 200.0f;           // 확대 제스처 감지를 위해 처음 두 손가락 거리 저장(최초 기본거리 200으로 잡음)
    private int         m_nCameraMode;

    private Vector2     m_vecTouchStartPosition; 
    private Vector2     m_vecTouchEndPosition;

    public bool IsMinimapMode()
    {
        return !(m_nCameraMode == GAME_MODE);
    }

    public int GetCameraMode()
    {
        return m_nCameraMode;
    }

    void Start()
    {
        m_vecTouchStartPosition = Vector2.zero;
        m_vecTouchEndPosition = Vector2.zero;

        m_fFirstTouchDistance = 0.0f;
        m_nCameraMode = GAME_MODE;
    }

    void Update()
    {
        // 카메라 나가지 않도록 처리
        Vector3 vecCameraPosition = transform.position;
        if (vecCameraPosition.x > CAMERA_RANGE_WIDTH)   vecCameraPosition.x = CAMERA_RANGE_WIDTH;
        if (vecCameraPosition.x < -CAMERA_RANGE_WIDTH)  vecCameraPosition.x = -CAMERA_RANGE_WIDTH;
        if (vecCameraPosition.z > CAMERA_RANGE_HEIGHT)  vecCameraPosition.z = CAMERA_RANGE_HEIGHT;
        if (vecCameraPosition.z < -CAMERA_RANGE_HEIGHT) vecCameraPosition.z = -CAMERA_RANGE_HEIGHT;
        transform.position = vecCameraPosition;

        switch (m_nCameraMode)
        {
            case GAME_MODE:
                {
                    if (Input.touchCount == 1)
                    {
                        
                        CheckScrollGesture();
                    }
                    else if (Input.touchCount == 2)
                    {
                        CheckReduceGesture();
                    }
                    break;
                }
            case MAGNIFY_MODE:
                {
                    MagnifyCamera();
                    break;
                }
            case REDUCE_MODE:
                {
                    ReduceCamera();
                    break;
                }
            case MINIMAP_MODE:
                {
                    if (Input.touchCount == 1)
                    {
                        // 중앙에서 축소되도록
                        transform.position = new Vector3(0.0f, 40.0f, 0.0f);

                        // 해당 좌표를 중점으로 확대
                        transform.position = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                        m_nCameraMode = MAGNIFY_MODE;
                    }
                    else if (Input.touchCount == 2)
                    {
                        // 의미없고 오작동만 일으키는듯
                        // CheckMagnifyGesture();
                    }
                    break;
                }
        }
    }

    void CheckScrollGesture()
    {
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            m_vecTouchStartPosition = Input.GetTouch(0).position;
        }
        if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            m_vecTouchEndPosition = Input.GetTouch(0).position;

            // 스크롤 방향으로 힘을 가함으로써 가속도 처리
            rigidbody.AddForce((m_vecTouchStartPosition.x - m_vecTouchEndPosition.x) * fScrollSpeed, 0.0f, (m_vecTouchStartPosition.y - m_vecTouchEndPosition.y) * fScrollSpeed);
        }
    }

    void CheckMagnifyGesture()
    {
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            m_fFirstTouchDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
        }
        else if (Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            if (Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position) > m_fFirstTouchDistance)
            {
                m_nCameraMode = MAGNIFY_MODE;
            }
        }
    }

    void CheckReduceGesture()
    {
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            m_fFirstTouchDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
        }
        else if (Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            if (Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position) < m_fFirstTouchDistance)
            {
                m_nCameraMode = REDUCE_MODE;

                // UI 표시되지 않도록(개짱깨식)
                GameObject.Find("ReselectButton").transform.Translate(new Vector3(0.0f, 0.0f, -100.0f));
                GameObject.Find("GoMainButton").transform.Translate(new Vector3(0.0f, 0.0f, -100.0f));
            }
        }
    }

    void MagnifyCamera()
    {
        GetComponent<tk2dCamera>().ZoomFactor += fMagnifySpeed;
        fMagnifySpeed += 0.01f;

        if (GetComponent<tk2dCamera>().ZoomFactor > fMagnifyZoomFactor)
        {
            GetComponent<tk2dCamera>().ZoomFactor = fMagnifyZoomFactor;
            m_nCameraMode = GAME_MODE;

            m_vecTouchStartPosition = m_vecTouchEndPosition;
            fMagnifySpeed = 0.05f;

            // UI 표시
            GameObject.Find("ReselectButton").transform.Translate(new Vector3(0.0f, 0.0f, 100.0f));
            GameObject.Find("GoMainButton").transform.Translate(new Vector3(0.0f, 0.0f, 100.0f));

            // 스코어 텍스트 표시되도록
            GameObject.Find("ScoreText").GetComponent<MeshRenderer>().enabled = true;
        }
    }

    void ReduceCamera()
    {
        // 중앙에서 축소되도록
        transform.position = new Vector3(0.0f, 40.0f, 0.0f);

        GetComponent<tk2dCamera>().ZoomFactor -= fMagnifySpeed;
        fMagnifySpeed += 0.01f;

        if (GetComponent<tk2dCamera>().ZoomFactor < fReduceZoomFactor)
        {
            GetComponent<tk2dCamera>().ZoomFactor = fReduceZoomFactor;
            m_nCameraMode = MINIMAP_MODE;

            fMagnifySpeed = 0.05f;

            // 텍스트 표시 안되도록
            GameObject.Find("ScoreText").GetComponent<MeshRenderer>().enabled = false;
        }
    }
}