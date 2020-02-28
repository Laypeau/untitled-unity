using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkAway : MonoBehaviour
{
    public int waitFrames = 200;
    public int shrinkFrames = 10;

    private Vector3 startingScale;

    public void StartShrinking()
    {
        StartCoroutine(Shrink());
    }

    IEnumerator Shrink()
    {
        for (int i = 0; i < waitFrames; i++)
        {
            yield return null;
        }
        startingScale = transform.localScale; //Parent object should be destroyed by this point, unless something's broke
        for (int i = 0; i < shrinkFrames; i++)
        {
            transform.localScale -= startingScale / shrinkFrames;
            yield return null;
        }
        Destroy(gameObject);
    }
}
