using UnityEngine;

public class NPCDeathState : BaseState
{
    private NPCCharacter npc;
    
    public NPCDeathState(NPCCharacter npc)
    {
        this.npc = npc;
    }

    public override void Initialize()
    {
        DoState();
    }

    public override void DoState()
    {
        
    }
}