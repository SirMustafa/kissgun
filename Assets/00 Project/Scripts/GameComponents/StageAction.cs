using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class StageAction : MonoBehaviour
{
    protected void StartMe()
    {

    }
    
    public virtual bool DoAction(List<NPCCharacter> enemies, SplineFollower follower)
    {
        return true;
    }
}
