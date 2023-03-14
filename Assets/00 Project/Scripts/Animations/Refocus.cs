using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refocus : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private void OnApplicationFocus(bool focusStatus) {
        if (focusStatus)
        {
            anim.Update(Time.deltaTime);
        }        
    }
}
