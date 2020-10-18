using UnityEngine;
using System.Collections;

public class SceneChange : MonoBehaviour {
	public float delayTime = 5.0f;
	public string sceneName = "Title";
	
	float currentTime = 0.0f;
	bool buttonIsClicked = false;
	
	void Awake(){
		//If this scene is "TeamLogo" the scene will be played automatically play from beginning to end
		if(sceneName.Equals("Title") != true){
			GameObject.Find("Panel").transform.animation.Play("FadeIn");
		}
	}
	
	void fadeOut(){
		GameObject.Find("Panel").transform.animation.Play("FadeOut");
		buttonIsClicked = true;
		
		// If this scene name is "StageChoice(FirstSection)" stop playing the animation
		if(GameObject.Find("PressStartButton") != null){
			if(GameObject.Find("PressStartButton").animation.IsPlaying("Blinking") == true){
				GameObject.Find("PressStartButton").animation.Stop();
			}
		}
	}
	
	void Update(){
		if(buttonIsClicked == true || GameObject.Find("TeamLogo") != null){
			currentTime += Time.deltaTime;
			
			if(currentTime > delayTime){
				Application.LoadLevel(sceneName);
			}
		}
	}
}