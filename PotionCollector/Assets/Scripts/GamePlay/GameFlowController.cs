using UnityEngine;

public class GameFlowController : MonoBehaviour, IGameEventListener<PotionCollectionData>
{
    #region SerializedFields
    [SerializeField] private GameStartedEvent GameStartEvent;
    [SerializeField] private GameEndedEvent GameEndEvent;
    [SerializeField] private ScoreUpdatedEvent ScoreUpdateEvent;
    [SerializeField] private PotionCollectedEvent PotionCollectedEvent;
    #endregion

    #region Private Variables
    private int currentScore = 0;
    private string currentSessionId;
    private float timer;
    private bool isGameActive = false;
    #endregion

    #region Monobehaviour Methods
    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (isGameActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                EndGame();
            }
        }
    }
    private void OnEnable()
    {
        PotionCollectedEvent.RegisterListener(this);
    }
    private void OnDisable()
    {
        PotionCollectedEvent.UnregisterListener(this);
    }
    #endregion

    #region Public Methods
    public void OnEventRaised(PotionCollectionData data)
    {
        int delta = data.value;
        currentScore += delta;
        ScoreUpdateEvent.Raise(new ScoreUpdateData(currentScore, delta));
    }

    public void StartGame()
    {
        currentSessionId = System.Guid.NewGuid().ToString();
        currentScore = 0;
        GameStartEvent.Raise(new SessionData(System.DateTime.Now.ToString(), currentSessionId));
        ScoreUpdateEvent.Raise(new ScoreUpdateData(0, 0));
    }

    public void EndGame()
    {
        GameEndEvent.Raise(new GameEndData(System.DateTime.Now.ToString(), currentScore));
    }
    #endregion

}
