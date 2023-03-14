using UnityEngine;

public class EventMaster : MonoBehaviour
{
    [SerializeField] EventMaster_SO eventMaster;

    private void Start() {
        eventMaster.levelLoaded.Register(eventMaster.UnregisterAll);
    }
}
