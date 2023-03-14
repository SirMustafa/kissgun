using System;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class AdsMaster : MonoBehaviour
{
    //public IntEvent loadLevelWithAdsEvent;
    public BoolEvent successEvent;
    public BoolEvent failEvent;
/*
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }*/

    void Start()
    {
        //loadLevelWithAdsEvent.Register(ShowAds);
        successEvent.Register(ShowAds);
        failEvent.Register(ShowAds);
    }

    public void ShowAds()
    {
        try
        {
            YsoCorp.GameUtils.YCManager.instance.adsManager.ShowInterstitial(() => {
                // TODO call the action (eg: play, restart, back, next level, ...)
            });
            Debug.Log("Show Ad");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
