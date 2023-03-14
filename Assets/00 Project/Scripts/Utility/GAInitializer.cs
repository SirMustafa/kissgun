using UnityEngine;
//using GameAnalyticsSDK;

public class GAInitializer : MonoBehaviour
{
    private void Awake() {
        #if UNITY_ANDROID
        Application.targetFrameRate = 60;
        #elif UNITY_IOS
        Application.targetFrameRate = 60;
        #endif
        //GameAnalytics.Initialize();
    }
}
