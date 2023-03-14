using UniRx;
using UnityEngine;

public class NPCMoveState : BaseState
{
    private NPCCharacter npc;
        
    public NPCMoveState(NPCCharacter npc)
    {
        this.npc = npc;
    }
    
    public override void Initialize()
    {
        _disposable = Observable.EveryFixedUpdate().Subscribe(_ =>
        {
            DoState();
        });
    }

    public override void DoState()
    {
        if (npc.target)
        {
            if (Vector3.Distance(npc.transform.position, npc.targetPos) <= npc.attackRadius.Value)
            {
                npc.SetState(npc.attackState);
            }
            else
            {
                if (npc.inLove && !npc.CanFight())
                    npc.SetState(npc.wanderState);
                
                npc.myDir = AdvancedMath.ScaleTopdown(npc.targetPos - npc.transform.position).normalized;
                Vector3 targetVelocity = npc.myDir * npc.currentSpeed;
                npc.myVelocity = Vector3.Lerp(npc.myVelocity, targetVelocity, 10 * Time.fixedDeltaTime);
                npc.rb.velocity = npc.myVelocity;
                npc.anim.SetFloat("Speed", npc.myVelocity.magnitude / npc.currentSpeed);
                
                npc.LookToTarget();
            }
        }
        else if (StageMaster.instance.GetStage(npc.stageIndex).alivedEnemyCount.Value > 0)
        {
            npc.target = StageMaster.instance.GetStage(npc.stageIndex).ClosestEnemy(npc.transform.position).health;
        }
        else // Stage completed
        {
            npc.SetState(npc.idleState);
        }
    }
}
