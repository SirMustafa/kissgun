using UnityEngine;

[CreateAssetMenu(fileName = "PickupItem", menuName = "Trambolin Runner/PickupItem", order = 0)]
public class PickupItem : ScriptableObject 
{
    public float score = 0f;

    public virtual void PickupMe(simpleDelegate myDelegate = null)
    {
        if (myDelegate != null)
            myDelegate.Invoke();
    }

    public virtual void GetInfo(out simpleDelegate start, out simpleDelegate end)
    {
        start = null;
        end = null;
    }
}