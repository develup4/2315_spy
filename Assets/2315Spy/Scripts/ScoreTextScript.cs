using UnityEngine;
using System.Collections;

public class ScoreTextScript : MonoBehaviour
{
    public void ChangeText(int nPsionicCount, int nEvolutionCount)
    {
        tk2dTextMesh textMesh = GetComponent<tk2dTextMesh>();
        textMesh.text = nPsionicCount.ToString() + " : " + nEvolutionCount.ToString();
        textMesh.Commit();
    }
}