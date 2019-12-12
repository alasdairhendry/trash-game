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

    public void FadeIn (float fadeTime, bool unscaledTime = false)
    {
        StopAllCoroutines ();
        StartCoroutine ( FadeInIE ( fadeTime ) );
    }

    public void FadeOut (float fadeTime, bool unscaledTime = false)
    {
        StopAllCoroutines ();
        StartCoroutine ( FadeOutIE (fadeTime) );
    }

    private IEnumerator FadeInIE(float fadeTime, bool unscaledTime  = false)
    {
        float fadeInterval = (1 - canvasGroup.alpha) / fadeTime;

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += fadeInterval * (unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator FadeOutIE (float fadeTime, bool unscaledTime = false)
    {
        float fadeInterval = canvasGroup.alpha / fadeTime;

        while(canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= fadeInterval * (unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
            yield return null;
        }
    }


}
