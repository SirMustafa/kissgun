using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UniRx.Triggers;
using UniRx;
using MadLab.Utilities;

public enum CompareType
{
    Tag,
    Layer,
    Everything
}

[System.Serializable]
public class TriggerAndCollision_EventData
{
    [FoldoutGroup("$GetTitle"), ValueDropdown("GetCompareType")]
    public CompareType compare;

    [FoldoutGroup("$GetTitle"), ShowIf("compare", CompareType.Tag)]
    public string myTag = "Untagged";

    [FoldoutGroup("$GetTitle"), ShowIf("compare", CompareType.Layer)]
    public LayerMask layerMask;

    [FoldoutGroup("$GetTitle")] public UnityEvent action;

    private static IEnumerable<CompareType> GetCompareType = Enumerable.Range(0, 3).Cast<CompareType>();

    string GetTitle()
    {
        string title = "";

        switch(compare)
        {
            case CompareType.Tag:
                if (myTag == "")
                    title = "No Tag";
                else
                    title = myTag;
                break;
            
            case CompareType.Layer:
                title = "Layers";
                break;

            case CompareType.Everything:
                title = "Everything";
                break;
        }

        return title;
    }
}

public class Event_OnTriggerEnter : MonoBehaviour
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
                        var streamTag = this.OnTriggerEnterAsObservable()
                            .Where(other => other.transform.tag == trigger.myTag)
                            .Subscribe(other => trigger.action.Invoke());
                        break;

                    case CompareType.Layer:
                        var streamLayer = this.OnTriggerEnterAsObservable()
                            .Where(other => Utility.Contains(trigger.layerMask, other.gameObject.layer))
                            .Subscribe(other => trigger.action.Invoke());
                        break;

                    case CompareType.Everything:
                        var streamEverything = this.OnTriggerEnterAsObservable()
                            .Subscribe(other => trigger.action.Invoke());
                        break;
                }
            }
        }
    }
}