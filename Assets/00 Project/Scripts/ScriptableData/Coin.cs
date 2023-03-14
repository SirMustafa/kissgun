using UnityEngine;


[CreateAssetMenu(fileName = "Coin", menuName = "Trambolin Runner/Coin", order = 1)]
public class Coin : PickupItem 
{
    public int value;

    public override void PickupMe(simpleDelegate myDelegate = null)
    {
        if(myDelegate != null)
            myDelegate.Invoke();
        DataBase.coins++;
        UIMaster.instance.UpdateCoins();
    }
}