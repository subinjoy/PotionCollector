using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using System.Collections.Generic;
using Firebase.Analytics;
using System;

public class FirebaseManager : MonoBehaviour,
    IGameEventListener<SessionData>,
    IGameEventListener<GameEndData>,
    IGameEventListener<PotionCollectionData>,
    IGameEventListener
{
    #region SerializedFields
    [SerializeField] private LeaderboardLoadedEvent LeaderboardLoadedEvent;
    [SerializeField] private FirebaseSyncStartedEvent FirebaseSyncStartedEvent;
    [SerializeField] private FirebaseSyncCompletedEvent FirebaseSyncCompletedEvent;
    [SerializeField] private GameStartedEvent GameStartEvent;
    [SerializeField] private GameEndedEvent GameEndEvent;
    [SerializeField] private PotionCollectedEvent PotionCollectedEvent;
    [SerializeField] private RequestLeaderboardEvent RequestLeaderboardEvent;
    [SerializeField] private string FirebaseDatabaseUrl;
    #endregion

    #region Private Variables
    private DatabaseReference dbReference;
    private FirebaseAuth auth;
    private FirebaseUser user;
    private const string LeaderboardKey = "leaderboard";
    #endregion

    #region Monobehaviour Methods (Event Wiring)
    private void OnEnable()
    {
        GameStartEvent?.RegisterListener(this);
        GameEndEvent?.RegisterListener(this);
        PotionCollectedEvent?.RegisterListener(this);
        RequestLeaderboardEvent?.RegisterListener(this);
    }

    private void OnDisable()
    {
        GameStartEvent?.UnregisterListener(this);
        GameEndEvent?.UnregisterListener(this);
        PotionCollectedEvent?.UnregisterListener(this);
        RequestLeaderboardEvent?.UnregisterListener(this);
    }
    #endregion

    #region Initialization (FIXED: Resolves Stuck Issue and URL)

    private void Start()
    {
        InitializeFirebase();
    }

    private async void InitializeFirebase()
    {
        try
        {
            DependencyStatus dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (dependencyStatus == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                dbReference = FirebaseDatabase.GetInstance(FirebaseDatabaseUrl).RootReference;

                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                Debug.Log("<color=blue>Firebase Services Initialized successfully with explicit URL.</color>");
                LoginAnonymously();
            }
            else
            {
                Debug.LogError($"Could not resolve Firebase dependencies: {dependencyStatus}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("FATAL Firebase Initialization Error: " + e.Message);
        }
    }

    private async void LoginAnonymously()
    {
        if (user != null || auth == null) return;

        try
        {
            var task = auth.SignInAnonymouslyAsync();
            await task;

            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Anonymous Sign-In Failed: " + task.Exception.ToString());
                return;
            }

            user = task.Result.User;
            Debug.Log($"User logged in: {user.UserId}");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Anonymous Sign-In Failed Exception: " + e.Message);
        }
    }
    #endregion

    #region Event Handlers (OnEventRaised)

    public void OnEventRaised(SessionData data)
    {
        LogAnalyticsEvent("game_start", new Dictionary<string, object> {
            { "session_id", data.sessionId }
        });
    }

    public void OnEventRaised(GameEndData data)
    {
        SaveSessionAndScore(data);

        LogAnalyticsEvent("game_end", new Dictionary<string, object> {
            { "final_score", data.totalScore },
            { "end_reason", data.endReason }
        });
    }

    public void OnEventRaised(PotionCollectionData data)
    {
        LogAnalyticsEvent("potion_collected", new Dictionary<string, object> {
            { "potion_name", data.potionName },
            { "score_value", data.value }
        });
    }

    public void OnEventRaised()
    {
        LoadLeaderboard();
    }
    #endregion

    #region Save/Load Logic (COMPLETED AND DEBUGGED)

    private async void SaveSessionAndScore(GameEndData gameData)
    {
        FirebaseSyncStartedEvent.Raise(new SyncData("SAVE_SESSION"));
        Debug.Log("SAVE CHECK 1: SaveSessionAndScore started."); 

        if (user == null || dbReference == null)
        {
            Debug.LogError("SAVE CHECK FAIL: User not logged in or Firebase not ready. Cannot save session.");
            FirebaseSyncCompletedEvent.Raise(new SyncData("SAVE_SESSION", false));
            return;
        }

        SessionRecord sessionRecord = new SessionRecord(
            id: user.UserId,
            name: "AnonUser",
            score: gameData.totalScore,
            start: gameData.sessionStartTime,
            end: gameData.sessionEndTime
        );

        string json = JsonUtility.ToJson(sessionRecord);
        Debug.Log($"SAVE CHECK 2: JSON data generated: {json}"); 

        string uniqueKey = gameData.sessionId;

        try
        {
            await dbReference.Child(LeaderboardKey).Child(uniqueKey).SetRawJsonValueAsync(json);
            FirebaseSyncCompletedEvent.Raise(new SyncData("SAVE_SESSION", true));
            Debug.Log($"<color=green>SAVE CHECK SUCCESS: Session saved successfully with key: {uniqueKey}</color>");
        }
        catch (Exception e)
        {
            Debug.LogError("SAVE CHECK FAIL: Error saving session data: " + e.Message);
            FirebaseSyncCompletedEvent.Raise(new SyncData("SAVE_SESSION", false));
        }
    }

    private async void LoadLeaderboard()
    {
        FirebaseSyncStartedEvent.Raise(new SyncData("LOAD_LEADERBOARD"));

        if (dbReference == null)
        {
            Debug.LogError("Database reference is null. Cannot load leaderboard.");
            FirebaseSyncCompletedEvent.Raise(new SyncData("LOAD_LEADERBOARD", false));
            return;
        }

        try
        {
            var snapshot = await dbReference.Child(LeaderboardKey)
                .OrderByChild(nameof(SessionRecord.finalScore))
                .LimitToLast(5)
                .GetValueAsync();

            List<ScoreEntry> topScores = new List<ScoreEntry>();

            foreach (var childSnapshot in snapshot.Children)
            {
                SessionRecord record = JsonUtility.FromJson<SessionRecord>(childSnapshot.GetRawJsonValue());

                topScores.Add(new ScoreEntry(
                    playerName: record.playerName,
                    score: record.finalScore,
                    sessionId: childSnapshot.Key
                ));
            }

            topScores.Reverse(); 
            LeaderboardLoadedEvent.Raise(topScores);
            FirebaseSyncCompletedEvent.Raise(new SyncData("LOAD_LEADERBOARD", true));
            Debug.Log($"<color=green>Leaderboard loaded successfully: {topScores.Count} entries found.</color>");
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading leaderboard: " + e.Message);
            FirebaseSyncCompletedEvent.Raise(new SyncData("LOAD_LEADERBOARD", false));
        }
    }

    private void LogAnalyticsEvent(string eventName, Dictionary<string, object> parameters)
    {
        List<Parameter> analyticsParams = new List<Parameter>();

        foreach (var kvp in parameters)
        {
            if (kvp.Value is string s) analyticsParams.Add(new Parameter(kvp.Key, s));
            else if (kvp.Value is int i) analyticsParams.Add(new Parameter(kvp.Key, i));
            else if (kvp.Value is long l) analyticsParams.Add(new Parameter(kvp.Key, l));
            else if (kvp.Value is float f) analyticsParams.Add(new Parameter(kvp.Key, f));
            else if (kvp.Value is double d) analyticsParams.Add(new Parameter(kvp.Key, d));
        }

        FirebaseAnalytics.LogEvent(eventName, analyticsParams.ToArray());
    }
    #endregion
}