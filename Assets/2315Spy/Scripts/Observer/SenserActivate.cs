using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {
	public float senserRange;
	public float observeFrequency;
	public float readyTime;
	public float sustainmentTime;
	public float finishTime;
	
	private float currentTime = 0.0f;
	
	void Update(){
		currentTime += Time.deltaTime;
		
		if(currentTime > observeFrequency){
			senserActivate();
		}
	}
	
	void senserActivate(){
	}
}
