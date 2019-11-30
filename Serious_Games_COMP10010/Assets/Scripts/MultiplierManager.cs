using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplierManager : MonoBehaviour
{
    public static MultiplierManager instance;

    [SerializeField] private List<MultiplierSegment> multipliers = new List<MultiplierSegment> ();
    [Space]
    [SerializeField] private List<Image> fillImages;
    [SerializeField] private float fillAmountTarget;
    [SerializeField] private float fillAmountDamp = 2.5f;
    [SerializeField] private float timePerMultiplier = 5.0f;
    [SerializeField] private float timeCounter = 0.0f;
    [Space]
    [SerializeField] private float currentSegmentProgress = 0;
    [SerializeField] private int currentSegmentIndex = -1;
    [Space]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private GameObject newMultiplierTextPrefab;
    [SerializeField] private List<Transform> multiplierPositions = new List<Transform> ();
    private Dictionary<Transform, bool> multipliersWithGameObjects = new Dictionary<Transform, bool> ();
    private int lastUsedMultiplierPositionIndex = -1;
    [SerializeField] private UITween parentTween;
    [SerializeField] private UITween flameTween;

    [SerializeField] private Image multiplierFillImage;
    [SerializeField] private Image timeFillImage;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private float highestFill = 0.6875f;
    private float multiplierFillTarget;
    private float timeFillTarget;

    public System.Action<int> OnMultiplierChanged;

    public int GetCurrentMultiplier
    {
        get
        {
            if (currentSegmentIndex == -1) return 1;
            else return (int)multipliers[currentSegmentIndex].multiplier;
        }
    }

    private void Awake ()
    {
        if (instance == null)
            instance = this;
        else if(instance != this)
        {
            Destroy ( this.gameObject );
            return;
        }

        for (int i = 0; i < multiplierPositions.Count; i++)
        {
            multipliersWithGameObjects.Add ( multiplierPositions[i], false );
        }
    }

    private Transform GetEmptyMultiplierPosition (string reason)
    {
        List<Transform> transformsFound = new List<Transform> ();

        for (int i = 0; i < multiplierPositions.Count; i++)
        {
            if (multipliersWithGameObjects[multiplierPositions[i]] == false)
            {
                transformsFound.Add ( multiplierPositions[i] );
            }
            else
            {
                if (multiplierPositions[i].GetComponentInChildren<TextMeshProUGUI> ().text.Contains ( reason ))
                {
                    return null;
                }
            }
        }

        if (transformsFound.Count > 0)
        {
            if(transformsFound.Count > 1)
            {
                if(lastUsedMultiplierPositionIndex >= 0 && transformsFound.Contains ( multiplierPositions[lastUsedMultiplierPositionIndex] ))
                {
                    transformsFound.Remove ( multiplierPositions[lastUsedMultiplierPositionIndex] );
                }
            }

            int randIndex = Random.Range ( 0, transformsFound.Count );
            lastUsedMultiplierPositionIndex = multiplierPositions.IndexOf ( transformsFound[randIndex] );
            return transformsFound[randIndex];
        }

        return null;
    }

    private void Update ()
    {
        MonitorTime ();
        CheckCurrentSegment ();

        if (currentSegmentIndex >= 0)
        {
            float fillX = multipliers[currentSegmentIndex].fillExtents.x;
            float fillY = multipliers[currentSegmentIndex].fillExtents.y;

            if (currentSegmentIndex != multipliers.Count - 1)
                multiplierFillTarget = Mathf.Lerp ( 0, highestFill, Mathf.InverseLerp ( multipliers[currentSegmentIndex].progressRequired, multipliers[currentSegmentIndex + 1].progressRequired, currentSegmentProgress ) );
            else

                multiplierFillTarget = 1.0f;
        }
        else
        {
            multiplierFillTarget = 0.0f;
        }

        multiplierFillImage.fillAmount = Mathf.Lerp ( multiplierFillImage.fillAmount, multiplierFillTarget, Time.deltaTime * fillAmountDamp );

        //for (int i = 0; i < fillImages.Count; i++)
        //{
        //    fillImages[i].fillAmount = Mathf.Lerp ( fillImages[i].fillAmount, fillAmountTarget, Time.deltaTime * fillAmountDamp );
        //}

        //if (currentSegmentIndex >= 0)
        //{
        //    float fillX = multipliers[currentSegmentIndex].fillExtents.x;
        //    float fillY = multipliers[currentSegmentIndex].fillExtents.y;

        //    if (currentSegmentIndex != multipliers.Count - 1)
        //        fillAmountTarget = Mathf.Lerp ( fillX, fillY, Mathf.InverseLerp ( multipliers[currentSegmentIndex].progressRequired, multipliers[currentSegmentIndex + 1].progressRequired, currentSegmentProgress ) );
        //    else
        //        fillAmountTarget = 1.0f;
        //}
        //else
        //{
        //    fillAmountTarget = 0.0f;
        //}

        //for (int i = 0; i < fillImages.Count; i++)
        //{
        //    fillImages[i].fillAmount = Mathf.Lerp ( fillImages[i].fillAmount, fillAmountTarget, Time.deltaTime * fillAmountDamp );
        //}
    }

    public void AddProgress (float amount, string reason)
    {
        currentSegmentProgress += amount;
        currentSegmentProgress = Mathf.Clamp ( currentSegmentProgress, 0.0f, multipliers[multipliers.Count - 1].progressRequired  + 1);

        if (currentSegmentIndex >= 0)
            timeCounter = timePerMultiplier;

        Transform t = GetEmptyMultiplierPosition (reason);
        if (t == null) return;

        GameObject go = Instantiate ( newMultiplierTextPrefab );

        go.GetComponent<SelfDestruct> ().onDestruct += () => { multipliersWithGameObjects[t] = false; };
        multipliersWithGameObjects[t] = true;

        go.transform.SetParent ( t );
        go.GetComponent<RectTransform> ().anchoredPosition3D = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;

        string s = "<color=#F8FF53>Score Multiplier!</color>";
        s += "\n";
        s += "<size=85%>" + reason + "</size>";
        go.GetComponent<TextMeshProUGUI> ().text = s;
    }

    private void MonitorTime ()
    {
        if (timeCounter > 0)
        {
            timeCounter -= Time.deltaTime;

            if (timeCounter < 0)
            {
                if (currentSegmentIndex > 0)
                    currentSegmentProgress = multipliers[currentSegmentIndex - 1].progressRequired;
                else currentSegmentProgress = 0;

                if (currentSegmentIndex >= 0)
                    timeCounter = timePerMultiplier;
            }
        }

        timeFillTarget = Mathf.Lerp ( 0, highestFill, Mathf.InverseLerp ( 0.0f, timePerMultiplier, timeCounter ) );

        if (timeFillImage.fillAmount < timeFillTarget)
            timeFillImage.fillAmount = Mathf.Lerp ( timeFillImage.fillAmount, timeFillTarget, Time.deltaTime * fillAmountDamp * 5.0f );
        else
            timeFillImage.fillAmount = Mathf.Lerp ( timeFillImage.fillAmount, timeFillTarget, Time.deltaTime * fillAmountDamp );

        

        //if (currentSegmentIndex >= 0)
        //{
        //    timeText.text = timeCounter.ToString ( "0.0" ) + " secs";
        //}
        //else
        //{
        //    timeText.text = "";
        //}

    }

    private void FinishMultiplier ()
    {
        currentSegmentProgress = 0.0f;
    }

    private void CheckCurrentSegment ()
    {
        if (currentSegmentIndex + 1 < multipliers.Count)
        {
            if (currentSegmentProgress >= multipliers[currentSegmentIndex + 1].progressRequired)
            {
                IncreaseSegment ();
            }
        }

        if (currentSegmentIndex >= 0)
        {
            float progressTarget = 0.0f;
            progressTarget = multipliers[currentSegmentIndex].progressRequired;

            if (currentSegmentProgress < progressTarget)
            {
                DecreaseSegment ();
            }
        }
    }

    private void IncreaseSegment ()
    {
        currentSegmentIndex++;
        parentTween.FadeIn ( 0.25f );
        multiplierText.text = multipliers[currentSegmentIndex].multiplier.ToString ( "0" ) + "x";

        OnMultiplierChanged?.Invoke ( GetCurrentMultiplier );

        if (currentSegmentIndex >= multipliers.Count - 1) flameTween.FadeIn (0.5f);
    }

    private void DecreaseSegment ()
    {
        currentSegmentIndex--;
        flameTween.FadeOut ( 0.5f );

        OnMultiplierChanged?.Invoke ( GetCurrentMultiplier );

        if (currentSegmentIndex <= -1)
        {
            multiplierText.text = "";
            parentTween.FadeOut ( 0.25f );
        }
        else
        {
            multiplierText.text = multipliers[currentSegmentIndex].multiplier.ToString ( "0" ) + "x";
        }
    }

    [System.Serializable]
    public class MultiplierSegment
    {
        public float multiplier;
        public float progressRequired;
        public Vector2 fillExtents = new Vector2 ( 0.05f, 0.5f );
        public Animator anim;
    }
}
