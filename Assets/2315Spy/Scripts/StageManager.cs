using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    // UnitContainer
    private List<GameObject> m_listPsionicUnitContainer     	= new List<GameObject>();
	private List<GameObject> m_listEvolutonUnitContainer    	= new List<GameObject>();
	private List<GameObject> m_listPsionicObserverContainer 	= new List<GameObject>();
	private List<GameObject> m_listEvolutionObserverContainer 	= new List<GameObject>();

    // PrefabUnit
    public Transform    m_objBerserker;                         // 경장갑
    public Transform    m_objTitan;                             // 중장갑
    public Transform    m_objTempest;                           // 거대
	public Transform	m_objPsionicObserver;

    public Transform    m_objRaptor;                            // 경장갑
    public Transform    m_objViper;                             // 중장갑
    public Transform    m_objLeviathan;                         // 거대
	public Transform	m_objEvolutionObserver;

    public const int    PSIONIC                 = 0;
    public const int    EVOLUTION               = 1;

    public const float  STANDARD_POSITION_X     = 2600.0f;
    public const float  STANDARD_POSITION_Z     = 1500.0f;

    public const long   FIRST_NOT_SELECT_TIME   = 200000000;    // 최초에 유닛 선택안될 경우에 사용
    public const long   DEAD_NOT_SELECT_TIME    = 100000000;    // 유닛이 죽은 후 선택안될 경우에 사용
    public const long   WAVE_TIME_SLICE         = 360000000;

    private bool        m_bSelectedUnitDead     = false;        // 선택한 유닛이 죽었을때 다시 선택될때까지 true
    private bool        m_bUnitFirstSelect      = false;
    private int         m_nWaveUnitCount        = 10;           // 웨이브 유닛 개수
    private int         m_nPositionCount        = 12;           // 웨이브 위치 개수

    private int         m_nSmallUnitCount       = 5;            // 웨이브때 경장갑 유닛 개수
    private int         m_nMediumUnitCount      = 3;            // 웨이브때 중장갑 유닛 개수
    private int         m_nLargeUnitCount       = 2;            // 웨이브때 거대 유닛 개수

    private int         m_nWaveCount            = 6;
    private int         m_nWaveNowCount         = 0;
    private long        m_nStageStartTime       = 0;
    private long        m_nPrevWaveStartTime    = 0;
    private long        m_nSelectedUnitDeadTime = 0;            // 선택된 유닛이 죽었을때 시간

	void Start()
    {
        // 스테이지 시작 시간 체크
        m_nStageStartTime       = System.DateTime.Now.Ticks;
        m_nPrevWaveStartTime    = m_nStageStartTime;

        Wave(PSIONIC);
        Wave(EVOLUTION);
        m_nWaveNowCount++;

        // 웨이브 텍스트 표시
        GameObject.Find("WaveText").GetComponent<TextChangeScript>().ChangeText(m_nWaveNowCount);
		m_listPsionicObserverContainer.Add(Instantiate(m_objPsionicObserver.gameObject, new Vector3(-1125.0f, 0.2f, 1125.0f), Quaternion.identity) as GameObject);
		m_listPsionicObserverContainer.Add(Instantiate(m_objPsionicObserver.gameObject, new Vector3(-1125.0f, 0.2f, 0.0f), Quaternion.identity) as GameObject);
		m_listPsionicObserverContainer.Add(Instantiate(m_objPsionicObserver.gameObject, new Vector3(-1125.0f, 0.2f, -1125.0f), Quaternion.identity) as GameObject);
		
		m_listEvolutionObserverContainer.Add(Instantiate(m_objPsionicObserver.gameObject, new Vector3(1125.0f, 0.2f, 1125.0f), Quaternion.identity) as GameObject);
		m_listEvolutionObserverContainer.Add(Instantiate(m_objPsionicObserver.gameObject, new Vector3(1125.0f, 0.2f, 0.0f), Quaternion.identity) as GameObject);
		m_listEvolutionObserverContainer.Add(Instantiate(m_objPsionicObserver.gameObject, new Vector3(1125.0f, 0.2f, -1125.0f), Quaternion.identity) as GameObject);
	}

    void FixedUpdate()
    {
        // 유닛이 최초 20초동안 선택되지 않았을 경우 랜덤 선택
        if (m_bUnitFirstSelect == false && System.DateTime.Now.Ticks - m_nStageStartTime > FIRST_NOT_SELECT_TIME)
        {
            UnitRandomSelect();
        }

        // 선택한 유닛이 죽었을때 10초동안 선택되지 않을 경우 랜덤 선택
        if (m_bSelectedUnitDead && System.DateTime.Now.Ticks - m_nSelectedUnitDeadTime > DEAD_NOT_SELECT_TIME)
        {
            UnitRandomSelect();
        }

        if (m_nWaveNowCount < m_nWaveCount && System.DateTime.Now.Ticks - m_nPrevWaveStartTime > WAVE_TIME_SLICE)
        {
            Wave(PSIONIC);
            Wave(EVOLUTION);

            m_nPrevWaveStartTime = System.DateTime.Now.Ticks;
            m_nWaveNowCount++;

            // 웨이브 텍스트 표시
            GameObject.Find("WaveText").GetComponent<TextChangeScript>().ChangeText(m_nWaveNowCount);
        }

        // 카운트 표시
        GameObject.Find("ScoreText").GetComponent<ScoreTextScript>().ChangeText(m_listPsionicUnitContainer.Count, m_listEvolutonUnitContainer.Count);

        // 한쪽이 전멸했을때 발각 처리
        if (m_listPsionicUnitContainer.Count == 0 || m_listEvolutonUnitContainer.Count == 0)
        {
            SpyDetect();
        }

        // 진영을 옮기지 않았을때 발각 처리(중간발표때 시연이 어려워지므로 발표 이후 작업)
    }

    private void UnitRandomSelect()
    {
        System.Random random = new System.Random((int)(DateTime.Now.Ticks));
        int nRandomRace = random.Next(0, 2);

        if (nRandomRace == 0)
        {
            int nRandomIndex = random.Next(0, m_listPsionicUnitContainer.Count - 1);
            m_listPsionicUnitContainer[nRandomIndex].GetComponent<Unit>().setMode("USER");
            GameObject.Find("MainCamera").GetComponent<UnitSelect>().ChangeSelected(true);

            // 재선택 버튼 활성화
            GameObject.Find("ReselectButton").GetComponent<BoxCollider>().enabled = true;

            // 카메라 그 유닛으로 이동
            Vector3 vecPosition = m_listPsionicUnitContainer[nRandomIndex].transform.position;
            vecPosition.y = 40.0f;
            GameObject.Find("MainCamera").transform.position = vecPosition;

            // 바닥에 선택원 표시
            m_listPsionicUnitContainer[nRandomIndex].GetComponentInChildren<UnitSelectCircle>().MeshRendererOn(true);
        }
        else 
        {
            int nRandomIndex = random.Next(0, m_listEvolutonUnitContainer.Count - 1);
            m_listEvolutonUnitContainer[nRandomIndex].GetComponent<Unit>().setMode("USER");
            GameObject.Find("MainCamera").GetComponent<UnitSelect>().ChangeSelected(true);

            // 재선택 버튼 활성화
            GameObject.Find("ReselectButton").GetComponent<BoxCollider>().enabled = true;

            // 카메라 그 유닛으로 이동
            Vector3 vecPosition = m_listEvolutonUnitContainer[nRandomIndex].transform.position;
            vecPosition.y = 40.0f;
            GameObject.Find("MainCamera").transform.position = vecPosition;

            // 바닥에 선택원 표시
            m_listEvolutonUnitContainer[nRandomIndex].GetComponentInChildren<UnitSelectCircle>().MeshRendererOn(true);
        }

        // 선택
        GameObject.Find("MainCamera").GetComponent<UnitSelect>().ChangeSelected(true);
        UnitSelected();
    }

    private void Wave(int nRace)
    {
        int[] nUnitPosition = new int[m_nWaveUnitCount];
        bool[] bIsIndexOccupyed = new bool[m_nPositionCount];
        for (int i = 0; i < m_nPositionCount; i++)
        {
            bIsIndexOccupyed[i] = false;
        }

        // nIndexOccupyed에 값 하나씩 넣기(섹션당 3개씩 9개)
        for (int i = 0; i < m_nWaveUnitCount - 1; i++)
        {
            while (true)
            {
                System.Random random = new System.Random((int)(DateTime.Now.Ticks));
                int nRandomIndex = random.Next(0, m_nPositionCount);

                // 일단 빈자리인지 체크
                if (bIsIndexOccupyed[nRandomIndex] == false)
                {
                    // 해당 섹션에 4개가 되지 않는지 체크
                    int nSectionUnitCount = 0;
                    int nSectionStartIndex = nRandomIndex / 4; nSectionStartIndex *= 4;


                    for (int j = 0; j < m_nPositionCount / 3; j++)
                    {
                        if (bIsIndexOccupyed[nSectionStartIndex + j] == true)
                        {
                            nSectionUnitCount++;
                        }
                    }
                    if (nSectionUnitCount < 3)
                    {
                        bIsIndexOccupyed[nRandomIndex] = true;
                        nUnitPosition[i] = nRandomIndex;

                        break;
                    }
                }
            }
        }

        // 마지막 한마리 빈자리 중 랜덤으로 넣기
        while (true)
        {
            System.Random random = new System.Random((int)(DateTime.Now.Ticks));
            int nRandomIndex = random.Next(0, m_nPositionCount);

            // 일단 빈자리면 오케이
            if (bIsIndexOccupyed[nRandomIndex] == false)
            {
                bIsIndexOccupyed[nRandomIndex] = true;
                nUnitPosition[m_nWaveUnitCount - 1] = nRandomIndex;
                break;
            }
        }

        for (int i = 0; i < m_nWaveUnitCount; i++)
        {
            System.Random random = new System.Random((int)(DateTime.Now.Ticks));
            int nRandomPosition = random.Next(0, 250);

            if (nRace == PSIONIC)
            {
                if (i < m_nSmallUnitCount)
                {
                    m_listPsionicUnitContainer.Add(Instantiate(m_objBerserker.gameObject, new Vector3(-STANDARD_POSITION_X, 3.0f, STANDARD_POSITION_Z - nUnitPosition[i] * 250 - nRandomPosition), Quaternion.identity) as GameObject);
                }
                else if (i < m_nSmallUnitCount + m_nMediumUnitCount)
                {
                    m_listPsionicUnitContainer.Add(Instantiate(m_objTitan.gameObject, new Vector3(-STANDARD_POSITION_X, 3.0f, STANDARD_POSITION_Z - nUnitPosition[i] * 250 - nRandomPosition), Quaternion.identity) as GameObject);
                }
                else if (i < m_nSmallUnitCount + m_nMediumUnitCount + m_nLargeUnitCount)
                {
                    m_listPsionicUnitContainer.Add(Instantiate(m_objTempest.gameObject, new Vector3(-STANDARD_POSITION_X, 3.0f, STANDARD_POSITION_Z - nUnitPosition[i] * 250 - nRandomPosition), Quaternion.identity) as GameObject);
                }
            }
            else if (nRace == EVOLUTION)
            {
                if (i < m_nSmallUnitCount)
                {
                    m_listEvolutonUnitContainer.Add(Instantiate(m_objRaptor.gameObject, new Vector3(STANDARD_POSITION_X, 3.0f, STANDARD_POSITION_Z - nUnitPosition[i] * 250 - nRandomPosition), Quaternion.identity) as GameObject);
                }
                else if (i < m_nSmallUnitCount + m_nMediumUnitCount)
                {
                    m_listEvolutonUnitContainer.Add(Instantiate(m_objViper.gameObject, new Vector3(STANDARD_POSITION_X, 3.0f, STANDARD_POSITION_Z - nUnitPosition[i] * 250 - nRandomPosition), Quaternion.identity) as GameObject);
                }
                else if (i < m_nSmallUnitCount + m_nMediumUnitCount + m_nLargeUnitCount)
                {
                    m_listEvolutonUnitContainer.Add(Instantiate(m_objLeviathan.gameObject, new Vector3(STANDARD_POSITION_X, 3.0f, STANDARD_POSITION_Z - nUnitPosition[i] * 250 - nRandomPosition), Quaternion.identity) as GameObject);
                }
            }
        }
    }

    public void SelectedUnitDead()
    {
        m_bSelectedUnitDead     = true;
        m_nSelectedUnitDeadTime = System.DateTime.Now.Ticks;
    }

    public void UnitSelected()
    {
        m_bUnitFirstSelect      = true;
        m_bSelectedUnitDead     = false;
    }
	
	public List<GameObject> getListPsionicUnitContainer()
    {
		return m_listPsionicUnitContainer;
	}
	
	public List<GameObject> getListEvolutonUnitContainer()
    {
		return m_listEvolutonUnitContainer;
	}
	
	public List<GameObject> getUnitContainer(string kind)
    {
		if (kind.Equals("Psionic"))	    { return m_listPsionicUnitContainer;    }
		else 						    { return m_listEvolutonUnitContainer;   }
	}
	
	public List<GameObject> getTargetUnitContainer(string kind)
    {
		if (kind.Equals("Psionic"))     { return m_listEvolutonUnitContainer;   }
		else 						    { return m_listPsionicUnitContainer;    }
	}

    public void OnReselectButtonDown()
    {
        foreach (GameObject obj in m_listPsionicUnitContainer)
        {
            if (obj.GetComponent<Unit>().getMode().Equals("USER"))
            {
                obj.GetComponent<LineRenderer>().SetPosition(0, transform.position);
                obj.GetComponent<LineRenderer>().SetPosition(1, transform.position);

                obj.GetComponent<Unit>().setMode("AI");

                // 바닥에 선택원 제거
                obj.GetComponentInChildren<UnitSelectCircle>().MeshRendererOn(false);
                AllRedCircleDelete();
            }
        }
        foreach (GameObject obj in m_listEvolutonUnitContainer)
        {
            if (obj.GetComponent<Unit>().getMode().Equals("USER"))
            {
                obj.GetComponent<LineRenderer>().SetPosition(0, transform.position);
                obj.GetComponent<LineRenderer>().SetPosition(1, transform.position);

                obj.GetComponent<Unit>().setMode("AI");

                // 바닥에 선택원 제거
                obj.GetComponentInChildren<UnitSelectCircle>().MeshRendererOn(false);
                AllRedCircleDelete();
            }
        }

        // 재선택 버튼 비활성화
        GameObject.Find("ReselectButton").GetComponent<BoxCollider>().enabled = false;

        // 선택 해제
        GameObject.Find("MainCamera").GetComponent<UnitSelect>().ChangeSelected(false);
    }

    public void OnGoMainButtonDown()
    {
        Application.LoadLevel("Scene01");
    }

    public void SpyDetect()
    {
        // 웨이브 텍스트 표시
        GameObject.Find("WaveText").GetComponent<TextChangeScript>().DetectMessage();
    }

    public void AllRedCircleDelete()
    {
        foreach (GameObject obj in m_listPsionicUnitContainer)
        {
            obj.GetComponentInChildren<TargetedCircle>().MeshRendererOn(false);
        }
        foreach (GameObject obj in m_listEvolutonUnitContainer)
        {
            obj.GetComponentInChildren<TargetedCircle>().MeshRendererOn(false);
        }
    }
}