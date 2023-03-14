using UnityEngine;
using UnityEditor;

[System.Serializable]
public enum MathfTools
{
    DistanceCalculator,
    Remap
}

public class MathfAsistan : EditorWindow {

    private MathfTools currentMathfTool;


    [MenuItem("ML Framework/Mathf Asistan")]
    private static void ShowWindow() {
        var window = GetWindow<MathfAsistan>();
        window.titleContent = new GUIContent("MathfAsistan");
        window.Show();
    }

    private void OnGUI() {

        GUILayout.Label("Mathf Asistan");


        currentMathfTool = (MathfTools)EditorGUILayout.EnumPopup(currentMathfTool);
        switch (currentMathfTool)
        {
            case MathfTools.DistanceCalculator:
                DrawDistanceCalculator();
                break;
            
            case MathfTools.Remap:
                DrawRemap();
                break;
        }
    }

    #region DistanceCalculator
    Transform t1;
    Transform t2;

    bool x;
    bool z;
    bool y;

    bool calculated = false;
    Vector3 result = Vector3.zero;

    private void DrawDistanceCalculator()
    {
        GUILayout.BeginVertical();

        t1 = EditorGUILayout.ObjectField("Point 1 : ", t1, typeof(Transform), true) as Transform;
        t2 = EditorGUILayout.ObjectField("Point 2 : ", t2, typeof(Transform), true) as Transform;

        GUILayout.BeginHorizontal();

        x = GUILayout.Toggle(x, "x");
        y = GUILayout.Toggle(y, "y");
        z = GUILayout.Toggle(z, "z");

        GUILayout.EndHorizontal();

        if (t1 && t2)
        {
            if (GUILayout.Button("Calculate Distance"))
            {
                result = ScalePos(t2.position) - ScalePos(t1.position);
                calculated = true;
            }

            if (calculated)
            {
                GUILayout.Label("Result : " + result.magnitude);
                GUILayout.Label("Direction : " + result);
            }
        }
        else if (calculated)
        {
            result = Vector3.zero;
            calculated = false;
        }

        GUILayout.EndVertical();
    }

    private Vector3 ScalePos(Vector3 position)
    {
        return Vector3.Scale(new Vector3(x ? 1f : 0f, y ? 1f : 0f, z ? 1f : 0f), position);
    }

    #endregion

    #region Remap

    Vector2 firstRange = new Vector2(0f, 1f);
    Vector2 targetRange = new Vector2(0f, 0f);
    float remapEnter = 0f;
    float remapResult = 0f;
    bool remapCalculated = false;

    private void DrawRemap()
    {
        remapEnter = EditorGUILayout.FloatField("Value :", remapEnter);
        firstRange = EditorGUILayout.Vector2Field("First Range :", firstRange);
        targetRange = EditorGUILayout.Vector2Field("Target Range :", targetRange);

        if (GUILayout.Button("Remap")){
            remapResult = AdvancedMath.Remap(firstRange.x, firstRange.y, targetRange.x, targetRange.y, remapEnter);
            remapCalculated = true;
        }

        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;

        if (remapCalculated){
            GUILayout.Label("Result : " + remapResult, style);
        }
    }

    #endregion
}