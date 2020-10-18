using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {
	
	public string m_kind;
	public string m_type;
	public int m_meleePower;
	public int m_rangePower;
	public float m_meleeAttackSpeed;
	public float m_rangeAttackSpeed;
	public int m_armor;
	public int m_health;
	public float m_speed;
	public float m_meleeRange;
	public float m_meleeTransformRange;
	public float m_rangeRange;
	
	private bool m_order;			// 명령상태
	private string m_state;         // 유닛 상태(정지, 공격, 이동, 죽음)
	private string m_attackState;   // 공격(원거리, 근거리)
	private string m_mode;          // 캐릭터 모드(AI, PLAYER)
	
	void Start(){
		m_mode = "AI";
		m_state = "IDLE";
		m_order = false;
		m_attackState = "NOTTHING";
	}
	
	public void death(){
		m_state = "DEATH";
		m_mode = "AI";
		m_order = false;
		
		List<GameObject> deleteUnitContainer = GameObject.Find("StageManager").GetComponent<StageManager>().getUnitContainer(m_kind);
		deleteUnitContainer.Remove(gameObject);
		Destroy(gameObject);
	}
	
	public string getMode(){
		return m_mode;
	}
	
	public void setMode(string mode){
		m_mode = mode;
	}
	
	public string getState(){
		return m_state;
	}
	
	public void setState(string state){
		m_state = state;
	}
	
	public string getAttackState() {
		return m_attackState;
	}
	
	public void setAttackState(string attackState) {
		m_attackState = attackState;
	}
	
	public bool getOrder(){
		return m_order;
	}
	
	public void setOrder(bool tf){
		m_order = tf;
	}
}
