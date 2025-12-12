// Scripts/Managers/PotionPooler.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

public class PotionPooler : MonoBehaviour, IGameEventListener<PotionData>, IGameEventListener<GameObject>
{
    #region EventListeners
    [Header("Event Listeners")]
    [SerializeField] private GameEvent<PotionData> PotionSpawnRequestEvent;
    [SerializeField] private GameEvent<GameObject> PotionReturnToPoolEvent;
    #endregion

    #region ObjectPoolVariables
    [Header("Potion pool Settings")]
    [SerializeField] private List<PotionData> allPotionData;
    [SerializeField] private int initialPoolSize = 5;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(10, 1, 10);
    #endregion

    #region Private Variabels
    private Dictionary<string, List<GameObject>> pool;
    private Dictionary<string, GameObject> prefabCache;
    #endregion

    #region Monobehaviour Methods
    private void Awake()
    {
        pool = new Dictionary<string, List<GameObject>>();
        prefabCache = new Dictionary<string, GameObject>();
    }

    private void Start()
    {
        StartCoroutine(PreloadPotions());
    }

    private void OnEnable()
    {
        PotionSpawnRequestEvent.RegisterListener(this); 
        PotionReturnToPoolEvent.RegisterListener(this); 
    }

    private void OnDisable()
    {
        PotionSpawnRequestEvent.UnregisterListener(this);
        PotionReturnToPoolEvent.UnregisterListener(this);
    }
    #endregion

    #region Pool Methods
    public void OnEventRaised(PotionData data)
    {
        SpawnPotionFromPool(data, GetRandomSpawnPosition());
    }

    public void OnEventRaised(GameObject potionInstance)
    {
        // Return to pool
        Potion p = potionInstance.GetComponent<Potion>();
        if (p != null && pool.ContainsKey(p.PotionDataReference.addressableLabel))
        {
            pool[p.PotionDataReference.addressableLabel].Add(potionInstance);
        }
    }

    private IEnumerator PreloadPotions()
    {
        foreach (var data in allPotionData)
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(data.addressableLabel);
            yield return handle;

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                GameObject prefab = handle.Result;
                prefabCache.Add(data.addressableLabel, prefab);
                pool.Add(data.addressableLabel, new List<GameObject>());

                for (int i = 0; i < initialPoolSize; i++)
                {
                    CreateAndPoolPotion(data);
                }
            }
        }
    }

    private void CreateAndPoolPotion(PotionData data, bool spawnImmediately = false)
    {
        GameObject prefab = prefabCache[data.addressableLabel];
        GameObject potionGO = Instantiate(prefab, transform);
        potionGO.SetActive(false);

        Potion p = potionGO.GetComponent<Potion>() ?? potionGO.AddComponent<Potion>();
        p.SetupReferences(PotionReturnToPoolEvent);

        if (spawnImmediately)
        {
            p.Reinitialize(data, GetRandomSpawnPosition());
        }
        else
        {
            pool[data.addressableLabel].Add(potionGO);
        }
    }

    private void SpawnPotionFromPool(PotionData data, Vector3 spawnPos)
    {
        if (pool.ContainsKey(data.addressableLabel) && pool[data.addressableLabel].Count > 0)
        {
            GameObject potion = pool[data.addressableLabel][0];
            pool[data.addressableLabel].RemoveAt(0);
            potion.GetComponent<Potion>().Reinitialize(data, spawnPos);
        }
        else
        {
            CreateAndPoolPotion(data, true);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float z = Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2);
        return new Vector3(x, 0.5f, z) + transform.position;
    }
    #endregion
}
