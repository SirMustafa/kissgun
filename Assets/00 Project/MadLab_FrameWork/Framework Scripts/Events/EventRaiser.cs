using UnityEngine;
using UnityAtoms.BaseAtoms;
using Sirenix.OdinInspector;

public class EventRaiser : MonoBehaviour
{
    private enum EventType {
        BoolEvent,
        FloatEvent,
        IntEvent,
        StringEvent,
        Vector2Event,
        Vector3Event
    }

    [SerializeField] private EventType eventType;

    [SerializeField, LabelText("Target Event"), ShowIf("$eventType", EventType.BoolEvent)] private BoolEvent boolEvent;
    [SerializeField, LabelText("Target Event"), ShowIf("$eventType", EventType.FloatEvent)] private FloatEvent floatEvent;
    [SerializeField, LabelText("Target Event"), ShowIf("$eventType", EventType.IntEvent)] private IntEvent intEvent;
    [SerializeField, LabelText("Target Event"), ShowIf("$eventType", EventType.StringEvent)] private StringEvent stringEvent;
    [SerializeField, LabelText("Target Event"), ShowIf("$eventType", EventType.Vector2Event)] private Vector2Event vector2Event;
    [SerializeField, LabelText("Target Event"), ShowIf("$eventType", EventType.Vector3Event)] private Vector3Event vector3Event;

    public void RaiseEvent() {
        switch (eventType) {
            case EventType.BoolEvent:
                if (boolEvent)
                    boolEvent.Raise();
                break;
            
            case EventType.FloatEvent:
                if (floatEvent)
                    floatEvent.Raise();
                break;
            
            case EventType.IntEvent:
                if (intEvent)
                    intEvent.Raise();
                break;
            
            case EventType.StringEvent:
                if (stringEvent)
                    stringEvent.Raise();
                break;

            case EventType.Vector2Event:
                if (vector2Event)
                    vector2Event.Raise();
                break;

            case EventType.Vector3Event:
                if (vector3Event)
                    vector3Event.Raise();
                break;
        }
    }
}
