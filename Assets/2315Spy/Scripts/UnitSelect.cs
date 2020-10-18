using UnityEngine;
using System.Collections;

public class UnitSelect : MonoBehaviour {
	
	private Vector3     vecTouchStartPos;
	
	private GameObject MainCamera;
	private RaycastHit m_hitPoint;
	private bool m_selected;
	
	public float fMinScrollLength;

    // 터치 포인트 객체
    private GameObject m_objTouchPoint;
	
	// Use this for initialization
	void Start () {
		MainCamera          = GameObject.Find("MainCamera");
		fMinScrollLength    = 10.0f;
		
		m_selected = false;
	}
	
	// Update is called once per frame
	void Update ()
    {	
		if(!m_selected)
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
							if(m_hitPoint.transform.tag.Equals("Unit") && m_hitPoint.transform.GetComponent<Unit>().getMode().Equals("AI"))
                            {
                                m_hitPoint.transform.GetComponent<Unit>().setMode("USER");
                                m_hitPoint.transform.GetComponentInChildren<PossessionAnimation>().SpyPossesion();
                                m_selected = true;

                                // 바닥에 선택원 표시
                                m_hitPoint.transform.GetComponentInChildren<UnitSelectCircle>().MeshRendererOn(true);


                                // 유닛이 죽은 뒤 선택된건지 체크를 위해 전달
                                GameObject.Find("StageManager").GetComponent<StageManager>().UnitSelected();

                                // 재선택 버튼 활성화
                                GameObject.Find("ReselectButton").GetComponent<BoxCollider>().enabled = true;
                            }
						}
					}
				}
			}
		}
	}
	
	public void ChangeSelected(bool selected)
    {
		m_selected = selected;
	}
}
