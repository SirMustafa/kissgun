using MoreMountains.NiceVibrations;

public static class VibrationMaster
{
    public static void Haptic(HapticTypes type)
    {
        if (DataBase.vibration)
            MMVibrationManager.Haptic(type);
    }
}