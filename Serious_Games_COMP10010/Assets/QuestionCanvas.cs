using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestionCanvas : MonoBehaviour
{
    public static QuestionCanvas instance;
    [SerializeField] private List<UITween> otherCanvases = new List<UITween> ();
    [SerializeField] private UITween thisCanvas;
    [Space]
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI percentageDisplayText;
    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private GameObject statusCorrectText;
    [SerializeField] private GameObject statusIncorrectText;
    [SerializeField] private GameObject bodyPanel;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private List<Button> answerButtons = new List<Button> ();
    [SerializeField] private List<TextMeshProUGUI> answerTexts = new List<TextMeshProUGUI> ();
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;
    [Space]
    [SerializeField] private List<Question> questions = new List<Question> ();
    private int questionIndex = 0;
    private bool isRetryQuestion = false;

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
        ShuffleQuestions ();
        quitButton.onClick.AddListener ( () => { SceneManager.LoadScene ( 0 ); } );
    }

    [NaughtyAttributes.Button]
    public void ShowQuestion ()
    {
        infoPanel.SetActive ( false );
        bodyPanel.SetActive ( true );

        for (int i = 0; i < otherCanvases.Count; i++)
        {
            otherCanvases[i].FadeOut ( 0.5f, false );
        }

        thisCanvas.FadeIn ( 0.5f, false );

        StartCoroutine ( SetTimeScale ( 0.0f, 2.0f, 0.5f ) );

        if (questionIndex > questions.Count - 1)
            ShuffleQuestions ();

        Question question = questions[questionIndex];

        string header = "";

        if (isRetryQuestion)
            header += "Final Attempt This Task!";
        else
            header += "You have run out of time!";

        header += "\n";

        if (isRetryQuestion)
            header += "<size=50%>Answer the question <size=65%>CORRECTLY</size> to gain ";
        else
            header += "<size=50%>Answer the question to gain ";


        header += (TaskManager.instance.activeTask.timeAllowed / 2.0f).ToString ();
        header += " seconds </size>";
        headerText.text = header;

        questionText.text = "Q. " + question.question;
        continueButton.onClick.RemoveAllListeners ();
        continueButton.onClick.AddListener ( () => { HidePanel (); } );

        List<string> possibleAnswers = new List<string> ()
        {
            question.correctAnswer,
            question.wrongAnswer01,
            question.wrongAnswer02,
            question.wrongAnswer03
        };

        for (int i = 0; i < answerTexts.Count; i++)
        {
            bool isCorrectAnswer = false;
            int randomlyChosenQuestionIndex = UnityEngine.Random.Range ( 0, possibleAnswers.Count );
            answerTexts[i].text = possibleAnswers[randomlyChosenQuestionIndex];

            possibleAnswers.RemoveAt ( randomlyChosenQuestionIndex );

            if (answerTexts[i].text == question.correctAnswer)
                isCorrectAnswer = true;

            answerButtons[i].onClick.RemoveAllListeners ();
            answerButtons[i].onClick.AddListener ( () =>
            {
                if (isCorrectAnswer)
                {
                    OnCorrectAnswerChosen ();
                }
                else
                {
                    OnWrongAnswerChosen ();
                }
            } );
        }
    }

    private void OnCorrectAnswerChosen ()
    {
        SetInfoText ( "Correct" );
        statusCorrectText.SetActive ( true );
        statusIncorrectText.SetActive ( false );
        //questions[questionIndex].AddAnswer ( new Answer ( true, DateTime.UtcNow, questions[questionIndex].answers.Count, Time.time ) );
        MultiplierManager.instance.IncreaseMultiplier ();

        isRetryQuestion = false;
        TaskManager.instance.AddTime ();
        correctAnswers++;
        answerAttempts++;
        percentageDisplayText.text = "Question Statistics";
        percentageText.text = correctAnswers.ToString () + "/" + answerAttempts.ToString () + " correct (" + (((float)correctAnswers / (float)answerAttempts) * 100.0f).ToString ( "0" ) + "%)";
    }

    private void OnWrongAnswerChosen ()
    {
        SetInfoText ( "Incorrect" );
        statusIncorrectText.SetActive ( true );
        statusCorrectText.SetActive ( false );
        //questions[questionIndex].AddAnswer ( new Answer ( false, DateTime.UtcNow, questions[questionIndex].answers.Count, Time.time ) );
        MultiplierManager.instance.DecreaseMultiplier ();

        if(isRetryQuestion)
        {
            // Player failed the retry
            isRetryQuestion = false;
            TaskManager.instance.FailTask ();
        }
        else
        {
            // Player failed first attempt
            isRetryQuestion = true;
            TaskManager.instance.AddTime ();
        }

        answerAttempts++;
        percentageDisplayText.text = "Question Statistics";
        percentageText.text = correctAnswers.ToString () + "/" + answerAttempts.ToString () + " correct (" + (((float)correctAnswers / (float)answerAttempts) * 100.0f).ToString ( "0" ) + "%)";
    }

    private int correctAnswers = 0;
    private int answerAttempts = 0;

    public void HidePanel ()
    {
        Time.timeScale = 1.0f;

        thisCanvas.FadeOut ( 0.5f, false );

        for (int i = 0; i < otherCanvases.Count; i++)
        {
            otherCanvases[i].FadeIn ( 0.5f, false );
        }
    }

    private void SetInfoText (string status)
    {             
        infoText.text = questions[questionIndex].information;

        bodyPanel.SetActive ( false );
        infoPanel.SetActive ( true );

        questionIndex++;
    }

    private void Update ()
    {

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

    private void ShuffleQuestions ()
    {
        questions = Shuffle ( questions );
        questionIndex = 0;
    }

    public List<T> Shuffle<T> (List<T> list)
    {
        System.Random rng = new System.Random ();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next ( n + 1 );
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    [System.Serializable]
    public class Question
    {
        [TextArea] public string question;
        [Space]
        [TextArea(10, 15)] public string information;
        [Space]
        public string correctAnswer;
        public string wrongAnswer01;
        public string wrongAnswer02;
        public string wrongAnswer03;
        public List<Answer> answers { get; protected set; } = new List<Answer> ();

        public void AddAnswer (Answer answer)
        {
            answers.Add ( answer );
        }
    }

    public class Answer
    {
        public bool correctAnswer;
        public DateTime timeAnswer;
        public int timesAnswered;
        public float timePlayed;

        public Answer (bool correctAnswer, DateTime timeAnswer, int timesAnswered, float timePlayed)
        {
            this.correctAnswer = correctAnswer;
            this.timeAnswer = timeAnswer;
            this.timesAnswered = timesAnswered;
            this.timePlayed = timePlayed;
        }
    }
}
