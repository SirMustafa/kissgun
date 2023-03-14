using UnityEngine;
using UnityEngine.SceneManagement;

namespace MadLab
{
    public static class GameSettingsData
    {
        #if UNITY_EDITOR
        public static ML_Resources resources = UnityEditor.AssetDatabase.LoadAssetAtPath<ML_Resources>("Assets/00 Project/MadLab_FrameWork/Data/ML_Resources.asset");
        #endif

        public static GameSettings gameSettings = Resources.Load<GameSettings>("Game Settings/Game Settings");

        public static Globalizer globalizer = Resources.Load<Globalizer>("Game Settings/Globalizer");

        public static int totalLevels => SceneManager.sceneCountInBuildSettings - 1;

        public static bool coinsInGame = gameSettings.CoinsInGame();

        public static Sprite coinSprite = gameSettings.CoinSprite();

        public static int loopStartLevel = gameSettings.LoopStartLevel();
    }
}
