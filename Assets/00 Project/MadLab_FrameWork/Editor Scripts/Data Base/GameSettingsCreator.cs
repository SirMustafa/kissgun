#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class GameSettingsCreator : Editor
{
    #region Game Settings

    public static GameSettings gameSettings;
    public static string directory_GameSettings = "Assets/00 Project/Resources/Game Settings";

    static GameSettingsCreator()
    {
        gameSettings = AssetDatabase.LoadAssetAtPath<GameSettings>(directory_GameSettings + "/Game Settings.asset");
        if (gameSettings)
        {
            gameSettings.Init();
        }
    }

    [MenuItem("ML Framework/Create/Game Settings", false, 2)]
    static void CreateSettingsDataBase()
    {
        if (gameSettings)
        {
            Debug.LogWarning("You already have Game Settings Data!");
            return;
        }
        else
        {
            gameSettings = AssetDatabase.LoadAssetAtPath<GameSettings>(directory_GameSettings + "/Game Settings.asset");
            if (gameSettings)
                return;
        }

        gameSettings = ScriptableObject.CreateInstance<GameSettings>();
        if (!AssetDatabase.IsValidFolder(directory_GameSettings))
            Directory.CreateDirectory(directory_GameSettings);

        AssetDatabase.CreateAsset(gameSettings, directory_GameSettings + "/Game Settings.asset");
        Debug.Log("[Game Settings] database successfully created.");
    }

    #endregion

    #region Globalizer Creator

    public static Globalizer globalizerData;
    public static string directory_Globalizer = "Assets/00 Project/Resources/Game Settings";

    [MenuItem("ML Framework/Create/Globalizer Data", false, 1)]
    static void CreateGlobalizer()
    {
        if (globalizerData)
        {
            Debug.LogWarning("You already have Globalizer Data!");
            return;
        }
        else
        {
            globalizerData = AssetDatabase.LoadAssetAtPath<Globalizer>(directory_Globalizer + "/Globalizer.asset");
            if (globalizerData)
                return;
        }

        globalizerData = ScriptableObject.CreateInstance<Globalizer>();

        if (!AssetDatabase.IsValidFolder(directory_Globalizer))
            Directory.CreateDirectory(directory_Globalizer);

        AssetDatabase.CreateAsset(globalizerData, directory_Globalizer + "/Globalizer.asset");
        Debug.Log("[Globalizer] database successfully created.");
    }

    #endregion

    #region Items Database Creator

    public static ItemsDataBase items_DataBase;
    public static string directory_itemsDatabase = "Assets/00 Project/Resources/Game Settings";

    [MenuItem("ML Framework/Create/Items Database", false, 0)]
    static void Create_DataBaseForItems()
    {
        if (items_DataBase)
        {
            Debug.LogWarning("You already have ItemsDataBase!");
            return;
        }
        else
        {
            items_DataBase = AssetDatabase.LoadAssetAtPath<ItemsDataBase>(directory_itemsDatabase + "/ItemsDataBase.asset");
            if (items_DataBase)
                return;
        }

        items_DataBase = ScriptableObject.CreateInstance<ItemsDataBase>();

        if (!AssetDatabase.IsValidFolder(directory_itemsDatabase))
            Directory.CreateDirectory(directory_itemsDatabase);

        AssetDatabase.CreateAsset(items_DataBase, directory_itemsDatabase + "/ItemsDataBase.asset");
        Debug.Log("[ItemsDataBase] successfully created.");
    }

    #endregion

    [MenuItem("ML Framework/Create/SDK Master", false, 3)]
    static void CreateSDK_Master()
    {
        SDK_Master sdkMaster = FindObjectOfType<SDK_Master>();
        if (sdkMaster){
            sdkMaster.FindEvents();
        } else{
            GameObject sdkMasterObject = new GameObject("SDK Master");
            sdkMaster = sdkMasterObject.AddComponent<SDK_Master>();
            Debug.Log("SDK Master created successfully!");
            sdkMaster.FindEvents();
            EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }
    }
}
#endif