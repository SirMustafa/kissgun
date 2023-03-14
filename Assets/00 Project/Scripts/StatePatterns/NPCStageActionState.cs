using DG.Tweening;
using UniRx;
using UnityEngine;

public class NPCStageActionState : BaseState
{
    public bool doAction = false;
    private NPCCharacter myNpc;
    private Vector3 pos;
    
    public NPCStageActionState(NPCCharacter npc)
    {
        myNpc = npc;
    }

    public override void Initialize()
    {
        pos = myNpc.transform.position;
        if (!doAction && Mathf.Abs(pos.x) < 1f)
        {
            var pos = myNpc.transform.position;
            pos.x = Random.Range(1, 2f) * Mathf.Sign(pos.x);
            myNpc.GoTo(pos, Quaternion.LookRotation(Vector3.right * Mathf.Sign(pos.x), Vector3.up).eulerAngles, 1f, Ease.InOutSine, () => myNpc.LookPlayer());
            return;
        }
        
        _disposable = Observable.EveryFixedUpdate().Subscribe(_ =>
        {
            DoState();
        });
    }

    public override void DoState()
    {
        if (!doAction)
        {
            if (myNpc.myVelocity != Vector3.zero)
            {
                myNpc.myVelocity = Vector3.Lerp(myNpc.myVelocity, Vector3.zero, 10 * Time.fixedDeltaTime);
                myNpc.rb.velocity = myNpc.myVelocity;
                myNpc.anim.SetFloat("Speed", myNpc.myVelocity.magnitude / myNpc.currentSpeed);
            }
                
            myNpc.CalculateDir();
            myNpc.LookToTarget();
        }
    }
}