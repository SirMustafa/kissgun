using System;
using System.Collections.Generic;
using Dreamteck.Splines;
using Sirenix.OdinInspector;
using UnityEngine;
using UniRx;

public class Stage : MonoBehaviour {

    [SerializeField] private SplineFollower follower;
    [SerializeField] private StageAction action;
    public List<NPCCharacter> enemies = new List<NPCCharacter>();
    public ReactiveProperty<int> alivedEnemyCount { private set; get; } = new ReactiveProperty<int>(0);
    IDisposable stagePasser;

    private void Start()
    {
        alivedEnemyCount.Value = enemies.Count;
        
        // When all enemies die pass to next level
        stagePasser = alivedEnemyCount.Where(value => value == 0)
            .Subscribe(value =>
            {
                StageMaster.instance.PassToNextStage();
                stagePasser.Dispose();
            });
    }

    public NPCCharacter ClosestEnemy(Vector3 position)
    {
        float distance = 1000;
        
        int index = 0;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].ItsMakeDanger())
            {
                float newDis = Vector3.Distance(position, enemies[i].transform.position);
                if (newDis < distance)
                {
                    distance = newDis;
                    index = i;
                }
            }
        }

        return enemies[index];
    }

    public void Setup(int stageId)
    {
        foreach (NPCCharacter enemyCharacter in enemies)
        {
            enemyCharacter.stageIndex = stageId;
        }
    }

    public SplineFollower GetFollower()
    {
        return follower;
    }

    public void StartStage()
    {
        foreach (NPCCharacter enemyCharacter in enemies)
        {
            enemyCharacter.WakeUp();
        }
    }

    public void DecreaseEnemyCount(){
        alivedEnemyCount.Value--;
    }

    /// <summary>
    /// If we have enemies in love, we use this enemies for passing next stage.
    /// </summary>
    /// <returns></returns>
    [Button("Do Action")]
    public bool DoStageAction()
    {
        bool result = action.DoAction(enemies, follower);
        return result;
    }
}