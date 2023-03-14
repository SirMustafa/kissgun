using UniRx;
using UnityEngine;

namespace ElCapitan.StatePatterns
{
    public class NPCIdleState : BaseState
    {
        private NPCCharacter npc;
        
        public NPCIdleState(NPCCharacter npc)
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
            if (!npc.target)
            {
                _disposable.Dispose();
                return;
            }
                
            if (npc.myVelocity != Vector3.zero)
            {
                npc.myVelocity = Vector3.Lerp(npc.myVelocity, Vector3.zero, 10 * Time.fixedDeltaTime);
                npc.rb.velocity = npc.myVelocity;
                npc.anim.SetFloat("Speed", npc.myVelocity.magnitude / npc.currentSpeed);
            }
            
            npc.CalculateDir();
            npc.LookToTarget();
        }
    }
}