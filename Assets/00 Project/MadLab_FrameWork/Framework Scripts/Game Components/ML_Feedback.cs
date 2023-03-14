using UnityEngine;
using UnityAtoms.BaseAtoms;
using Sirenix.OdinInspector;
using MoreMountains.NiceVibrations;

public enum VibrationTypes
{
    Nope,
    Peek,
    Pop
}

public class ML_Feedback : MonoBehaviour
{


    [ToggleGroup("UseEvent"), SerializeField]
    private bool UseEvent;
    [ToggleGroup("UseEvent"), SerializeField]
    private BoolEvent targetEvent;

    [ToggleGroup("ShakeFeedBack"), SerializeField]
    private bool ShakeFeedBack = false;
    [ToggleGroup("ShakeFeedBack"), SerializeField]
    private float magnitude;
    [ToggleGroup("ShakeFeedBack"), SerializeField]
    private float roughness;
    [ToggleGroup("ShakeFeedBack"), SerializeField]
    private float fadeIn;
    [ToggleGroup("ShakeFeedBack"), SerializeField]
    private float fadeOut;
    [ToggleGroup("ShakeFeedBack"), MinMaxSlider(0f, 75f, true), SerializeField]
    private Vector2 radius = new Vector2(25f, 50f);
    [ToggleGroup("ShakeFeedBack"), SerializeField]
    private bool shakeOnStart;

    [ToggleGroup("HapticFeedBack"), SerializeField]
    private bool HapticFeedBack = false;
    [ToggleGroup("HapticFeedBack"), Title("Haptic"), SerializeField]
    private HapticTypes vibration;
/*
    private string VibrationInfo()
    {
        string result = "";
        switch(vibration)
        {
            case VibrationTypes.Nope:
                result = "3 small vibrations [5,5,5]";
                break;
            case VibrationTypes.Peek:
                result = "Small peek vibration [25]";
                break;
            case VibrationTypes.Pop:
                result = "Tiny pop vibration [15]";
                break;
        }
        return result;
    }*/
   
    [ToggleGroup("HapticFeedBack"), SerializeField]
    private bool vibrateOnStart;

    [ToggleGroup("gizmos"), SerializeField] private bool gizmos;
    [ToggleGroup("gizmos"), SerializeField] private Color gizmosColorOut = new Color(1f, 0, 0, 0.5f);
    [ToggleGroup("gizmos"), SerializeField] private Color gizmosColorIn = new Color(0, 1f, 1f, 0.5f);

    private void Start() {
        if(shakeOnStart)
            Shake();
        if(vibrateOnStart)
            Vibrate();

        if (UseEvent)
        {
            targetEvent.Register(FeedbacksWithEvent);
        }
    }

    private void FeedbacksWithEvent()
    {
        Shake();
        Vibrate();
    }

    public void Shake()
    {
        if(ShakeFeedBack)
        {
            float dis = Mathf.Clamp(Vector2.Distance(FPSCamera.instance.transform.position, transform.position), radius.x, radius.y);
            float magMultiplier = Mathf.Abs(radius.y - radius.x) == 0 ? 1f : Mathf.Clamp01((radius.y - dis) / (radius.y - radius.x));
            EZCameraShake.CameraShaker.Instance.ShakeOnce(magnitude * magMultiplier, roughness * magMultiplier, fadeIn, fadeOut);
        }
    }

    public void Vibrate()
    {
        if (HapticFeedBack && DataBase.vibration)
            MMVibrationManager.Haptic(vibration);
    }

    public void VibrateAndShake()
    {
        Vibrate();
        Shake();
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected() {
        if (!gizmos)
            return;
        Gizmos.color = gizmosColorOut;
        Gizmos.DrawSphere(transform.position, radius.y);
        
        Gizmos.color = gizmosColorIn;
        Gizmos.DrawSphere(transform.position, radius.x);
    }

#endif
}
