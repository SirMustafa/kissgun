using UnityEngine;
using UnityEngine.SceneManagement;
using MadLab;

public static class DataBase
{
    public static bool DeveloperMode
    {
        get
        {
#if UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
    }

    #region Collectible Items

    /// <summary>
    /// Oyuncunun sahip olduğu elmaslar
    /// </summary>
    public static int gems
    {
        get
        {
            if (PlayerPrefs.HasKey("Gems"))
                PlayerPrefs.SetInt("Gems", 0);

            return PlayerPrefs.GetInt("Gems");
        }
        set
        {
            gems = value;
        }
    }

    public static int coins
    {
        get
        {
            if (!PlayerPrefs.HasKey("Coins"))
                PlayerPrefs.SetInt("Coins", 0);

            return PlayerPrefs.GetInt("Coins");
        }
        set
        {
            PlayerPrefs.SetInt("Coins", value);
        }
    }

    #endregion

    #region Level Datas

    /// <summary>
    /// Toplam level sayısı
    /// </summary>
    public static int totalLevels
    {
        get
        {
            return SceneManager.sceneCountInBuildSettings - 1;
        }
    }

    /// <summary>
    /// Açık levelin build indexi
    /// </summary>
    public static int currentLevelIndex
    {
        get
        {
            return SceneManager.GetActiveScene().buildIndex;
        }
    }

    /// <summary>
    /// Sonraki Levelin build indexi
    /// </summary>
    public static int nextLevelIndex
    {
        get
        {
            return currentLevelIndex != totalLevels ? currentLevelIndex + 1 : MadLab.GameSettingsData.loopStartLevel;
        }
    }

    /// <summary>
    /// Önceki Levelin build indexi
    /// </summary>
    public static int prevLevelIndex
    {
        get
        {
            return currentLevelIndex != MadLab.GameSettingsData.loopStartLevel ? currentLevelIndex - 1 : totalLevels;
        }
    }

    public static int SavedCurrentLevel
    {
        get
        {
            if (!PlayerPrefs.HasKey("Current Level"))
                PlayerPrefs.SetInt("Current Level", 1);
            return PlayerPrefs.GetInt("Current Level");
        }
    }

    /// <summary>
    /// Level Numarası
    /// </summary>
    public static int currentLevelNo
    {
        get
        {
            return currentLevelIndex
                    + ((totalLevels - MadLab.GameSettingsData.loopStartLevel + 1) * gameCompletedCount);
        }
    }

    /// <summary>
    /// Şimdi çalışan levelin oynanış sayısı (Döngüdekiler ayrı hesaplanıyor)
    /// </summary>
    public static int currentLevelPlayCount
    {
        get
        {
            return PlayerPrefs.HasKey("PlayCount_Level" + currentLevelNo) ? PlayerPrefs.GetInt("PlayCount_Level" + currentLevelNo) : 0;
        }
    }

    #endregion



    /// <summary>
    /// Oyunun bütün bölümleri kaç kere bitirilmiş
    /// </summary>
    public static int gameCompletedCount
    {
        get
        {
            return PlayerPrefs.HasKey("Games Completed") ? PlayerPrefs.GetInt("Games Completed") : 0;
        }
    }

    #region Settings Data

    /// <summary>
    /// Titreşimin aktifliği
    /// </summary>
    public static bool vibration
    {
        get
        {
            bool result = true;
            if (PlayerPrefs.HasKey("Vibration Settings"))
            {
                result = PlayerPrefs.GetInt("Vibration Settings") == 1 ? true : false;
            }
            else
            {
                PlayerPrefs.SetInt("Vibration Settings", 1);
            }
            return result;
        }
    }

    /// <summary>
    /// Seslerin aktifliği
    /// </summary>
    public static bool sound
    {
        get
        {
            bool result = true;
            if (PlayerPrefs.HasKey("Sound Settings"))
            {
                result = PlayerPrefs.GetInt("Sound Settings") == 1 ? true : false;
            }
            else
            {
                PlayerPrefs.SetInt("Sound Settings", 1);
            }
            return result;
        }
    }

    #endregion


}