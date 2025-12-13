using UnityEngine;

public class GameFlowController : MonoBehaviour, IGameEventListener<PotionCollectionData>
{
    #region SerializedFields
    [SerializeField] private float gameDuration;
    [SerializeField] private GameStartedEvent GameStartEvent;
    [SerializeField] private GameEndedEvent GameEndEvent;
    [SerializeField] private ScoreUpdatedEvent ScoreUpdateEvent;
    [SerializeField] private PotionCollectedEvent PotionCollectedEvent;
    [SerializeField] private RequestLeaderboardEvent RequestLeaderboardEvent;
    #endregion

    #region Private Variables
    private int currentScore = 0;
    private string currentSessionId;
    private string sessionStartTime;
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
                EndGame("TimeOut");
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
        if (!isGameActive) return;

        int delta = data.value;
        currentScore += delta;
        ScoreUpdateEvent.Raise(new ScoreUpdateData(currentScore, delta));
    }

    public void StartGame()
    {
        currentSessionId = System.Guid.NewGuid().ToString();
        sessionStartTime = System.DateTime.Now.ToString("o");
        currentScore = 0;
        timer = gameDuration;
        isGameActive = true;

        GameStartEvent.Raise(new SessionData(sessionStartTime, currentSessionId));
        ScoreUpdateEvent.Raise(new ScoreUpdateData(0, 0));
    }

    public void EndGame(string endReason)
    {
        if (!isGameActive) return;

        isGameActive = false;
        string sessionEndTime = System.DateTime.Now.ToString("o");

        GameEndEvent.Raise(new GameEndData(
            sessionEndTime,
            currentScore,
            endReason,
            currentSessionId,
            sessionStartTime
        ));

        RequestLeaderboardEvent.Raise();
    }

    public void RequestLeaderboard()
    {
        RequestLeaderboardEvent.Raise();
    }
    #endregion
}