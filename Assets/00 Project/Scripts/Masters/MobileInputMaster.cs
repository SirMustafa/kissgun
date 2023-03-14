using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MadLab;

public class MobileInputMaster : MonoBehaviour
{
    #region Singleton
    public static MobileInputMaster instance = null;
    private void Awake() {
        instance = this;

        startType = GameSettingsData.gameSettings.GameStarterType;
        controlType = GameSettingsData.gameSettings.MobileInputType;
    }
    #endregion

    private MobileControlType controlType;
    private GameStartType startType;
    [SerializeField] private GameObject otherControls;
    [SerializeField] private Joystick joystickControl;

    private void Start() {
        if (controlType == MobileControlType.Joystick)
        {
            joystickControl.gameObject.SetActive(true);
            joystickControl.Setup(GameSettingsData.gameSettings.animatedJoystick, 
                                    GameSettingsData.gameSettings.showJoystickGraphics,
                                    GameSettingsData.gameSettings.movableJoystick,
                                    GameSettingsData.gameSettings.joystickSizeAnimation_Ease,
                                    startType);
        }
        else
        {
            otherControls.SetActive(true);
        }
    }
}
