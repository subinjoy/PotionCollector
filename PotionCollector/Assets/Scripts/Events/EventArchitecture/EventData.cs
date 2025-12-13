using UnityEngine;
using System;

#region Firebase and Session Data

[Serializable]
public struct SessionRecord
{
    public string userId;
    public string playerName;
    public int finalScore;
    public string sessionStartTime;
    public string sessionEndTime;

    public SessionRecord(string id, string name, int score, string start, string end)
    {
        userId = id;
        playerName = name;
        finalScore = score;
        sessionStartTime = start;
        sessionEndTime = end;
    }
}

[Serializable]
public struct ScoreEntry
{
    public string playerName;
    public int score;
    public string sessionId;

    public ScoreEntry(string playerName, int score, string sessionId)
    {
        this.playerName = playerName;
        this.score = score;
        this.sessionId = sessionId;
    }
}

#endregion

#region Event Payload Structs

[Serializable]
public struct SessionData
{
    public string sessionStartTime;
    public string sessionId;

    public SessionData(string startTime, string id)
    {
        sessionStartTime = startTime;
        sessionId = id;
    }
}

[Serializable]
public struct GameEndData
{
    public string sessionEndTime;
    public int totalScore;
    public string endReason;
    public string sessionId;
    public string sessionStartTime;

    public GameEndData(string endTime, int score, string reason, string id, string startTime)
    {
        sessionEndTime = endTime;
        totalScore = score;
        endReason = reason;
        sessionId = id;
        sessionStartTime = startTime;
    }
}

[Serializable]
public struct PotionCollectionData
{
    public string potionName;
    public int value;
    public string collectionTimestamp;

    public PotionCollectionData(string name, int val, string timestamp)
    {
        potionName = name;
        value = val;
        collectionTimestamp = timestamp;
    }
}

[Serializable]
public struct PotionSpawnData 
{
    public Vector3 spawnPosition;

    public PotionSpawnData(Vector3 position)
    {
        spawnPosition = position;
    }
}

[Serializable]
public struct ScoreUpdateData
{
    public int newScore;
    public int delta;

    public ScoreUpdateData(int newS, int d)
    {
        newScore = newS;
        delta = d;
    }
}

[Serializable]
public struct SyncData
{
    public string syncType;
    public bool isSuccess;

    public SyncData(string type, bool success = true)
    {
        syncType = type;
        isSuccess = success;
    }
}
#endregion