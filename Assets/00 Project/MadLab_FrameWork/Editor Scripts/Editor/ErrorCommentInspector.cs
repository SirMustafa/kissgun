using UnityEngine;
using UnityEditor;
using MadLab.Utilities;

namespace MadLab.Editor {

    [CustomEditor(typeof(ML_ErrorComment))]
    public class ErrorCommentInspector : UnityEditor.Editor {
        
        private ML_ErrorComment script { get { return target as ML_ErrorComment; }}
		private GUIStyle style = new GUIStyle();

        private static Color pro = Utility.GetColorFromString("FF534A");
		private static Color free = new Color(0.2f, 0.3f, 0.1f, 1f);
        
		public override void OnInspectorGUI() {
			if (serializedObject == null) return;
			
			style.wordWrap = true;
			style.normal.textColor = EditorGUIUtility.isProSkin? pro: free;
			
			serializedObject.Update();
			EditorGUILayout.Space();
			
			string text = EditorGUILayout.TextArea(script.text, style);
			if (text != script.text) {
				Undo.RecordObject(script, "Edit Comments");
				script.text = text;
			}
			
			EditorGUILayout.Space();
			
			serializedObject.ApplyModifiedProperties();
		}
	}
}