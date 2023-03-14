using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;
using MoreMountains.NiceVibrations;

public enum GameStatus
{
    WaitForStart,
    Started,
    Paused,
    GameOver,
    Finished
}

public class GameManager : OptimizedUpdate
{
    #region  Singleton
    public static GameManager instance = null;
    
    private void Awake()
    {
        instance = this; 
        GameStatus = GameStatus.WaitForStart;
    }
    #endregion

    GameSettings gameSettingsData;
    private int currentChapter = 0;

    [HideInInspector] public int chapterCount = 2;
    [TabGroup("Level")] public Transform finish;

    [TabGroup("Events"), SerializeField] private BoolEvent gameStartEvent;
    [TabGroup("Events"), SerializeField] private BoolEvent gameOverEvent;
    [TabGroup("Events"), SerializeField] private BoolEvent victoryEvent;



    public static GameStatus GameStatus = GameStatus.WaitForStart;
    
    public static bool isChapterMode => MadLab.GameSettingsData.gameSettings.chapterMode;

    private float totalDis;

    bool chapterSystem = false;
    int charactersReachedFinish = 0;

    private void Start() 
    {
        TimeMaster.SetTimeDirectly(1);
        chapterSystem = isChapterMode;
        chapterCount = StageMaster.instance.StageCount();

//        AudioMaster.instance.SetMusicLowpass(0f);

        GameStatus = GameStatus.WaitForStart;

        totalDis = Mathf.Abs(PlayerController.instance.transform.position.z - finish.position.z);

        UIMaster.instance.Setup();
        LevelLoader.instance.StartCurrentLevel();

        SetupOptimizedUpdate();
    }

    public float Dis2Finish(float zPos)
    {
        return finish.position.z - zPos;
    }

    public override void UpdateMe(float deltaTime)
    {
        if (GameStatus != GameStatus.Started)
            return;

        if (!chapterSystem)
        {
            float progress = Mathf.Clamp01((totalDis - (finish.position.z - PlayerController.instance.transform.position.z)) / totalDis);
            UIMaster.instance.UpdateProgressBar(progress);
        }
    }


    #region Pause System

    public void PauseGame()
    {
        GameStatus = GameStatus.Paused;
    }

    public void ResumeGame()
    {
        UIMaster.instance.ResumeGame();
    }

    #endregion

    #region Game Events

    public void ReachFinish(out int place, bool isPlayer = false){
        charactersReachedFinish++;
        place = charactersReachedFinish;

        if (isPlayer)
        {
     //       FinishTheGame(place);
        }
    }

    public void PassToNextChapter(){
        if (currentChapter < chapterCount)
            currentChapter++;

        UIMaster.instance.UpdateProgressBar(currentChapter);
/*
        if (currentChapter == chapterCount)
        {
            int place;
            ReachFinish(out place, true);
            FinishTheGame(place);
        }*/
    }
/*
    public void FinishTheGame(int place) {
        UIMaster.instance.SetRank(place);
        Victory();
    }*/

    public void StartGame()
    {
        UIMaster.instance.StartGame();
        GameStatus = GameStatus.Started;

        gameStartEvent.Raise();
        if (AudioMaster.instance)
            AudioMaster.instance.SetMusicLowpass(1f, 1f);
    }

    public void Victory() 
    {
        GameStatus = GameStatus.Finished;
        VibrationMaster.Haptic(HapticTypes.Success);
        UIMaster.instance.Victory();

        victoryEvent.Raise();
    }

    public void GameOver() 
    {
        GameStatus = GameStatus.GameOver;
        VibrationMaster.Haptic(HapticTypes.Warning);
        UIMaster.instance.GameOver();
        
        gameOverEvent.Raise();
    }

    #endregion

}
