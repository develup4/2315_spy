using UnityEngine;
using System.Collections;

public class MinimapAnimation : MonoBehaviour
{
    private GameObject MainCamera;

	void Start()
    {
        MainCamera = GameObject.Find("MainCamera");
        GetComponent<SkeletonAnimation>().animationName = "map";
        GetComponent<SkeletonAnimation>().loop = true;
    }

	void Update()
    {
        GetComponent<MeshRenderer>().enabled = MainCamera.GetComponent<CameraTouchInput>().IsMinimapMode();
	}
}
