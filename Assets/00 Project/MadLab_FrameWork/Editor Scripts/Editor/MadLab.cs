#if UNITY_EDITOR
using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

namespace MadLab.Editor {
    [InitializeOnLoad]
    public class MadLab : UnityEditor.Editor
    {
        // var assembly = Assembly.LoadFile("C:\path_to_your_exe\YourExe.exe");
        /*
        foreach (var type in assembly.GetTypes())
        {
            Console.WriteLine($"Class {type.Name}:");
            Console.WriteLine($"  Namespace: {type.Namespace}");
            Console.WriteLine($"  Full name: {type.FullName}");

            Console.WriteLine($"  Methods:");
            foreach (var methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                Console.WriteLine($"    Method {methodInfo.Name}");

                if (methodInfo.IsPublic)
                    Console.WriteLine($"      Public");

                if (methodInfo.IsFamily)
                    Console.WriteLine($"      Protected");

                if (methodInfo.IsAssembly)
                    Console.WriteLine($"      Internal");

                if (methodInfo.IsPrivate)
                    Console.WriteLine($"      Private");

            Console.WriteLine($"      ReturnType {methodInfo.ReturnType}");
            Console.WriteLine($"      Arguments {string.Join(", ", methodInfo.GetParameters().Select(x => x.ParameterType))}");
        }*/
    }

    [InitializeOnLoad]
    public class ML_Settings : OdinMenuEditorWindow
    {
        public static ML_Resources resources;
        private static string dataFolderPath = "Assets/00 Project/MadLab_FrameWork/Data";

        private void OnEnable() {
            resources = AssetDatabase.LoadAssetAtPath<ML_Resources>(dataFolderPath + "/ML_Resources.asset");
        }


        [MenuItem("ML Framework/Settings", false, 30)]
        private static void OpenWindow()
        {
            if (resources == null)
                CreateResourcesDataBase();

            EditorWindow myWindow = GetWindow<ML_Settings>("ML Framework Settings");
            myWindow.minSize = new Vector2(500, 600);
        }

        static void CreateResourcesDataBase()
        {
            if (resources)
            {
                return;
            }
            else
            {
                resources = AssetDatabase.LoadAssetAtPath<ML_Resources>(dataFolderPath + "/ML_Resources.asset");
                if (resources)
                    return;
            }

            resources = ScriptableObject.CreateInstance<ML_Resources>();
            if (!AssetDatabase.IsValidFolder(dataFolderPath))
                Directory.CreateDirectory(dataFolderPath);

            AssetDatabase.CreateAsset(resources, dataFolderPath + "/ML_Resources.asset");
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            tree.AddAssetAtPath("Game Settings", GameSettingsCreator.directory_GameSettings + "/Game Settings.asset", typeof(GameSettings));
            tree.AddAssetAtPath("Globalizer", GameSettingsCreator.directory_GameSettings + "/Globalizer.asset", typeof(Globalizer));
            tree.AddAssetAtPath("Resources", dataFolderPath + "/ML_Resources.asset", typeof(ML_Resources));

            return tree;
        }
    }
}

#endif