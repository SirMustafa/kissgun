using UniRx;

public class NPCWanderState : BaseState
{
    private NPCCharacter myNpc;
    
    public NPCWanderState(NPCCharacter npc)
    {
        myNpc = npc;
    }

    public override void Initialize()
    {
        _disposable = Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                DoState();
            });
    }

    public override void DoState()
    {
        if (myNpc.inLove) // If in love and current stage have enemies
        {
            if (StageMaster.instance.GetStage(myNpc.stageIndex).alivedEnemyCount.Value > 0)
            {
                myNpc.npcTargets.Clear(); // clear my targets
                myNpc.Fight(StageMaster.instance.GetStage(myNpc.stageIndex).ClosestEnemy(myNpc.transform.position)); // Find new target and fight them
            }
            else
            {
                myNpc.SetState(myNpc.idleState);
            }
        } 
        else
        {
            if (myNpc.npcTargets.Count > 0) // If mynpc have old targets(invited)
            {
                bool researching = true;
                while (researching)
                {
                    if (myNpc.npcTargets[0].health.isAlive) // Check target is alive
                    {
                        myNpc.Fight(myNpc.npcTargets[0]);
                        researching = false;
                    }
                    else
                    {
                        myNpc.npcTargets.RemoveAt(0);
                        if (myNpc.npcTargets.Count == 0)
                            researching = false;
                    }
                }
            }
            else // If myNpc havent targets new target is player
            {
                myNpc.target = PlayerController.instance.health;
                myNpc.SetState(myNpc.moveState);
            }
        }
    }
}