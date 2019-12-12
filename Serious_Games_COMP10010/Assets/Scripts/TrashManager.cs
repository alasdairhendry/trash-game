using TMPro;
using UnityEngine;

public enum TrashType { Bag, Plastic, Paper, Glass, Food, Metal }

public class TrashManager : MonoBehaviour
{
    public static TrashManager instance;

    [NaughtyAttributes.ShowNativeProperty] public int plasticCollected { get; protected set; } = 0;
    [NaughtyAttributes.ShowNativeProperty] public int paperCollected { get; protected set; } = 0;
    [NaughtyAttributes.ShowNativeProperty] public int glassCollected { get; protected set; } = 0;
    [NaughtyAttributes.ShowNativeProperty] public int foodCollected { get; protected set; } = 0;
    [NaughtyAttributes.ShowNativeProperty] public int metalCollected { get; protected set; } = 0;

    [SerializeField] private TextMeshProUGUI plasticText;
    [SerializeField] private TextMeshProUGUI paperText;
    [SerializeField] private TextMeshProUGUI glassText;
    [SerializeField] private TextMeshProUGUI foodText;
    [SerializeField] private TextMeshProUGUI metalText;

    [SerializeField] private GameObject collectableAddedTextPrefab;

    public int plasticCollectedTextTarget;
    public int paperCollectedTextTarget;
    public int glassCollectedTextTarget;
    public int foodCollectedTextTarget;
    public int metalCollectedTextTarget;

    [SerializeField] TrashType type;
    [SerializeField] int amount;
    [SerializeField] private int minTrashToAdd;
    [SerializeField] private int maxTrashToAdd;
    [SerializeField] private float uiDamp = 3.5f;

    private void Awake ()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy ( this.gameObject );
            return;
        }

        TickSystem.Tick += Tick;
    }

    private void Tick ()
    {
        if (TickSystem.Equals ( 4 ))
        {
            if (plasticText.text != plasticCollectedTextTarget.ToString ())
                plasticText.text = plasticCollectedTextTarget.ToString ( "0" );

            if (paperText.text != paperCollectedTextTarget.ToString ())
                paperText.text = paperCollectedTextTarget.ToString ( "0" );

            if (glassText.text != glassCollectedTextTarget.ToString ())
                glassText.text = glassCollectedTextTarget.ToString ( "0" );

            if (foodText.text != foodCollectedTextTarget.ToString ())
                foodText.text = foodCollectedTextTarget.ToString ( "0" );

            if (metalText.text != metalCollectedTextTarget.ToString ())
                metalText.text = metalCollectedTextTarget.ToString ( "0" );
        }
    }

    private void Update ()
    {
        if (plasticCollectedTextTarget != plasticCollected)
            plasticCollectedTextTarget = Mathf.CeilToInt ( Mathf.Lerp ( (float)plasticCollectedTextTarget, (float)plasticCollected, Time.deltaTime * uiDamp ) );

        if (paperCollectedTextTarget != paperCollected)
            paperCollectedTextTarget = Mathf.CeilToInt ( Mathf.Lerp ( (float)paperCollectedTextTarget, (float)paperCollected, Time.deltaTime * uiDamp ) );

        if (glassCollectedTextTarget != glassCollected)
            glassCollectedTextTarget = Mathf.CeilToInt ( Mathf.Lerp ( (float)glassCollectedTextTarget, (float)glassCollected, Time.deltaTime * uiDamp ) );

        if (foodCollectedTextTarget != foodCollected)
            foodCollectedTextTarget = Mathf.CeilToInt ( Mathf.Lerp ( (float)foodCollectedTextTarget, (float)foodCollected, Time.deltaTime * uiDamp ) );

        if (metalCollectedTextTarget != metalCollected)
            metalCollectedTextTarget = Mathf.CeilToInt ( Mathf.Lerp ( (float)metalCollectedTextTarget, (float)metalCollected, Time.deltaTime * uiDamp ) );
    }

    int[] toAdd = new int[5];

    public void AddTrash (TrashType type, int amount = 0)
    {
        if (amount <= 0)
        {
            amount = Random.Range ( minTrashToAdd, maxTrashToAdd );
            amount *= MultiplierManager.instance.GetCurrentMultiplier;
        }

        switch (type)
        {
            case TrashType.Bag:
                toAdd[0] = 0;
                toAdd[1] = 0;
                toAdd[2] = 0;
                toAdd[3] = 0;
                toAdd[4] = 0;

                for (int i = 0; i < amount; i++)
                {
                    int random = Random.Range ( 0, 5 );

                    if (random == 0)
                        toAdd[0]++;
                    else if (random == 1)
                        toAdd[1]++;
                    else if (random == 2)
                        toAdd[2]++;
                    else if (random == 3)
                        toAdd[3]++;
                    else if (random == 4)
                        toAdd[4]++;
                }

                AddTrash ( TrashType.Plastic, toAdd[0] );
                AddTrash ( TrashType.Paper, toAdd[1] );
                AddTrash ( TrashType.Glass, toAdd[2] );
                AddTrash ( TrashType.Food, toAdd[3] );
                AddTrash ( TrashType.Metal, toAdd[4] );
                break;
            case TrashType.Plastic:
                plasticCollected += amount;
                SpawnCollectableAddedText ( plasticText.transform.GetChild (0), amount );
                break;
            case TrashType.Paper:
                paperCollected += amount;
                SpawnCollectableAddedText ( paperText.transform.GetChild (0), amount );
                break;
            case TrashType.Glass:
                glassCollected += amount;
                SpawnCollectableAddedText ( glassText.transform.GetChild (0), amount );
                break;
            case TrashType.Food:
                foodCollected += amount;
                SpawnCollectableAddedText ( foodText.transform.GetChild (0), amount );
                break;
            case TrashType.Metal:
                metalCollected += amount;
                SpawnCollectableAddedText ( metalText.transform.GetChild (0), amount );
                break;
        }
    }

    private void SpawnCollectableAddedText (Transform transform, int amount)
    {
        GameObject go = Instantiate ( collectableAddedTextPrefab );
        go.GetComponentInChildren<TextMeshProUGUI> ().text = "+" + amount.ToString ( "0" );
        go.transform.SetParent ( transform );
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
    }
}
