using System.Collections;
using System.Collections.Generic;
using MadLab.Editor.TextureEditor;
using UnityEngine.UIElements;
using UnityEditor;

namespace MadLab.Editor{
    public class ML_InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<ML_InspectorView, VisualElement.UxmlTraits> { }

        UnityEditor.Editor editor;
        
        public ML_InspectorView(){

        }

        internal void UpdateSelection(NodeView nodeView)
        {
            Clear();
            UnityEngine.Object.DestroyImmediate(editor);
            editor = UnityEditor.Editor.CreateEditor(nodeView.node);
            IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
            Add(container);
        }
    }
}
