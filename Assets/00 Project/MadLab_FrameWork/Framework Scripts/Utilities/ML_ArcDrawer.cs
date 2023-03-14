using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;

public class ML_ArcDrawer : MonoBehaviour
{
    public FloatVariable minAngle;
    public FloatVariable maxAngle;
    public FloatVariable radius;

#if UNITY_EDITOR
    public int gizmosQuality = 64;
    public Color gizmosColor = new Color(0f, 0.5f, 1f, 1f);
#endif

    public Vector3 MinDirection{
        private set {}
        get {

            return Quaternion.AngleAxis(minAngle.Value, transform.up) * transform.forward;
        }
    }

    public Vector3 MaxDirection{
        private set {}
        get {

            return Quaternion.AngleAxis(maxAngle.Value, transform.up) * transform.forward;
        }
    }
}