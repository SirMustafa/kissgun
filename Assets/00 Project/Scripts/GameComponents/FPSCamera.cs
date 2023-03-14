using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Dreamteck;
using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    #region Singleton

    public static FPSCamera instance = null;
    private void Awake()
    {
        instance = this;
    }
    #endregion
    
    [SerializeField] private ParticleSystem speedEffect;
    
    private Camera[] cameras;
    private float fov;
    private Sequence fovSeq;

    private void Start()
    {
        cameras = GetComponentsInChildren<Camera>();
        fov = cameras[0].fieldOfView;
    }

    public void SetSpeedEffect(bool enabled)
    {
        var emission = speedEffect.emission;
        emission.enabled = enabled;
    }
    
    public void SetFov(float targetFov, float duration, Ease ease)
    {
        if (fovSeq != null && fovSeq.IsActive())
            fovSeq.Kill();
        
        fovSeq = DOTween.Sequence();
        fovSeq.Append(
            DOTween.To(() => fov, x => fov = x, targetFov, duration)
                .SetEase(ease)
                .OnUpdate(() => {
                        for (int i = 0; i < cameras.Length; i++)
                        {
                            cameras[i].fieldOfView = fov;
                        }
                    })
            );
    }
}
