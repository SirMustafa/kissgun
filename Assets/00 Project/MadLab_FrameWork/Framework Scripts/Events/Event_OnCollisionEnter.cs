using UnityEngine;
using UniRx.Triggers;
using UniRx;
using MadLab.Utilities;

public class Event_OnCollisionEnter : MonoBehaviour
{
    public TriggerAndCollision_EventData[] collisions;

    private void Start() {
        foreach(TriggerAndCollision_EventData collision in collisions)
        {
            if (collision.action.GetPersistentEventCount() > 0)
            {
                switch(collision.compare)
                {
                    case CompareType.Tag:
                        var streamTag = this.OnCollisionEnterAsObservable()
                            .Where(other => other.transform.tag == collision.myTag)
                            .Subscribe(other => collision.action.Invoke());
                        break;

                    case CompareType.Layer:
                        var streamLayer = this.OnCollisionEnterAsObservable()
                            .Where(other => Utility.Contains(collision.layerMask, other.gameObject.layer))
                            .Subscribe(other => collision.action.Invoke());
                        break;

                    case CompareType.Everything:
                        var streamEverything = this.OnCollisionEnterAsObservable()
                            .Subscribe(other => collision.action.Invoke());
                        break;
                }
            }
        }
    }
}
