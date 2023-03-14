using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected float shootRate = 0.3f;
    protected float nextShoot = 0f;
    protected int dmg = 1;

    public virtual void Attack(Vector3 direction = new Vector3())
    {

    }
    
    public virtual void Reload()
    {
        
    }
}