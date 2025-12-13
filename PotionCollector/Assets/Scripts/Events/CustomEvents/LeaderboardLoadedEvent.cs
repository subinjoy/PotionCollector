using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomEvents/LeaderboardLoaded")]
public class LeaderboardLoadedEvent : GameEvent<List<ScoreEntry>> { }