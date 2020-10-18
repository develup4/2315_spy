using UnityEngine;
using System.Collections;

public class PossessionAnimation : MonoBehaviour
{
    public const long   ANIMATION_SHOW_TIME                 = 7000000;
    private long        m_nPossessionAnimationStartTime     = 0;
    private bool        m_bAnimationShow                    = false;
    
	void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
	    GetComponent<SkeletonAnimation>().animationName = "choice";
        GetComponent<SkeletonAnimation>().loop = true;
	}

    void Update()
    {
        if (m_bAnimationShow && System.DateTime.Now.Ticks - m_nPossessionAnimationStartTime > ANIMATION_SHOW_TIME)
        {
            GetComponent<MeshRenderer>().enabled = false;
            m_bAnimationShow = false;
        }
    }

    public void SpyPossesion()
    {
        GetComponent<MeshRenderer>().enabled = true;
        m_nPossessionAnimationStartTime = System.DateTime.Now.Ticks;
        m_bAnimationShow = true;
    }
}