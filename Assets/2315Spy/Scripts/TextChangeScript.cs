using UnityEngine;
using System;
using System.Collections;

public class TextChangeScript : MonoBehaviour
{
    public const long   WAVE_TEXT_TIME      = 50000000;
    private long        m_nTextShowTime     = 0;

    // 중간발표용(임시)
    private bool        m_bSpyDetected      = false;

    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    void FixedUpdate()
    {
        if (System.DateTime.Now.Ticks - m_nTextShowTime > WAVE_TEXT_TIME)
        {
            GetComponent<MeshRenderer>().enabled = false;

            // 중간발표용(임시)
            if (m_bSpyDetected)
            {
                Application.Quit();
            }
        }
    }

    public void ChangeText(int nWaveCount)
    {
        if (GameObject.Find("MainCamera").GetComponent<CameraTouchInput>().IsMinimapMode())
        {
            return;
        }

        // 시간 측정
        m_nTextShowTime = System.DateTime.Now.Ticks;

        GetComponent<MeshRenderer>().enabled = true;
        tk2dTextMesh textMesh = GetComponent<tk2dTextMesh>();
        textMesh.text = "WAVE" + nWaveCount.ToString();
        textMesh.Commit();
    }

    // 중간발표용 임시 함수
    public void DetectMessage()
    {
        m_nTextShowTime = System.DateTime.Now.Ticks;
        m_bSpyDetected  = true;

        GetComponent<MeshRenderer>().enabled = true;
        tk2dTextMesh textMesh = GetComponent<tk2dTextMesh>();
        textMesh.text = "SPY DETECTED!";
        textMesh.Commit();
    }
}