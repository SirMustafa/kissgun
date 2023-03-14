using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class UpdateOption
{
    private List<OptimizedUpdate> updateList = new List<OptimizedUpdate>();

    public void AddToUpdateList(OptimizedUpdate OUobject) {
        updateList.Add(OUobject);
    }

    public void UpdateAllObjects(float interval, float deltaTime)
    {
        float fps = 1 / deltaTime;
        float newDeltaTime = 1 / (fps / interval);

        List<OptimizedUpdate> temp = new List<OptimizedUpdate>(updateList);
        if (temp.Count > 0 && Time.frameCount % interval == 0)
        {
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].gameObject.activeInHierarchy)
                    temp[i].UpdateMe(newDeltaTime);
            }
        }
    }
}

public class UpdateMaster : MonoBehaviour
{
    public static UpdateMaster instance = null;

    private void Awake()
    {
        instance = this;
    }

    private UpdateOption normalUpdate = new UpdateOption();
    private UpdateOption fixedUpdate = new UpdateOption();
    private UpdateOption lateUpdate = new UpdateOption();

    [ProgressBar(0f, 10, 1f, 0.8f, 0f), SerializeField] private int updateInterval = 1;
    [ProgressBar(0f, 10, 0f, 1f, 1f), SerializeField] private int fixedUpdateInterval = 1;

    private void Update()
    {
        normalUpdate.UpdateAllObjects(updateInterval, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        fixedUpdate.UpdateAllObjects(fixedUpdateInterval, Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        lateUpdate.UpdateAllObjects(updateInterval, Time.deltaTime);
    }

    public void AddToCustomUpdateList(OptimizedUpdate targetObject, updateOptions type)
    {
        switch(type)
        {
            case updateOptions.Update:
                normalUpdate.AddToUpdateList(targetObject);
                break;

            case updateOptions.FixedUpdate:
                fixedUpdate.AddToUpdateList(targetObject);
                break;

            case updateOptions.LateUpdate:
                lateUpdate.AddToUpdateList(targetObject);
                break;
        }
    }
}