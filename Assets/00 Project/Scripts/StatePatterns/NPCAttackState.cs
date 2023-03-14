using DG.Tweening;
using UniRx;
using UnityEngine;

public class NPCAttackState : BaseState
{
    private NPCCharacter myNpc;
    private bool fight = false;

    public NPCAttackState(NPCCharacter myNpc)
    {
        this.myNpc = myNpc;
    }

    public override void Initialize()
    {
        DOTween.To(() => myNpc.myVelocity, x => myNpc.myVelocity = x, Vector3.zero, 0.25f)
            .OnUpdate(() =>
            {
                myNpc.anim.SetFloat("Speed", myNpc.myVelocity.magnitude / myNpc.currentSpeed);
            });
        
        _disposable = Observable.EveryUpdate().Subscribe(_ =>
        {
            DoState();
        });
    }

    public override void DoState()
    {
        if (myNpc.target && myNpc.target.isAlive && myNpc.CanFight()) // My target is alive and if i can fight with them
        {
            if (Time.time >= myNpc.nextAttack) // For attack rate
            {
                if (!fight && myNpc.inLove) // For inviting fighting
                {
                    fight = true;
                    myNpc.npcTargets[0].Fight(myNpc); // Invite target npc to fighting
                }
                
                myNpc.nextAttack = Time.time + myNpc.attackRate; // Calculate next attack time
                float startFigure = myNpc.anim.GetFloat("Attack Figure"); // Get current figure
                DOTween.To(() => startFigure, x => startFigure = x, (int) Random.Range(0, 4), 0.2f) // Smoothly translate Attack figure to random figure
                    .OnUpdate(() =>
                    {
                        myNpc.anim.SetFloat("Attack Figure", startFigure);
                    });
                myNpc.attackTargetId = myNpc.target.GetInstanceID();
                myNpc.anim.CrossFadeInFixedTime("Attack", 0.2f, 0, 0f); // Play Attack animation
            }
            
            myNpc.CalculateDir(); // Calculate look direction
            myNpc.LookToTarget(); // Look at to target
        }
        else // If target is missing or i can fight with them
        {
            if (fight)
                fight = false;
            
            myNpc.SetState(myNpc.wanderState); // Change my state to wander state for researching targets
        }
    }
}