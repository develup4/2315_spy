using UnityEngine;
using System.Collections;

public class UnitAnimation : MonoBehaviour {

    // 카메라
    private GameObject MainCamera;
	
	// 캐릭터 각 상태
	public const int IDLE = 1;            // 정지
	public const int ATTACK = 2;          // 공격
	public const int MOVE = 3;            // 이동
	public const int DIE = 4;             // 죽음
	
	private int m_state;
	
    public SkeletonAnimation skeleton;
	
	private int m_animationState;
	
	// Use this for initialization
	void Start () {
        MainCamera = GameObject.Find("MainCamera");

		skeleton = GetComponent<SkeletonAnimation>();
		m_state = IDLE;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // 미니맵모드에서는 그리지 않도록
        GetComponent<MeshRenderer>().enabled = !(MainCamera.GetComponent<CameraTouchInput>().IsMinimapMode());

		UnitAction();
	}
	
	public void ChangeState(int state) {
		m_state = state;
	}
	
	void UnitAction()
    {
		switch(m_state) {
			
		case IDLE :
			IdleUnit();
			break;
			
		case ATTACK :
			AttackUnit();
			break;	
			
		case MOVE :
			MoveUnit();
			break;
			
		case DIE :
			DieUnit();
			break;
		}
	}
	
	void IdleUnit() {
		skeleton.animationName = "idle";
		skeleton.loop = true;
	}
	
	void AttackUnit() {
		skeleton.animationName = "attack";
		skeleton.loop = true;
	}
	
	void MoveUnit() {
		skeleton.animationName = "walk";
		skeleton.loop = true;
	}
	
	void DieUnit() {
		skeleton.animationName = "die";
		skeleton.loop = false;
	}
}
