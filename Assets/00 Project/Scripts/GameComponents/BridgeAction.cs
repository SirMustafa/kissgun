using System.Collections.Generic;
using DG.Tweening;
using Dreamteck.Splines;
using UnityEngine;

public class BridgeAction : StageAction
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private SplineFollower bridgePath;
    
    public override bool DoAction(List<NPCCharacter> enemies, SplineFollower follower)
    {
        bool result = false;
        NPCCharacter[] inLove = enemies.FindAll(x => x.inLove).ToArray();
        
        bridgePath.targetTransform = inLove[0].transform;

        if (inLove.Length > 0)
            result = true;

        if (inLove.Length > 1)
        {
            for (int i = 1; i < inLove.Length; i++)
            {
                inLove[i].LookPlayer(follower);
            }
        }

        inLove[0].GoTo(startPoint.position, startPoint.eulerAngles, 2f, Ease.InOutSine, () =>
        {
            bridgePath.follow = true;
            inLove[0].anim.CrossFade("Bridge", 0.25f, 0, 0f);
            PlayerController.instance.GoToNextStage(follower);
        }, true);
        
        return result;
    }
}
