using DG.Tweening;

namespace ElCapitan.StatePatterns
{
    public class NPCJumpState : BaseState
    {
        private NPCCharacter npc;
        
        public NPCJumpState(NPCCharacter npc)
        {
            this.npc = npc;
        }
    
        public override void Initialize()
        {
            DoState();
        }

        public override void DoState()
        {
            npc.anim.SetBool("Air", true);
            npc.anim.Play("falling");
            npc.transform.DOJump(npc.jumpPoint.position, 4, 1, npc.jumpDelay) 
                .SetDelay(npc.jumpDelay)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    npc.anim.SetBool("Air", false);
                    npc.jump = false;
                    npc.WakeUp();
                });
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}