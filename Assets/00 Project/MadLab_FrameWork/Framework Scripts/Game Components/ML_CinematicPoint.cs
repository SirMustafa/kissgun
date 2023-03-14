using UnityEngine;

public class ML_CinematicPoint : MonoBehaviour
{

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            CameraFollow.instance.LookFromPoint(1);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            CameraFollow.instance.LookFromPoint(0);
        }
    }
}
