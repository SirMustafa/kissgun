using System.Collections.Generic;
using UnityEngine;

public class HorizontalGroup : MonoBehaviour
{
    [SerializeField] private Vector2 childSize;
    List<RectTransform> childs = new List<RectTransform>();
    RectTransform myRect = null;
    
    private void Awake() {
        myRect = GetComponent<RectTransform>();
        SetupChilds();

        if(childSize.x == 0 || childSize.y == 0)
            childSize = childs[0].sizeDelta;
        
        UpdateGroup();
    }

    void SetupChilds()
    {
        childs = new List<RectTransform>();
        RectTransform[] rects = transform.GetComponentsInChildren<RectTransform>();
        foreach(RectTransform rect in rects)
        {
            if(rect != myRect)
            {
                childs.Add(rect);
                rect.sizeDelta = childSize;
            }
        }
    }

    public void UpdateGroup()
    {
        SetupChilds();
        float widthPerObject = (myRect.rect.width -  (2 * (childSize.x * 0.5f))) / (childs.Count + 1);
        float start = -(myRect.rect.width / 2f) + (childSize.x * 0.5f);
        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].anchoredPosition = new Vector2(start + (widthPerObject * (i + 1)), 0f); 
        }
    }
}
