using UnityEngine;

[CreateAssetMenu(menuName = "Level Color Data")]
public class LevelColors : ScriptableObject
{
    [Space(10f), Header("Colors")]
    public Color light;
    public Color dark;
    public Color light_Obstacle;
    public Color dark_Obstacle;

    [Space(10f), Header("Materials")]
    public Material lightMat;
    public Material darkMat;
    public Material light_ObstacleMat;
    public Material dark_ObstacleMat;

    [Space(10f), Header("Sky")]
    public Material skyMaterial;
    public Material TargetSky;

    public void ChangeColors()
    {
        lightMat.SetColor("_MainColor", light);
        darkMat.SetColor("_MainColor", dark);

        light_ObstacleMat.SetColor("_MainColor", light_Obstacle);
        dark_ObstacleMat.SetColor("_MainColor", dark_Obstacle);

        skyMaterial.CopyPropertiesFromMaterial(TargetSky);
    }

    public void ChangePlatformsColor()
    {
        lightMat.SetColor("_MainColor", light);
        darkMat.SetColor("_MainColor", dark);
    }

    public void ChangeObstaclesColor()
    {
        light_ObstacleMat.SetColor("_MainColor", light_Obstacle);
        dark_ObstacleMat.SetColor("_MainColor", dark_Obstacle);
    }

    public void ChangeSkybox()
    {
        skyMaterial.CopyPropertiesFromMaterial(TargetSky);
    }
}
