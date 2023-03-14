using UnityEngine;

[CreateAssetMenu(fileName = "ExpandUp", menuName = "Trambolin Runner/ExpandUp", order = 2)]
public class ExpandUp : PickupItem
{
    [Range(1f, 5f)] public float expandValue = 2.5f;
    [Range(0f, 5f)] public float deadTime = 2.5f;
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
    }

    public void EndPowerup()
    {
    }
}