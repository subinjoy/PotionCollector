using UnityEngine;
using TMPro;

public class ScoreUIManager : MonoBehaviour,
    IGameEventListener<ScoreUpdateData>,
    IGameEventListener<GameEndData>
{
    #region serialised Fields
    [SerializeField] private ScoreUpdatedEvent ScoreUpdatedEvent;
    [SerializeField] private TMP_Text  scoreText;
    [SerializeField] private TMP_Text GameOverText;
    [SerializeField] private GameEndedEvent GameEndedEvent;
    #endregion

    #region Methods

    private void OnEnable()
    {
        ScoreUpdatedEvent?.RegisterListener(this);
        GameEndedEvent?.RegisterListener(this);
    }

    private void OnDisable()
    {
        ScoreUpdatedEvent?.UnregisterListener(this);
        GameEndedEvent?.UnregisterListener(this);
    }

    public void OnEventRaised(ScoreUpdateData data)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {data.newScore}";
        }
    }

    public void OnEventRaised(GameEndData data)
    {
        if (scoreText != null)
        {
            GameOverText.text = $"SESSION OVER";
        }
    }
    #endregion
}