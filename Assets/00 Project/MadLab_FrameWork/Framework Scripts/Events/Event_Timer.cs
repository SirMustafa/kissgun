using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UniRx.Triggers;
using UniRx;
using System;

public enum TimerType
{
    Single,
    Loop
}

[System.Serializable]
public class TimerData
{
    [FoldoutGroup("Timer Settings"), Space(5f)]
    [EnumToggleButtons, HideLabel] public TimerType type;

    [FoldoutGroup("Timer Settings"), Space(5f)]
    public float time;

    [FoldoutGroup("Timer Settings")]
    [Space(10f)] public UnityEvent action;
}

[HideMonoScript]
public class Event_Timer : MonoBehaviour
{
    [SerializeField, BoxGroup("Timers"), HideLabel] private TimerData[] timers;
    List<IDisposable> timersDisposable = new List<IDisposable>();

    private void OnEnable() 
    {
        for(int i = 0; i < timers.Length; i++)
        {
            if (timers[i].action.GetPersistentEventCount() > 0)
            {
                int id = i;
                switch(timers[i].type)
                {
                    case TimerType.Single:
                        var singleStream = Observable.Interval(TimeSpan.FromSeconds(timers[i].time)).Subscribe(x => 
                        {
                            timers[id].action.Invoke();
                            timersDisposable[id].Dispose();
                        });
                        timersDisposable.Insert(i, singleStream);
                        break;
                    
                    case TimerType.Loop:
                        var loopStream = Observable.Interval(TimeSpan.FromSeconds(timers[i].time)).Subscribe(x => 
                            {
                                timers[id].action.Invoke();
                            });
                        timersDisposable.Insert(i, loopStream);
                        break;
                }
            }
        }
    }

    private void OnDisable() {
        if(timersDisposable.Count > 1)
            timersDisposable.ForEach(x => x.Dispose());
        else
            timersDisposable[0].Dispose();
    }
}
