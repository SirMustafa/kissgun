using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Item : MonoBehaviour
{
    [FoldoutGroup("Item Settings"), SerializeField] private Ease collectEase = Ease.InBack;
    [FoldoutGroup("Item Settings"), SerializeField] private Ease spawnEase = Ease.OutBack;
    [FoldoutGroup("Item Settings"), SerializeField] private PickupItem[] itemData;
    [FoldoutGroup("Item Settings"), SerializeField] private string effectTag;
    [FoldoutGroup("Item Settings"), SerializeField] private bool setEffectColor = false;
    [FoldoutGroup("Item Settings"), ShowIf("$setEffectColor"), SerializeField] private Color effectColor = Color.white;
    [FoldoutGroup("Pickup Event"), SerializeField] private UnityEvent pickupEvent;
    private Collider coll;

    [ToggleGroup("resetAfterPickup", "Reset After Pickup"), SerializeField] bool resetAfterPickup = false;
    [ToggleGroup("resetAfterPickup", "Reset After Pickup"), SerializeField] float resetTime = 2f;

    private void Start() {
        coll = GetComponent<Collider>();
    }

    public void Setup(float resetTime = 2f, bool resetAfterPickup = false)
    {
        //GetComponent<SineMoveAnimation>().Setup();
        GetComponent<RotateAnimation>().Setup();
        if(resetAfterPickup)
        {
            this.resetAfterPickup = resetAfterPickup;
            this.resetTime = resetTime;
        }
    }

    public float GetScore(){
        return itemData.Length > 0 ? itemData[0].score : 0f;
    }

    public bool IsAvailable(){
        return !picked;
    }

    bool picked = false;
    public void PickupMe(int Count = 1, simpleDelegate eventWhenPickup = null)
    {
        if (picked)
            return;

        picked = true;

        if(itemData.Length > 0)
        {
            int ID = Random.Range(0, itemData.Length);
            for(int i = 0; i < Count; i++)
            {
                itemData[ID].PickupMe(eventWhenPickup);
            }
        }
        for (int i = 0; i < Count; i++)
        {
            pickupEvent.Invoke();
        }

        HideMe();
        if (resetAfterPickup)
            Invoke("ShowMe", resetTime);
    }

    public void HideMe()
    {
        coll.enabled = false;
        
        GameObject effect = ObjectPooler.Instance.SpawnFromPool(effectTag, transform.position, Quaternion.identity, false);
        if (setEffectColor){
            var effectMain = effect.GetComponent<ParticleSystem>().main;
            effectMain.startColor = effectColor;
        }
        effect.SetActive(true);

        transform.DOScale(Vector3.zero, 0.2f).
                    SetEase(collectEase);
    }

    public void ShowMe()
    {
        picked = false;

        transform.DOScale(Vector3.one, 0.4f).
                    SetEase(spawnEase).
                    OnComplete(()=> { picked = false; coll.enabled = true; });
    }
}
