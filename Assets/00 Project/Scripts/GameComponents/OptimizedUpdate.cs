using UnityEngine;

public enum updateOptions
{
    Update,
    FixedUpdate,
    LateUpdate
}

public class OptimizedUpdate : MonoBehaviour
{
    [Header("UpdateType")]
    [SerializeField] protected updateOptions updateType;

    /// <summary>
    /// Optimize update edilmesi için oyun başladığında setup yapınız.
    /// </summary>
    protected void SetupOptimizedUpdate()
    {
        UpdateMaster.instance.AddToCustomUpdateList(this, updateType);
    }

    /// <summary>
    /// Optimize bir şekilde bir manager tarafından update etmek için çağırın.
    /// </summary>
    public virtual void UpdateMe(float deltaTime) { }

    public void SetUpdateType(updateOptions type)
    {
        updateType = type;
    }
}
