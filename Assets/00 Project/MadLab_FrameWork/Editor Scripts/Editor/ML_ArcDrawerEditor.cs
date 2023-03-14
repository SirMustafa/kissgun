using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

[CustomEditor(typeof(ML_ArcDrawer))]
public class ML_ArcDrawerEditor : Editor
{
    ML_ArcDrawer t;
    JointAngularLimitHandle m_Handle = new JointAngularLimitHandle();
    float lastScale = 1f;
    bool editMode = false;

    private void OnEnable() {
        t = (ML_ArcDrawer)target;
        lastScale = t.radius.InitialValue;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

    private void OnSceneGUI() {
        if (!t.minAngle || !t.maxAngle || !t.radius)
            return;

        m_Handle.yMin = t.minAngle.InitialValue;
        m_Handle.yMax = t.maxAngle.InitialValue;

        Matrix4x4 handleMatrix = Matrix4x4.TRS(
            t.transform.position,
            t.transform.rotation,
            Vector3.one
        );

        Vector3 normal = t.transform.rotation * Vector3.up;
        Vector3 from = Quaternion.AngleAxis(t.minAngle.InitialValue, t.transform.up) * t.transform.forward;
        Vector3 centerDir = Quaternion.AngleAxis(t.minAngle.InitialValue + Mathf.Abs(t.minAngle.InitialValue - t.maxAngle.InitialValue) * 0.5f, t.transform.up) * t.transform.forward;

        EditorGUI.BeginChangeCheck();
        int arcQuality = t.gizmosQuality;
        for (int i = 0; i < arcQuality; i++) {
            Handles.color = new Color(t.gizmosColor.r, t.gizmosColor.g, t.gizmosColor.b, AdvancedMath.Remap(0, arcQuality, 1f / (arcQuality * 0.5f), 0f, i));
            Handles.DrawSolidArc(t.transform.position, normal, from, Mathf.Abs(t.minAngle.InitialValue - t.maxAngle.InitialValue), AdvancedMath.Remap(0,arcQuality - 1,0,t.radius.InitialValue,i));
        }
        Handles.color = Color.white;
        
        if (editMode) {
            float scale = t.radius.InitialValue;
            float snap = 0.5f;

                Event e = Event.current;
                switch (e.type)
                {
                    case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        lastScale = t.radius.InitialValue;
                    }
                    break;

                    case EventType.MouseUp:
                    if (e.button == 0)
                    {
                        lastScale = scale;
                    }
                    break;
                }

            EditorGUI.BeginChangeCheck();
            scale = Handles.ScaleSlider(t.radius.InitialValue, t.transform.position, centerDir, Quaternion.LookRotation(centerDir, t.transform.up), lastScale, snap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Change Scale Value");

                t.radius.InitialValue = scale;
                t.radius.Value = scale;
            }


            using (new Handles.DrawingScope(handleMatrix))
            {
                // maintain a constant screen-space size for the handle's radius based on the origin of the handle matrix
                m_Handle.radius = t.radius.InitialValue;
                m_Handle.fillAlpha = 0.0f;
                m_Handle.yHandleColor = Color.white;
                m_Handle.xHandleColor = new Color(0f, 0f, 0f, 0f);
                m_Handle.zHandleColor = new Color(0f, 0f, 0f, 0f);

                // draw the handle
                EditorGUI.BeginChangeCheck();
                m_Handle.DrawHandle();
                if (EditorGUI.EndChangeCheck())
                {
                    // record the target object before setting new values so changes can be undone/redone
                    Undo.RecordObject(t, "Change Joint Example Properties");

                    // copy the handle's updated data back to the target object
                    t.minAngle.InitialValue = m_Handle.yMin;
                    t.maxAngle.InitialValue = m_Handle.yMax;
                    t.minAngle.Value = m_Handle.yMin;
                    t.maxAngle.Value = m_Handle.yMax;
                }
            }
        }
        else
        {            
            Handles.DrawWireArc(t.transform.position, normal, from, Mathf.Abs(t.minAngle.InitialValue - t.maxAngle.InitialValue), t.radius.InitialValue, 3);
            Handles.DrawDottedLine(t.transform.position, t.transform.position + from * t.radius.InitialValue, 3);

            Vector3 end = Quaternion.AngleAxis(t.minAngle.InitialValue + Mathf.Abs(t.minAngle.InitialValue - t.maxAngle.InitialValue), t.transform.up) * t.transform.forward;;
            Handles.DrawDottedLine(t.transform.position, t.transform.position + end * t.radius.InitialValue, 3);
        }

        Handles.BeginGUI();

        GUILayout.BeginVertical();
        GUILayout.BeginArea(new Rect(20, 20, 60, 500));

        if (GUILayout.Button(editMode ? "Apply" : "Edit", GUILayout.MinHeight(30))){
            editMode = !editMode;
        }

        if (GUILayout.Button("Scale:1", GUILayout.MinHeight(30))){
            t.radius.InitialValue = 1f;
            t.radius.Value = 1f;
            lastScale = 1f;
        }

        GUILayout.EndArea();
        GUILayout.EndVertical();

        Handles.EndGUI();

/*
        Handles.DrawWireArc(t.transform.position, t.rotateAxis, Vector3.right, Mathf.Abs(t.startAngle - t.endAngle), 1f, 5f);
        Handles.color = new Color(0f, 1f, 0f, 0.4f);
        Handles.DrawSolidArc(t.transform.position, t.rotateAxis, Vector3.right, Mathf.Abs(t.startAngle - t.endAngle), 1f);*/
    }
}
