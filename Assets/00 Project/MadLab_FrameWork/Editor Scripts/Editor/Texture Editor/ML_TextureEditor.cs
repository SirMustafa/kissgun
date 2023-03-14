using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;

namespace MadLab.Editor.TextureEditor
{
    public class ML_TextureEditor : EditorWindow
    {
        ML_GraphView graphView;
        ML_InspectorView inspectorView;
        VisualElement root;
        

        [MenuItem("ML Framework/Texture Editor")]
        public static void OpenWindow()
        {
            ML_TextureEditor wnd = GetWindow<ML_TextureEditor>();
            wnd.titleContent = new GUIContent("Texture Editor");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is TextureBlueprint)
            {
                OpenWindow();
                return true;
            }

            return false;
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/00 Project/MadLab_FrameWork/Editor Scripts/Editor/Texture Editor/ML_TextureEditor.uxml");
            visualTree.CloneTree(root);


            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/00 Project/MadLab_FrameWork/Editor Scripts/Editor/Texture Editor/ML_TextureEditor.uss");
            var styleSheet2 = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/00 Project/MadLab_FrameWork/Editor Scripts/Editor/StyleSheets/Extensions/Default.uss");
            root.styleSheets.Add(styleSheet);
            root.styleSheets.Add(styleSheet);

            graphView = root.Q<ML_GraphView>();
            inspectorView = root.Q<ML_InspectorView>();
            graphView.OnNodeSelected = OnNodeSelectionChanged;
            OnSelectionChange();
        }

        private void OnGUI() {
            root.Q<Label>("TextureNameTitle").text = root.Q<TextField>("TextureNameField").text;
        }

        private void OnSelectionChange()
        {
            TextureBlueprint blueprint = Selection.activeObject as TextureBlueprint;
            if (blueprint && AssetDatabase.CanOpenForEdit(blueprint))
                graphView.PopulateView(blueprint);
        }

        void OnNodeSelectionChanged(NodeView node)
        {
            inspectorView.UpdateSelection(node);
        }
    }
}
