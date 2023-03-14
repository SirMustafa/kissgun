using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityToolbarExtender;

namespace MadLab
{
    static class ToolbarStyles
    {
        public static readonly GUIStyle commandButtonStyle;

        static ToolbarStyles()
        {
            commandButtonStyle = new GUIStyle("Command")
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold
            };
        }
    }

    [InitializeOnLoad]
    public class ML_Toolbar
    {
        
        static ML_Resources resources => GameSettingsData.resources;
        static ML_Toolbar()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }

        static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            if(GUILayout.Button(new GUIContent(resources.prev.texture), ToolbarStyles.commandButtonStyle))
            {
                PlayerPrefs.SetInt("Current Level", DataBase.prevLevelIndex);
                ChangeScene(DataBase.prevLevelIndex);
            }

            if(GUILayout.Button(new GUIContent(resources.refresh.texture), ToolbarStyles.commandButtonStyle))
            {
                ChangeScene(DataBase.currentLevelIndex);
            }

            if(GUILayout.Button(new GUIContent(resources.next.texture), ToolbarStyles.commandButtonStyle))
            {
                PlayerPrefs.SetInt("Current Level", DataBase.nextLevelIndex);
                if (DataBase.currentLevelIndex == DataBase.totalLevels)
                    PlayerPrefs.SetInt("Games Completed", DataBase.gameCompletedCount + 1);
                ChangeScene(DataBase.nextLevelIndex);
            }
        }

        static void ChangeScene(int index)
        {
            if (!EditorApplication.isPlaying)
                return;

            if (EditorApplication.isPaused)
                EditorApplication.isPaused = false;
            SceneManager.LoadScene(index);
        }

    }


/*
    static class SceneHelper
    {
        static string sceneToOpen;

        public static void StartScene(string sceneName)
        {
            if(EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            sceneToOpen = sceneName;
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate()
        {
            if (sceneToOpen == null ||
                EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            EditorApplication.update -= OnUpdate;

            if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                // need to get scene via search because the path to the scene
                // file contains the package version so it'll change over time
                string[] guids = AssetDatabase.FindAssets("t:scene " + sceneToOpen, null);
                if (guids.Length == 0)
                {
                    Debug.LogWarning("Couldn't find scene file");
                }
                else
                {
                    string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    EditorSceneManager.OpenScene(scenePath);
                    EditorApplication.isPlaying = true;
                }
            }
            sceneToOpen = null;
        }
    }
    */

}