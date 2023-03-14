using UnityEngine;
using UnityEditor;
using MadLab.Utilities;

public class ColorEditor : EditorWindow {

    private string currentColorHex;
    private string currentColorRGBA;
    private Color clr = Color.white;

    private Texture2D colorTex;
    private Texture2D noPreviewTex;
    private Rect colorTexRect;

    [MenuItem("ML Framework/Color Editor")]
    private static void ShowWindow() {
        var window = GetWindow<ColorEditor>();
        window.titleContent = new GUIContent("Color Editor");
        window.minSize = new Vector2(200, 360);
        window.maxSize = new Vector2(200, 360);
        window.Show();
    }

    private void OnEnable() {
        noPreviewTex = Resources.Load<Texture2D>("Editor Arts/Color Editor/NoPreview");
    }

    private void InitTexture(){
        colorTex = new Texture2D(1, 1);
        colorTex.SetPixel(0, 0, clr);
        colorTex.Apply();
    }

    private void DrawLayout(){
        colorTexRect.x = 10;
        colorTexRect.y = 10;
        colorTexRect.width = 180;
        colorTexRect.height = 180;

        InitTexture();

        GUI.DrawTexture(colorTexRect, noPreviewTex);
        GUI.DrawTexture(colorTexRect, colorTex);
        GUIContent content = new GUIContent();
        content.text = "";
        clr = EditorGUI.ColorField(new Rect(150, 195, 40, 20), content, clr, true, false, false);
    }

    private void DrawSliders(){
        GUI.Label(new Rect(10, 220, 20, 20), "R :");
        GUI.Label(new Rect(10, 242, 20, 20), "G :");
        GUI.Label(new Rect(10, 264, 20, 20), "B :");
        GUI.Label(new Rect(10, 286, 20, 20), "A :");

        clr.r = GUI.HorizontalSlider(new Rect(35, 220, 120, 10), clr.r, 0f, 1f);
        clr.g = GUI.HorizontalSlider(new Rect(35, 242, 120, 10), clr.g, 0f, 1f);
        clr.b = GUI.HorizontalSlider(new Rect(35, 264, 120, 10), clr.b, 0f, 1f);
        clr.a = GUI.HorizontalSlider(new Rect(35, 286, 120, 10), clr.a, 0f, 1f);

        clr.r = (float)EditorGUI.IntField(new Rect(160, 222, 30, 20), Mathf.RoundToInt(clr.r * 255)) / 255f;
        clr.g = (float)EditorGUI.IntField(new Rect(160, 244, 30, 20), Mathf.RoundToInt(clr.g * 255)) / 255f;
        clr.b = (float)EditorGUI.IntField(new Rect(160, 264, 30, 20), Mathf.RoundToInt(clr.b * 255)) / 255f;
        clr.a = (float)EditorGUI.IntField(new Rect(160, 286, 30, 20), Mathf.RoundToInt(clr.a * 255)) / 255f;
    }

    private void DrawCodeFields(){
        currentColorHex = Utility.GetStringFromColor(clr, true);
        currentColorRGBA = "(" + clr.r.ToString("F2").Replace(",", ".") + "f, "
                               + clr.g.ToString("F2").Replace(",", ".") + "f, "
                               + clr.b.ToString("F2").Replace(",", ".") + "f, "
                               + clr.a.ToString("F2").Replace(",", ".") + "f)";
        EditorGUI.TextField(new Rect(10, 312, 180, 20), currentColorHex);
        EditorGUI.TextField(new Rect(10, 334, 180, 20), currentColorRGBA);
    }

    private void OnGUI() {
        DrawLayout();
        DrawSliders();
        DrawCodeFields();
    }
}