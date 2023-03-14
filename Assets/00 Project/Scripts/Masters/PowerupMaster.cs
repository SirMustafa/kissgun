using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Powerup
{
    public string name;
    public float timer;
    public PickupItem data;
    public simpleDelegate start;
    public simpleDelegate end;

    bool activation = false;

    public void SetupData()
    {
        data.GetInfo(out start, out end);
    }

    public void UpdatePowerup()
    {
        if (timer == 0)
            return;

        if (!activation)
        {
            StartPowerup();
        }

        timer -= Time.unscaledDeltaTime;
        timer = Mathf.Clamp(timer, 0f, 100f);

        if (activation && timer == 0)
        {
            EndPowerup();
        }
    }

    private void StartPowerup()
    {
        start.Invoke();
        activation = true;
    }
    
    private void EndPowerup()
    {
        end.Invoke();
        activation = false;
    }
}

public class PowerupMaster : MonoBehaviour
{
    #region Singleton
    public static PowerupMaster instance = null;
    private void Awake() 
    {
        instance = this;
        foreach(Powerup powerup in powerUps)
        {
            powerup.SetupData();
        }
    }

    #endregion

    [SerializeField] private List<Powerup> powerUps = new List<Powerup>();

    private void LateUpdate() 
    {
        foreach (Powerup powerup in powerUps)
        {
            powerup.UpdatePowerup();
        }
    }

    public void UpdatePowerupTime(string tag, float duration)
    {
        int ID = 0;
        for (int i = 0; i < powerUps.Count; i++)
        {
            if (powerUps[i].name == tag)
            {
                ID = i;
                break;
            }
        }

        powerUps[ID].timer += duration;
    }
}