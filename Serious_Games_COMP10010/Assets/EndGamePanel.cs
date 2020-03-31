using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGamePanel : MonoBehaviour
{
    public static EndGamePanel instance;

    [SerializeField] private List<UITween> otherCanvases = new List<UITween> ();
    [SerializeField] private UITween thisCanvas;

    [SerializeField] private TextMeshProUGUI questions;
    [SerializeField] private TextMeshProUGUI plastic;
    [SerializeField] private TextMeshProUGUI paper;
    [SerializeField] private TextMeshProUGUI glass;
    [SerializeField] private TextMeshProUGUI cans;
    [SerializeField] private TextMeshProUGUI food;

    private void Awake ()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy ( this.gameObject );
            return;
        }

        GetComponent<CanvasGroup> ().alpha = 0;
    }

    public void Show ()
    {
        for (int i = 0; i < otherCanvases.Count; i++)
        {
            otherCanvases[i].FadeOut ( 0.5f, false );
        }

        thisCanvas.FadeIn ( 0.5f, false );

        StartCoroutine ( SetTimeScale ( 0.0f, 2.0f, 0.5f ) );

        questions.text = string.Format ( "You answered {0}/{1} ({2}%) question correctly", QuestionCanvas.instance.correctAnswers, QuestionCanvas.instance.answerAttempts, ((float)QuestionCanvas.instance.correctAnswers / (float)QuestionCanvas.instance.answerAttempts) * 100.0f );
        plastic.text = "<color=black><size=155%>" + TrashManager.instance.plasticCollected.ToString ( "00" ) + "</size></color>" + "\n" + "plastic";
        paper.text = "<color=black><size=155%>" + TrashManager.instance.paperCollected.ToString ( "00" ) + "</size></color>" + "\n" + "paper";
        glass.text = "<color=black><size=155%>" + TrashManager.instance.glassCollected.ToString ( "00" ) + "</size></color>" + "\n" + "glass";
        cans.text = "<color=black><size=155%>" + TrashManager.instance.metalCollected.ToString ( "00" ) + "</size></color>" + "\n" + "metal";
        food.text = "<color=black><size=155%>" + TrashManager.instance.foodCollected.ToString ( "00" ) + "</size></color>" + "\n" + "food";
    }

    public void Quit ()
    {
        Application.Quit ();
    }


    private IEnumerator SetTimeScale (float t, float damp, float delay = 0.0f)
    {
        yield return new WaitForSeconds ( delay );

        float _t = Time.timeScale;

        while (_t != t)
        {
            if (_t < t)
            {
                _t += Time.unscaledDeltaTime * damp;

                if (_t >= t)
                    _t = t;

                Time.timeScale = _t;

                yield return null;
            }
            else
            {
                _t -= Time.unscaledDeltaTime * damp;

                if (_t <= t)
                    _t = t;

                Time.timeScale = _t;

                yield return null;
            }
        }
    }
}
