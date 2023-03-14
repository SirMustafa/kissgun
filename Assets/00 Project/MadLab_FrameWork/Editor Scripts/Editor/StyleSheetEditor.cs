using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI;
using UnityEditor.UIElements;

public class StyleSheetEditor : EditorWindow {

    [MenuItem("ML Framework/StyleSheet")]
    private static void ShowWindow() {
        var window = GetWindow<StyleSheetEditor>();
        window.titleContent = new GUIContent("StyleSheet");
        window.Show();
    }

    private void OnGUI() {
        
    }
}