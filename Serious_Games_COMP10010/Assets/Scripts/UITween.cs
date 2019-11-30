using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITween : MonoBehaviour
{
    private bool isTransitioning = false;
    private CanvasGroup canvasGroup;

    private void Awake ()
    {
        canvasGroup = GetComponent<CanvasGroup> ();
    }

    public void FadeIn (float fadeTime)
    {
        StopAllCoroutines ();
        StartCoroutine ( FadeInIE ( fadeTime ) );
    }

    public void FadeOut (float fadeTime)
    {
        StopAllCoroutines ();
        StartCoroutine ( FadeOutIE (fadeTime) );
    }

    private IEnumerator FadeInIE(float fadeTime)
    {
        float fadeInterval = (1 - canvasGroup.alpha) / fadeTime;

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += fadeInterval * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOutIE (float fadeTime)
    {
        float fadeInterval = canvasGroup.alpha / fadeTime;

        while(canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= fadeInterval * Time.deltaTime;
            yield return null;
        }
    }


}
