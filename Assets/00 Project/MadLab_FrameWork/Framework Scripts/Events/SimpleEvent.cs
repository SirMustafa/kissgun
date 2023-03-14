using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class SimpleEvent : MonoBehaviour
{
    [SerializeField] private bool showEvent = true;
    
    [SerializeField, ShowIf("$showEvent")]
    private UnityEvent myEvent = new UnityEvent();

    public void SetEvent(UnityAction action)
    {
        myEvent.RemoveAllListeners();
        myEvent.AddListener(action);
    }
    
    
    public void InvokeEvent()
    {
        myEvent.Invoke();
    }
}
