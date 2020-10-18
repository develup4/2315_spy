using UnityEngine;
using System.Collections;

/// <summary>
/// Demo for UI
/// </summary>
[AddComponentMenu("2D Toolkit/Demo/tk2dUIDemoController")]
public class tk2dUIDemoController : MonoBehaviour
{
    /// <summary>
    /// Button that will change to next page
    /// </summary>
    public tk2dUIItem nextPage;

    /// <summary>
    /// GameObject containing everything in page 1
    /// </summary>
    public GameObject window1;

    /// <summary>
    /// Button that will change to prev page
    /// </summary>
    public tk2dUIItem prevPage;

    /// <summary>
    /// GameObject containing everything in page 2
    /// </summary>
    public GameObject window2;

    /// <summary>
    /// Used to demo progress bar movement
    /// </summary>
    public tk2dUIProgressBar progressBar;
    private float timeSincePageStart = 0;
    private const float TIME_TO_COMPLETE_PROGRESS_BAR = 2f;
    private float progressBarChaseVelocity = 0.0f;
    public tk2dUIScrollbar slider;

    private GameObject currWindow;

    private bool startOnPage1 = true;

    void Awake()
    {
        // Page1 is already visible.
        if (!startOnPage1)
        {
            GoToPage2();
        }
        // else
        // {
        //     GoToPage2();
        // }
    }

    void OnEnable()
    {
        nextPage.OnClick += GoToPage2;
        prevPage.OnClick += GoToPage1;
    }

    void OnDisable()
    {
        nextPage.OnClick -= GoToPage2;
        prevPage.OnClick -= GoToPage1;
    }


    private void GoToPage1()
    {
        timeSincePageStart = 0;
        HideWindow(window2);
        ShowWindow(window1);
        currWindow = window1;
    }

    private void GoToPage2()
    {
        timeSincePageStart = 0;
        if (currWindow != window2)
        {
            progressBar.Value = 0;
            currWindow = window2;
            StartCoroutine(MoveProgressBar());
        }
        HideWindow(window1);
        ShowWindow(window2);

        
    }

    // Sample tween - use your favourite tween library here.
    IEnumerator coTweenTransformTo( Transform transform, float time, Vector3 toPos, Vector3 toScale, float toRotation) 
    {
        Vector3 fromPos = transform.localPosition;
        Vector3 fromScale = transform.localScale;
        Vector3 euler = transform.localEulerAngles;
        float fromRotation = euler.z;

        for (float t = 0; t < time; t += tk2dUITime.deltaTime) {
            float nt = Mathf.Clamp01( t / time );
            nt = Mathf.Sin(nt * Mathf.PI * 0.5f);

            transform.localPosition = Vector3.Lerp( fromPos, toPos, nt );
            transform.localScale = Vector3.Lerp( fromScale, toScale, nt );
            euler.z = Mathf.Lerp( fromRotation, toRotation, nt );
            transform.localEulerAngles = euler;
            yield return 0;
        }

        euler.z = toRotation;
        transform.localPosition = toPos;
        transform.localScale = toScale;
        transform.localEulerAngles = euler;
    }

    private void ShowWindow(GameObject window)
    {
        Transform t = window.transform;
        t.localPosition = new Vector3(-5, 0, 0);
        t.localScale = Vector3.zero;
        t.localEulerAngles = new Vector3(0, 0, 10);
        StartCoroutine( coTweenTransformTo( t, 0.3f, Vector3.zero, Vector3.one, 0 ) );
    }

    private void HideWindow(GameObject window)
    {
        Transform t = window.transform;
        StartCoroutine( coTweenTransformTo( t, 0.3f, new Vector3(5, 0, 0), Vector3.zero, -10 ) );
    }

    private IEnumerator MoveProgressBar()
    {
        while (currWindow == window2 && progressBar.Value < 1f)
        {
            progressBar.Value = timeSincePageStart/TIME_TO_COMPLETE_PROGRESS_BAR;
            yield return null;
            timeSincePageStart += tk2dUITime.deltaTime;
        }

        while (currWindow == window2) 
        {
            float smoothTime = 0.5f;
            progressBar.Value = Mathf.SmoothDamp( progressBar.Value, slider.Value, ref progressBarChaseVelocity, smoothTime, Mathf.Infinity, tk2dUITime.deltaTime );

            yield return 0;
        }
    }
}
