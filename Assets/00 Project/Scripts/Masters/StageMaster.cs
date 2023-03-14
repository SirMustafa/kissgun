using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using UnityEngine.Events;

public class StageMaster : MonoBehaviour
{
    #region Singleton
    public static StageMaster instance = null;
    private void Awake() {
        instance = this;
    }
    #endregion

    [SerializeField] private Stage[] stages;
    [SerializeField] private BoolEvent startEvent;
    [SerializeField] private BoolVariable onStage;
    [SerializeField] private UnityEvent onPassToNextStage;
    
    public Stage currentStage { get; private set; }
    int currentStageIndex = 0;

    public int StageCount(){
        return stages.Length;
    }

    private void Start()
    {
        currentStage = stages[0];
        onStage.SetValue(true);
        startEvent.Register(StartStage);

        for (int i = 0; i < stages.Length; i++)
        {
            stages[i].Setup(i);
        }
    }
    
    public Stage GetStage(int index){
        if (index < stages.Length && index >= 0)
            return stages[index];
        else
            return null;
    }

    public void SomeEnemyKilled(int stageId)
    {
        stages[stageId].DecreaseEnemyCount();
    }

    public void PassToNextStage()
    {
        currentStageIndex++;
        
        if (currentStageIndex < stages.Length)
        {
            if (!currentStage.DoStageAction())
            {
                PlayerController.instance.GoToNextStage(stages[currentStageIndex - 1].GetFollower());
            }

            currentStage = stages[currentStageIndex];
            
            if (onPassToNextStage.GetPersistentEventCount() > 0)
                onPassToNextStage.Invoke();
        }
        else if (currentStageIndex == stages.Length)
        {
            List<NPCCharacter> enemies = new List<NPCCharacter>(currentStage.enemies);
            enemies.FindAll(x => x.inLove).ForEach(x => x.LookPlayer());
            
            if (GameManager.GameStatus == GameStatus.Started)
                GameManager.instance.Victory();
        }
        
        onStage.SetValue(false);
        GameManager.instance.PassToNextChapter();
    }

    public void StartStage()
    {
        if (!onStage.Value)
            onStage.SetValue(true);
        currentStage.StartStage();
    }
}
