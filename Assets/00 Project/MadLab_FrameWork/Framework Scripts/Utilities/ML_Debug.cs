#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Sirenix.OdinInspector;

namespace MadLab
{
    public static class ML_Debug
    {
        public static GameObject DebugCanvas => AssetDatabase.LoadAssetAtPath<GameObject>("Assets/00 Project/MadLab_FrameWork/Prefabs/ML_Debug.prefab");
        public static ML_DebugCanvas debugCanvasInScene;

        public static Sprite background {
            get {     
                return AssetDatabase.LoadAssetAtPath<Sprite>(EditorPrefs.GetString("Background Sprite"));
            }
            set {
                EditorPrefs.SetString("Background Sprite", AssetDatabase.GetAssetPath(value));
            }
        }
        public static Sprite checkMark {
            get {     
                return AssetDatabase.LoadAssetAtPath<Sprite>(EditorPrefs.GetString("Checkmark Sprite"));
            }
            set {
                EditorPrefs.SetString("Checkmark Sprite", AssetDatabase.GetAssetPath(value));
            }
        }
        public static Sprite knob {
            get {     
                return AssetDatabase.LoadAssetAtPath<Sprite>(EditorPrefs.GetString("Knob Sprite"));
            }
            set {
                EditorPrefs.SetString("Knob Sprite", AssetDatabase.GetAssetPath(value));
            }
        }
        

        public static void Initialize()
        {
            if (debugCanvasInScene != null)
                return;

            debugCanvasInScene = MonoBehaviour.Instantiate(DebugCanvas, Vector3.zero, Quaternion.identity).GetComponent<ML_DebugCanvas>();
            debugCanvasInScene.SetupResources(background, checkMark, knob);
        }

        public static void DrawText(string name, Rect rect, Anchors anchor, string text, Color color, int fontSize = 14, FontStyle style = FontStyle.Normal)
        {
            Initialize();
            debugCanvasInScene.DrawText(name, rect, anchor, text, color, fontSize, style);
        }

        public static void DrawSlider(string name, Rect rect, Anchors anchor, float value, float maxValue = 1f, bool interactable = true)
        {
            Initialize();
            debugCanvasInScene.DrawSlider(name, rect, anchor, value, maxValue, interactable);
        }
        
        public static void DrawToggle(string name, Rect rect, Anchors anchor, bool isOn, string label)
        {
            Initialize();
            debugCanvasInScene.DrawToggle(name, rect, anchor, isOn, label);
        }
    }    
}

#endif
