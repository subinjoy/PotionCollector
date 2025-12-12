using UnityEngine;

#region PlayerSessionsData
[System.Serializable]
public struct SessionData
{
    public string timestamp;
    public string sessionId;

    public SessionData(string time, string id)
    {
        timestamp = time;
        sessionId = id;
    }
}

[System.Serializable]
public struct GameEndData
{
    public string timestamp;
    public int totalScore;

    public GameEndData(string time, int score)
    {
        timestamp = time;
        totalScore = score;
    }
}

[System.Serializable]
public struct SyncData
{
    public string operationType;
    public bool success;

    public SyncData(string op, bool succ = false)
    {
        operationType = op;
        success = succ;
    }
}
#endregion

#region PotionData
[System.Serializable]
public struct PotionSpawnData
{
    public string potionType;
    public Vector3 position;

    public PotionSpawnData(string type, Vector3 pos)
    {
        potionType = type;
        position = pos;
    }
}

[System.Serializable]
public struct PotionCollectionData
{
    public string potionType;
    public int value;
    public string timestamp;

    public PotionCollectionData(string type, int val, string time)
    {
        potionType = type;
        value = val;
        timestamp = time;
    }
}
#endregion

#region ScoreData
[System.Serializable]
public struct ScoreUpdateData
{
    public int newScore;
    public int scoreDelta;

    public ScoreUpdateData(int newS, int delta)
    {
        newScore = newS;
        scoreDelta = delta;
    }
}

[System.Serializable]
public struct ScoreEntry
{
    public string username;
    public int highScore;

    public ScoreEntry(string name, int score)
    {
        username = name;
        highScore = score;
    }
}
#endregion

