using UnityEngine;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using Sirenix.OdinInspector;
using UnityAtoms;

[CreateAssetMenu(fileName = "Event Master", menuName = "ML_Framework/Event Master", order = 1)]
public class EventMaster_SO : ScriptableObject
{
    public BoolEvent levelLoaded;
    [TabGroup("Events")]
    public List<AtomEventBase> events = new List<AtomEventBase>();
    [TabGroup("Variables")]
    public List<AtomBaseVariable> variables = new List<AtomBaseVariable>();

    public void UnregisterAll()
    {
        if (events.Count > 0)
            events.ForEach(x => x.UnregisterAll());

        if (variables.Count > 0)
            variables.ForEach(x => x.Reset());
    }
}
