using MadLab;
using UnityEngine;
using UnityAtoms.BaseAtoms;

public class SDK_Master : MonoBehaviour
{
    public BoolEvent gameStartEvent;
    public BoolEvent gameCompletedEvent;
    public BoolEvent gameOverEvent;

    public void FindEvents()
    {
        gameStartEvent = GameSettingsData.gameSettings.gameStartEvent;
        Debug.Log("GameStart event " + (gameStartEvent ? "founded successfully!" : "not founded!"));
        
        gameCompletedEvent = GameSettingsData.gameSettings.gameCompletedEvent;
        Debug.Log("GameCompleted event " + (gameCompletedEvent ? "founded successfully!" : "not founded!"));
        
        gameOverEvent = GameSettingsData.gameSettings.gameOverEvent;
        Debug.Log("GameOver event " + (gameOverEvent ? "founded successfully!" : "not founded!"));
    }

    private void Awake() {
        DontDestroyOnLoad(this);
    }

#if SDK_ENABLED

    private void OnEnable() {
        gameStartEvent.Register(SendStartEvent);
        gameCompletedEvent.Register(SendCompletedEvent);
        gameOverEvent.Register(SendFailEvent);
    }

    #region SDK Events

    private void SendStartEvent() {
        Debug.Log("YsoCorp : Level Started");
        YsoCorp.GameUtils.YCManager.instance.OnGameStarted(DataBase.currentLevelNo);
    }

    private void SendCompletedEvent() {
        Debug.Log("YsoCorp : Level Completed");
        YsoCorp.GameUtils.YCManager.instance.OnGameFinished(true);
    }

    private void SendFailEvent() {
        Debug.Log("YsoCorp : Level Over");
        YsoCorp.GameUtils.YCManager.instance.OnGameFinished(false);
    }

    #endregion

#endif

}