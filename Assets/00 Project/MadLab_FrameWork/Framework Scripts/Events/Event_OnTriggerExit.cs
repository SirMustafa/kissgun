using UnityEngine;
using UniRx.Triggers;
using UniRx;
using MadLab.Utilities;

public class Event_OnTriggerExit : MonoBehaviour
{
    public TriggerAndCollision_EventData[] triggers;

    private void Start() {
        foreach(TriggerAndCollision_EventData trigger in triggers)
        {
            if (trigger.action.GetPersistentEventCount() > 0)
            {
                switch(trigger.compare)
                {
                    case CompareType.Tag:
                        var streamTag = this.OnTriggerExitAsObservable()
                            .Where(other => other.transform.tag == trigger.myTag)
                            .Subscribe(other => trigger.action.Invoke());
                        break;

                    case CompareType.Layer:
                        var streamLayer = this.OnTriggerExitAsObservable()
                            .Where(other => Utility.Contains(trigger.layerMask, other.gameObject.layer))
                            .Subscribe(other => trigger.action.Invoke());
                        break;

                    case CompareType.Everything:
                        var streamEverything = this.OnTriggerExitAsObservable()
                            .Subscribe(other => trigger.action.Invoke());
                        break;
                }
            }
        }
    }
}