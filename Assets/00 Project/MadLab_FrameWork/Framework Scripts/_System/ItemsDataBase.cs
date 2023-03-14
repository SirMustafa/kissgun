using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class ItemData
{
    [HideLabel()]
    [FoldoutGroup("$name")]
    [PreviewField(60, ObjectFieldAlignment.Left)]
    [HorizontalGroup("$name/Row", 60), VerticalGroup("$name/Row/Left")]
    public Sprite icon;

    [FoldoutGroup("$name")]
    [VerticalGroup("$name/Row/Right"), LabelWidth(-54)]
    public string name = "New Item";

    [FoldoutGroup("$name")]
    [VerticalGroup("$name/Row/Right"), LabelWidth(-54)]
    public bool forSale = false;

    [FoldoutGroup("$name")]
    [VerticalGroup("$name/Row/Right"), LabelWidth(-54), ShowIf("$forSale")]
    public int price;

    [FoldoutGroup("$name")]
    [VerticalGroup("$name/Row/Right"), LabelWidth(-54)]
    public CustomVariable[] itemVariables;

    [VerticalGroup("$name/Row/Right"), LabelWidth(-54)]
    public bool isHave = false;
}

[System.Serializable]
public class ItemCategory
{
    [FoldoutGroup("$categoryName")] public string categoryName = "New Category";
    [FoldoutGroup("$categoryName")] public List<ItemData> items;
}

public class ItemsDataBase : ScriptableObject
{
    public List<ItemCategory> itemCategories;

    public ItemData GetItem(string category, string name)
    {
        ItemCategory targetCategory = itemCategories.Find(x => x.categoryName == category);
        ItemData item = targetCategory.items.Find(x => x.name == name);
        return item;
    }

    public ItemData GetItem(int category, int item)
    {
        ItemData result;
        try
        {
            result = itemCategories[category].items[item];
        }
        catch
        {
            result = null;
        }
        return result;
    }

    public int GetItemCount(int category)
    {
        return itemCategories[category].items.Count;
    }

    public void OwnItem(int category, int index){

        itemCategories[category].items[index].isHave = true;
    }
}
