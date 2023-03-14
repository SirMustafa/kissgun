using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class EnemyArmor
{
    [FoldoutGroup("$name")] public string name = "New Armor";
    [FoldoutGroup("$name")] public GameObject model;
    [FoldoutGroup("$name")] public GameObject[] colliderObjects;
    [FoldoutGroup("$name"), OnValueChanged("SetArmor")] public bool isArmored = false;

    public void SetArmor()
    {
        if (model)
            model.SetActive(isArmored);
        
        if (colliderObjects.Length > 0)
            foreach (GameObject coll in colliderObjects)
                coll.tag = isArmored ? "Untagged" : "Body";
    }
}