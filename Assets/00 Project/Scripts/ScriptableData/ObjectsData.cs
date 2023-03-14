using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace MadLab
{
    [CreateAssetMenu(fileName = "Objects Data", menuName = "ML_Framework/Objects Data", order = 0)]
    public class ObjectsData : SerializedScriptableObject
    {
        [SerializeField] private Dictionary<string, Object> objects;

        public Object GetObject(string name)
        {
            return objects[name];
        }
    }
}