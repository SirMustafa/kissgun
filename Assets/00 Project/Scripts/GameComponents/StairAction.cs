using System.Collections.Generic;
using DG.Tweening;
using Dreamteck.Splines;
using UnityEngine;

public class StairAction : StageAction
{
    [SerializeField] private Transform point;
    [SerializeField] private float characterLenght = 1.8f;
    [SerializeField] private int maxStairLenght;
    public override bool DoAction(List<NPCCharacter> enemies, SplineFollower follower)
    {
        bool result = false;

        List<NPCCharacter> inLove = enemies.FindAll(npc => npc.inLove);
        if (inLove.Count > 0)
        {
            result = true;
            
            var position = point.position;

            inLove.Sort(CompareDistance);

            int lenght = Mathf.Clamp(inLove.Count, 0, maxStairLenght);
            
            for (int i = 0; i < inLove.Count; i++)
            {
                if (i < lenght)
                {
                    int i1 = i;
                    if (i == 0)
                    {
                        inLove[i].GoTo(point.position, point.eulerAngles, 1f, Ease.InOutSine, () =>
                        {
                            inLove[i1].rb.isKinematic = true;
                            if (i1 == lenght - 1)
                            {
                                follower.spline.SetPointPosition(2, position + Vector3.up * (i1 + 1) * characterLenght);
                                PlayerController.instance.GoToNextStage(follower);
                            }
                        });
                    }
                    else
                    {
                        Vector3 dir = position - inLove[i].transform.position;
                        dir = AdvancedMath.ScaleTopdown(dir).normalized;
                        inLove[i].GoTo(position - dir * 2, point.eulerAngles, 1 + i * 0.25f, Ease.InOutSine, () =>
                        {
                            inLove[i1].transform.DOMoveX(position.x, i1 * 0.4f)
                                .SetEase(Ease.Linear);
                            inLove[i1].transform.DOMoveY(position.y + i1 * characterLenght, i1 * 0.4f)
                                .SetEase(Ease.OutBack, 2f)
                                .OnComplete(() =>
                                {
                                    if (i1 == lenght - 1)
                                    {
                                        follower.spline.SetPointPosition(2, position + Vector3.up * (i1 + 1) * characterLenght);
                                        PlayerController.instance.GoToNextStage(follower);
                                    }
                                });
                            inLove[i1].transform.DOMoveZ(position.z, i1 * 0.4f)
                                .SetEase(Ease.Linear);

                            inLove[i1].rb.isKinematic = true;
                        });
                    }
                }
                else
                {
                    inLove[i].LookPlayer(follower);
                }
            }
        }
        else
        {
            follower.spline.SetPointPosition(2, point.position);
        }

        return result;
    }

    private int CompareDistance(NPCCharacter npc1, NPCCharacter npc2)
    {
        var position = point.position;
        float npc1Dis = Vector3.Distance(npc1.transform.position, position);
        float npc2Dis = Vector3.Distance(npc2.transform.position, position);
        return npc1Dis.CompareTo(npc2Dis);
    }
}
