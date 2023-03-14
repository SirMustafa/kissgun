#region Modules
using System.Collections.Generic;
using UnityEngine;
#endregion

[System.Serializable]
public class Pool
{
    public string tag;
    public GameObject prefab;
    public Transform parent;
    public int size;
}

public class ObjectPooler : MonoBehaviour
{
    #region Singleton

    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    #region Setup
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools )
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, pool.parent);
                obj.SetActive (false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }
    #endregion

    #region Event
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, bool active = true)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.Log("Pool with tag" + tag + "doesn't excist");
            return null;
        }
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn);

        objectToSpawn.SetActive(active);

        return objectToSpawn;
    }
    
    public int GetTypeCountWithName(string name)
    {
        int result = 0;
        foreach(Pool pooled in pools)
        {
            if (pooled.tag.Contains(name))
                result++;
        }
        return result;
    }
    
    #endregion
}
