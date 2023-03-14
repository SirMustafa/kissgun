using UnityEngine;

[CreateAssetMenu(fileName = "SpeedUp", menuName = "Trambolin Runner/SpeedUp", order = 2)]
public class SpeedUp : PickupItem
{
    [Range(1f, 5f)] public float timeScale = 2.5f;
    [Range(0f, 5f)] public float deadTime = 2.5f;

    [SerializeField] private DG.Tweening.Ease easeAnim;
    public string powerUpTag;

    public override void PickupMe(simpleDelegate myDelegate = null)
    {
        if(myDelegate != null)
            myDelegate.Invoke();

        PowerupMaster.instance.UpdatePowerupTime(powerUpTag, deadTime);
    }

    public override void GetInfo(out simpleDelegate start, out simpleDelegate end)
    {
        start = StartPowerup;
        end = EndPowerup;
    }

    public void StartPowerup()
    {
        TimeMaster.SetTimeScale(timeScale, 1f, easeAnim);
        CameraFollow.instance.SetFOV(70f, true);
    }

    public void EndPowerup()
    {
        TimeMaster.SetTimeScale(1f, 1f, easeAnim);
        CameraFollow.instance.SetFOV(60f, true);
    }
}