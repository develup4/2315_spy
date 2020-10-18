using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitMove : MonoBehaviour{
	private Transform   m_unitAniamition;  // 캐릭터 애니메이션 오브젝트
    private GameObject  MainCamera;
    private Vector3     vecTouchStartPos;
	private Vector3     destPosition;

	private LineRenderer m_lineRenderer;   // 유닛 이동 거리 표시 선
	private RaycastHit m_hitPoint;         // 터치로 선택된 오브젝트 정보
	private float fMinScrollLength;
	
	private GameObject targetUnit;
    private GameObject m_objTouchPoint;
    
    private long m_nTouchPointShowTime = 0;
    private bool m_bTouchPointShow = false;

    public const long TOUCHPOINT_SHOW_TIME = 10000000;
    
	void Start()
    {
		m_unitAniamition    = transform.GetChild(0);
		m_unitAniamition.GetComponent<UnitAnimation>().ChangeState(1);
		
        MainCamera          = GameObject.Find("MainCamera");
        fMinScrollLength    = 10.0f;
		
		m_lineRenderer = GetComponent<LineRenderer>();
		
		m_lineRenderer.SetPosition(0, transform.position);
		m_lineRenderer.SetPosition(1, transform.position);
		
		targetUnit = null;
        m_objTouchPoint = GameObject.Find("TouchPoint");
        m_objTouchPoint.GetComponent<MeshRenderer>().enabled = false;
        m_objTouchPoint.GetComponent<SkeletonAnimation>().animationName = "move_green";
        m_objTouchPoint.GetComponent<SkeletonAnimation>().loop = true;
	}
	
	void Update ()
    {
        // 터치포인트 출력
        //if (Input.touchCount == 1)
        //{
        //    ShowTouchPoint(true);
        //}

        // 터치포인트 지속시간 체크
        if (m_bTouchPointShow && System.DateTime.Now.Ticks - m_nTouchPointShowTime > TOUCHPOINT_SHOW_TIME)
        {
            m_objTouchPoint.GetComponent<MeshRenderer>().enabled = false;
            m_bTouchPointShow = false;
        }

        // Player Mode
        if (gameObject.GetComponent<Unit>().getMode().Equals("USER"))
        {
            if (Input.touchCount == 1 && MainCamera.GetComponent<CameraTouchInput>().IsMinimapMode() == false)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    vecTouchStartPos = Input.GetTouch(0).position;
                }

                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    if (Vector2.Distance(vecTouchStartPos, Input.GetTouch(0).position) < fMinScrollLength)
                    {
                        if (Physics.Raycast(Camera.mainCamera.ScreenPointToRay(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0.0f)), out m_hitPoint, Mathf.Infinity))
                        {
                            // 선택한 유닛 플레이어 컨트롤
                            PlayerUnitMove(m_hitPoint);
                        }
                    }
                }
            }
        }

        // AI Mode
        if (gameObject.GetComponent<Unit>().getMode().Equals("AI"))
        {
            if (!gameObject.GetComponent<Unit>().getState().Equals("ATTACK"))
            {
                targetUnit = searchUnit();
                if (targetUnit != null)
                {
                    GetComponent<Unit>().setState("ATTACK");
                }
            }
        }

        UnitStateManager();
    }

    // 플레이어가 유닛 컨트롤 중
    void PlayerUnitMove(RaycastHit hitPoint)
    {
        // 일단 빨간원 제거
        GameObject.Find("StageManager").GetComponent<StageManager>().AllRedCircleDelete();

        // 땅 선택 했을때
        if (hitPoint.transform.tag.Equals("Ground"))
        {
            destPosition = hitPoint.point;
            GetComponent<NavMeshAgent>().destination = destPosition;
            m_unitAniamition.GetComponent<UnitAnimation>().ChangeState(3);
            targetUnit = null;
            gameObject.GetComponent<Unit>().setState("MOVE");
            gameObject.GetComponent<Unit>().setOrder(true);

            // 터치포인트 출력
            ShowTouchPoint(true);
        }

        // 유닛 선택 했을때
        if (hitPoint.transform.tag.Equals("Unit"))
        {
            // 같은 편
            if (gameObject.GetComponent<Unit>().m_kind.Equals(hitPoint.transform.GetComponent<Unit>().m_kind))
            {
                destPosition = hitPoint.point;
                gameObject.GetComponent<Unit>().setState("MOVE");

                // 터치포인트 출력
                ShowTouchPoint(true);
            }

            // 다른 편
            else
            {
                targetUnit = hitPoint.transform.gameObject;
                destPosition = targetUnit.transform.position;
                gameObject.GetComponent<Unit>().setState("ATTACK");
            }

            GetComponent<NavMeshAgent>().destination = destPosition;
            m_unitAniamition.GetComponent<UnitAnimation>().ChangeState(3);
        }

        // 기타 UI 선택했을때
        if (m_hitPoint.transform.tag.Equals("UI"))
        {
        }

        // 장애물 선택했을때
        if (m_hitPoint.transform.tag.Equals("Obstacle"))
        {
        }
    }

    void UnitStateManager()
    {
        // 타겟이 있으면
        if (targetUnit != null)
        {

            float distanceFromTarget = Vector3.Distance(gameObject.transform.position, targetUnit.transform.position);

            string state = gameObject.GetComponent<Unit>().getState();

            if (state.Equals("ATTACK"))
            {

                // 타겟이 근거리 범위에 들어왔을때 근접공격	
                if (distanceFromTarget <= gameObject.GetComponent<Unit>().m_meleeRange)
                {
                    m_unitAniamition.GetComponent<UnitAnimation>().ChangeState(2);
                    gameObject.GetComponent<Unit>().setAttackState("MELEE_ATTACK");
                    destPosition = targetUnit.transform.position;
                    GetComponent<NavMeshAgent>().Stop();
                }

                // 타겟이 원거리 범위에 들어왔을때 원거리공격
                else if (distanceFromTarget <= gameObject.GetComponent<Unit>().m_rangeRange)
                {
                    m_unitAniamition.GetComponent<UnitAnimation>().ChangeState(2);
                    gameObject.GetComponent<Unit>().setAttackState("RANGE_ATTACK");
                    destPosition = targetUnit.transform.position;
                    GetComponent<NavMeshAgent>().Stop();
                }

                // 타겟이 범위 밖에 있을때 타겟이 범위에 들어올때까지 따라감
                else
                {
                    m_unitAniamition.GetComponent<UnitAnimation>().ChangeState(3);
                    gameObject.GetComponent<Unit>().setAttackState("MOVE_ATTACK");
                    destPosition = targetUnit.transform.position;
                    GetComponent<NavMeshAgent>().destination = destPosition;
                }

                if (GetComponent<Unit>().getMode().Equals("USER"))
                {
                    m_lineRenderer.SetPosition(0, destPosition);
                    m_lineRenderer.SetPosition(1, transform.position);
                    m_lineRenderer.SetWidth(3f, 3f);

                    // 타겟에 빨간원 표시
                    targetUnit.GetComponentInChildren<TargetedCircle>().MeshRendererOn(true);
                }
            }
        }

        // 타겟이 없으면
        else
        {
            if (GetComponent<Unit>().getMode().Equals("AI"))
            {
                m_unitAniamition.GetComponent<UnitAnimation>().ChangeState(1);
                GetComponent<Unit>().setState("IDLE");
            }

            if (GetComponent<Unit>().getMode().Equals("USER"))
            {

                if (GetComponent<Unit>().getState().Equals("MOVE"))
                {
                    m_lineRenderer.SetPosition(0, destPosition);
                    m_lineRenderer.SetPosition(1, transform.position);
                    m_lineRenderer.SetWidth(3f, 3f);
                }

                else
                {
                    m_unitAniamition.GetComponent<UnitAnimation>().ChangeState(1);
                    GetComponent<Unit>().setState("IDLE");

                    destPosition = transform.position;

                    m_lineRenderer.SetPosition(0, destPosition);
                    m_lineRenderer.SetPosition(1, transform.position);
                    m_lineRenderer.SetWidth(3f, 3f);
                }
            }
        }
    }

    GameObject searchUnit()
    {
        GameObject retUnit = null;
        List<GameObject> searchTarget = null;

        searchTarget = GameObject.Find("StageManager").GetComponent<StageManager>().getTargetUnitContainer(gameObject.GetComponent<Unit>().m_kind);

        if (searchTarget.Count > 0)
        {
            retUnit = searchTarget[0];
            float compareDistance = 0;
            float min_distance = Vector3.Distance(transform.position, retUnit.transform.position);

            for (int i = 0; i < searchTarget.Count; i++)
            {
                compareDistance = Vector3.Distance(transform.position, searchTarget[i].transform.position);
                if (min_distance > compareDistance)
                {
                    min_distance = compareDistance;
                    retUnit = searchTarget[i];
                }
            }
        }

        return retUnit;
    }

    public GameObject getTargetUnit()
    {
        return targetUnit;
    }

    public void setTargetUnit(GameObject newTargetUnit)
    {
        targetUnit = newTargetUnit;
    }

    public Transform getUnitAniamition()
    {
        return m_unitAniamition;
    }

    public void setDestPosition(Vector3 destination)
    {
        destPosition = destination;
    }

    private void ShowTouchPoint(bool IsGreen)
    {
        // 터치 포인트 출력
        if (MainCamera.GetComponent<CameraTouchInput>().IsMinimapMode())
        {
            m_objTouchPoint.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Vector3 vecTouchPointPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                vecTouchPointPosition.y = 0.1f;
                m_objTouchPoint.transform.position = vecTouchPointPosition;

                m_objTouchPoint.GetComponent<MeshRenderer>().enabled = true;
                m_objTouchPoint.GetComponent<SkeletonAnimation>().animationName = IsGreen ? "move_green" : "move_red";
                m_objTouchPoint.GetComponent<SkeletonAnimation>().loop = true;

                m_nTouchPointShowTime = System.DateTime.Now.Ticks;
                m_bTouchPointShow = true;
            }
        }
    }
}