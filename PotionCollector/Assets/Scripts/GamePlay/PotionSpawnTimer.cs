using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PotionSpawnTimer : MonoBehaviour, IGameEventListener<SessionData>, IGameEventListener<GameEndData>
{
    #region SerialiZed Fields
    [SerializeField] private PotionSpawnRequestEvent PotionSpawnRequestEvent;
    [SerializeField] private GameStartedEvent GameStartEvent;
    [SerializeField] private GameEndedEvent GameEndEvent;
    [SerializeField] private List<PotionData> availablePotions;
    [SerializeField] private float spawnInterval = 3f;
    #endregion

    #region Private Vars
    private Coroutine spawnCoroutine;
    #endregion

    #region Monobehaviour Methods
    private void OnEnable()
    {
        GameStartEvent.RegisterListener(this);
        GameEndEvent.RegisterListener(this);
    }
    private void OnDisable()
    {
        GameStartEvent.UnregisterListener(this);
        GameEndEvent.UnregisterListener(this);
    }
    #endregion

    #region SpawnMethods
    public void OnEventRaised(SessionData data)
    {
        spawnCoroutine = StartCoroutine(SpawnTimerCoroutine());
    }

    public void OnEventRaised(GameEndData data)
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
    }

    IEnumerator SpawnTimerCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            PotionData potionData = availablePotions[Random.Range(0, availablePotions.Count)];
            PotionSpawnRequestEvent.Raise(potionData);
        }
    }
    #endregion
}
