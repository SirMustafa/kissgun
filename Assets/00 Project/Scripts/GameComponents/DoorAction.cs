using System.Collections.Generic;
using DG.Tweening;
using Dreamteck.Splines;
using Sirenix.OdinInspector;
using UnityEngine;

public class DoorAction : StageAction
{
    [System.Serializable]
    public class Door
    {
        public Transform door;
        public Transform doorPoint;
        public bool isUsed = false;
    }
    
    [SerializeField] private Ease ease;
    [SerializeField] private float doorOpeningTime = 0.5f;

    [SerializeField] private Door door_L;
    [SerializeField] private Door door_R;

    private Door closestDoor(Vector3 pos)
    {
        Door result = null;
        int closest = Vector3.Distance(door_L.door.position, pos) < Vector3.Distance(door_R.door.position, pos)
            ? -1
            : 1;
        
        if (closest == -1)
        {
            if (!door_L.isUsed)
            {
                result = door_L;
                door_L.isUsed = true;
            }
            else if (!door_R.isUsed)
            {
                result = door_R;
                door_R.isUsed = true;
            }
        }
        else
        {
            if (!door_R.isUsed)
            {
                result = door_R;
                door_R.isUsed = true;
            }
            else if (!door_L.isUsed)
            {
                result = door_L;
                door_L.isUsed = true;
            }
        }

        return result;
    }

    private bool EmptyDoors()
    {
        return !door_L.isUsed || !door_R.isUsed;
    }
    
    public override bool DoAction(List<NPCCharacter> enemies, SplineFollower follower)
    {
        StartMe();
        
        bool result = false;

        NPCCharacter[] inLoves = enemies.FindAll(npc => npc.inLove).ToArray();
        int lenght = Mathf.Clamp(inLoves.Length, 0, 2);
        
        for (int i = 0; i < inLoves.Length; i++)
        {
            if (i < lenght)
            {
                Door targetDoor = closestDoor(inLoves[i].transform.position);
                int i1 = i;
                inLoves[i].GoTo(targetDoor.doorPoint.position, targetDoor.doorPoint.eulerAngles, 2f, Ease.InOutSine, () =>
                {
                    inLoves[i1].transform.SetParent(targetDoor.door);
                    inLoves[i1].anim.CrossFadeInFixedTime("Open Doors", 0.2f);
                    PlayerController.instance.GoToNextStage(follower);
                    OpenDoors();
                });
            }
            else
            {
                inLoves[i].LookPlayer(follower);
            }
            result = true;
        }
        
        return result;
    }

    private bool isOpening = false;
    [Button("Open Doors")]
    private void OpenDoors()
    {
        if (isOpening)
            return;
        
        isOpening = true;
        door_L.door.DOLocalRotate(Vector3.up * -110f, doorOpeningTime)
            .SetEase(ease);
        door_R.door.DOLocalRotate(Vector3.up * 110f, doorOpeningTime)
            .SetEase(ease);
    }
}
