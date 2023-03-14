using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

public class Gun : Weapon
{
    [SerializeField] private string bulletTag;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private ParticleSystem muzzle;
    [SerializeField] private Animator anim;

    [SerializeField] private ML_Feedback attackFeedback;
    [SerializeField] private string shootSound;

    private bool canShoot = true;
    private Quaternion targetRot;
    private Sequence rotationSeq;
    private Sequence speedSeq;

    public void SetSpeed(float speed)
    {
        float value = anim.GetFloat("Speed");
        
        if (speedSeq != null && speedSeq.IsActive())
            speedSeq.Kill();

        speedSeq = DOTween.Sequence();
        speedSeq.Append(
            DOTween.To(() => value, x => value = x, speed, 0.2f)
                .OnUpdate(() =>
                {
                    anim.SetFloat("Speed", speed);
                })
        );
    }

    public override void Attack(Vector3 hitPoint = new Vector3())
    {
        if (canShoot)
        {
            canShoot = false;

          //  RotateWeapon(hitPoint);
            
            Vector3 dir = (hitPoint - shootPoint.position).normalized;
            ObjectPooler.Instance.SpawnFromPool(bulletTag, shootPoint.position, Quaternion.LookRotation(dir));
            shootPoint.localScale = Vector3.zero;
            
            muzzle.Play();
            anim.CrossFade("Shoot", 0.1f, 0, 0);
            
            attackFeedback.Vibrate();
            AudioMaster.instance.Play(shootSound);
            
            shootPoint.DOScale(Vector3.one, shootRate)
                .SetEase(Ease.OutBack, 2f)
                .OnComplete(() =>
                {
                    canShoot = true;
                });
        }
    }

    public void RotateWeapon(Vector3 hitPoint)
    {
        Vector3 dir = (hitPoint - transform.position).normalized;
        dir = transform.parent.InverseTransformDirection(dir);
        targetRot = Quaternion.LookRotation(dir);
        transform.localRotation = targetRot;
/*
        if (rotationSeq != null && rotationSeq.IsActive())
            rotationSeq.Kill();

        rotationSeq = DOTween.Sequence();
        rotationSeq.Append(
            transform.DOLocalRotateQuaternion(targetRot, 0.2f)
                .SetEase(Ease.OutSine)
        );*/
    }
    
    public void DefaultRotation()
    {
        if (rotationSeq != null && rotationSeq.IsActive())
            rotationSeq.Kill();

        rotationSeq = DOTween.Sequence();
        rotationSeq.Append(
            transform.DOLocalRotate(Vector3.zero, 0.2f)
                .SetEase(Ease.OutSine)
        );
    }
}
