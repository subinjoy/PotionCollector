
using UnityEngine;
using System.Collections;

public class Potion : MonoBehaviour
{
    #region Variables
    private PotionData data;
    private GameEvent<GameObject> potionReturnToPoolEvent;
    private Coroutine despawnCoroutine;
    public PotionData PotionDataReference => data;
    [SerializeField] private PotionCollectedEvent PotionCollectedEvent;
    #endregion

    #region Methods
    public void SetupReferences(GameEvent<GameObject> returnEvent)
    {
        this.potionReturnToPoolEvent = returnEvent;
    }

    public void Reinitialize(PotionData data, Vector3 spawnPosition, float despawnTime = 5f)
    {
        this.data = data;
        transform.position = spawnPosition;
        gameObject.SetActive(true);

        if (despawnCoroutine != null) StopCoroutine(despawnCoroutine);
        despawnCoroutine = StartCoroutine(DespawnTimer(despawnTime));
    }

    private void OnMouseDown()
    {
        Collect();
    }

    private void Collect()
    {
       //Fire Event
        PotionCollectionData payload = new PotionCollectionData(data.potionName, data.potency, System.DateTime.Now.ToString());
        PotionCollectedEvent.Raise(payload);

        if (despawnCoroutine != null) StopCoroutine(despawnCoroutine);
        ReturnToPool();
    }

    private IEnumerator DespawnTimer(float despawnTime)
    {
        yield return new WaitForSeconds(despawnTime);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        potionReturnToPoolEvent.Raise(gameObject);
        gameObject.SetActive(false);
    }
    #endregion
}
