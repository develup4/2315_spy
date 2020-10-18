using UnityEngine;
using System.Collections;

public class ObserverAnimation : MonoBehaviour{
	public int currentQuadrant;
	private int smallQuadrant, newQuadrant;
	private float randomX, randomZ;
	private float currentTime = 0.0f;
	private float maxDelayTime = 15.0f;
	private string state;
	private int[,] quadrantCoef = new int[4,2];
	private Vector3 destPosition;
	private float totalMoveDistance;
	//private SkeletonAnimation skeleton;
	
	void Start(){
		quadrantCoef[0,0] = 1;  quadrantCoef[0,1] = 1;
		quadrantCoef[1,0] = -1; quadrantCoef[1,1] = 1;
		quadrantCoef[2,0] = -1; quadrantCoef[2,1] = -1;
		quadrantCoef[3,0] = 1;  quadrantCoef[3,1] = -1;
		
		destPosition = destination();
		GetComponent<NavMeshAgent>().destination = destPosition;
		state = "MOVE";
	}
	
	void Update(){
		currentTime += Time.deltaTime;
		
		if(currentTime > totalMoveDistance / GetComponent<NavMeshAgent>().speed){
			GetComponent<NavMeshAgent>().destination = destination();
			currentTime = 0.0f;
		}
		else if(transform.position == destPosition){
			GetComponent<NavMeshAgent>().destination = destination();
		}
		
		GetComponent<NavMeshAgent>().destination = destPosition;
		Quaternion destRotation = Quaternion.LookRotation(destPosition);
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(destPosition), 1.0f);
	}
	
	Vector3 destination(){
		newQuadrant = currentQuadrant;
		switch(currentQuadrant){
			case 1: while(currentQuadrant == newQuadrant)		{ newQuadrant = Random.Range(1, 5); } break;
			case 2: while(currentQuadrant == newQuadrant)		{ newQuadrant = Random.Range(1, 5); } break;
			case 3: while(currentQuadrant == newQuadrant)		{ newQuadrant = Random.Range(1, 5); } break;
			case 4: while(currentQuadrant == newQuadrant)		{ newQuadrant = Random.Range(1, 5); } break;
		}
		smallQuadrant = (int)Random.Range(1, 5);
		
		float tempX = (1125 * (smallQuadrant % 2));
		float tempZ = (750 * (int)(smallQuadrant / 3));
			
		randomX = (int)Random.Range(tempX, tempX + 1125);
		randomZ = (int)Random.Range(tempZ, tempZ + 750);
		destPosition = new Vector3(randomX * quadrantCoef[newQuadrant-1,0], 0.2f, randomZ * quadrantCoef[newQuadrant-1,1]);
		totalMoveDistance = Vector3.Distance(transform.position, destPosition);
		currentQuadrant = newQuadrant;
		
		return destPosition;
	}
	
	public string getObserverState(){
		return state;
	}
	
	public void setObserverState(string newState){
		state = newState;
	}
}
