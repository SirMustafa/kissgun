using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using MadLab.Sensor;

public class RaycastClick : MonoBehaviour
{
    [SerializeField] private ML_Raycaster raycaster;
    [SerializeField] private BoolEvent onPointerDown;
    Camera myCam;

    private void Start() {
        myCam = GetComponent<Camera>();
        onPointerDown.Register(SendRay);
    }

    private void SendRay(){
        Ray ray = myCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        raycaster.SendRay(ray.direction.normalized, out hit);
        Debug.Log(hit.collider.name);
    }
}
