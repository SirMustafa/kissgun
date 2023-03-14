using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Dreamteck.Splines;
using UnityEngine;
using Random = UnityEngine.Random;

public class WallAction : StageAction
{
    [SerializeField] private GameObject solid;
    [SerializeField] private GameObject broken;
    [SerializeField] private Health hp;
    [SerializeField] private List<Transform> stayPoints = new List<Transform>();

    public override bool DoAction(List<NPCCharacter> enemies, SplineFollower follower)
    {
        bool result = false;
        NPCCharacter[] inLove = enemies.FindAll(x => x.inLove).ToArray();

        int pointCount = stayPoints.Count;
        for (int i = 0; i < inLove.Length; i++)
        {
            if (i < pointCount)
            {
                int point = Random.Range(0, stayPoints.Count);
                Transform myPoint = stayPoints[point];
                stayPoints.RemoveAt(point);
                int i1 = i;
                inLove[i].GoTo(myPoint.position, myPoint.eulerAngles, 2f, Ease.InOutSine, () =>
                {
                    inLove[i1].SetTarget(hp);
                    inLove[i1].anim.SetFloat("Attack Figure", (int)Random.Range(0, 3));
                    inLove[i1].anim.CrossFadeInFixedTime("Attack", 0.2f);
                    PlayerController.instance.GoToNextStage(follower);
                });
            }
            else
            {
                inLove[i].LookPlayer(follower);
            }
            result = true;
        }
        
        return result;
    }
}
