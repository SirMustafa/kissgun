using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

[System.Serializable]
public class LocalCameraPointData
{
    public Vector3 localPosition = Vector3.zero;
    public Vector3 localEulerAngles = Vector3.zero;
    public float fov = 60f;
    public bool applyFovToChilds = false;

    [SerializeField] private bool needDistance = false;
    [ShowIf("$needDistance")] public float distance = 0f;
}

[System.Serializable]
public class CameraPoint
{
    [FoldoutGroup("$point")] public string point = "Custom Point";

    #region Camera Pivot

    [FoldoutGroup("$point/Camera Pivot")] public bool offsetFromTarget;
    [ShowIf("$offsetFromTarget"), FoldoutGroup("$point/Camera Pivot")] public bool targetIsPlayer = false;
    [ShowIf("$showTarget"), FoldoutGroup("$point/Camera Pivot")] public Transform target;
    bool showTarget => offsetFromTarget == true && targetIsPlayer == false;
    [FoldoutGroup("$point/Camera Pivot")] public bool moveBetweenTwoPoint = false;

    [FoldoutGroup("$point/Camera Pivot"), SerializeField] private Vector3 positionStart;
    [FoldoutGroup("$point/Camera Pivot"), SerializeField, ShowIf("$moveBetweenTwoPoint")] private Vector3 positionEnd;

    [FoldoutGroup("$point/Camera Pivot"), SerializeField] private Vector3 eulerAnglesStart;
    [FoldoutGroup("$point/Camera Pivot"), SerializeField, ShowIf("$moveBetweenTwoPoint")] private Vector3 eulerAnglesEnd;
    [FoldoutGroup("$point/Camera Pivot"), ShowIf("$moveBetweenTwoPoint")] public float transitionSpeed;

    #endregion

    #region Child Pivot

    [FoldoutGroup("$point/Child Pivot")] public LocalCameraPointData childPivot;

    #endregion

    public Vector3 GetOffset()
    {
        return positionStart;
    }

    public void GetData(out Vector3 pos, out Vector3 angles)
    {
        float t = AdvancedMath.Remap(-1f, 1f, 0f, 1f, Mathf.Sin(Time.time * transitionSpeed));

        Vector3 posResult = moveBetweenTwoPoint ? Vector3.Lerp(positionStart, positionEnd, t) : positionStart;

        if (offsetFromTarget)
            posResult += targetIsPlayer ? PlayerController.GetPosition() : target.position;

        Vector3 anglesResult = moveBetweenTwoPoint ? AdvancedMath.VectorLerpAngles(eulerAnglesStart, eulerAnglesEnd, t) : eulerAnglesStart;

        pos = posResult;
        angles = anglesResult;
    }
}

public class CameraFollow : OptimizedUpdate
{
    #region Singleton

    public static CameraFollow instance = null;
    void Awake()
    {
        instance = this;
    }

    #endregion

    private Transform target;

    [TabGroup("Follow Settings"), SerializeField] private Transform childPivot;
    [TabGroup("Follow Settings"), SerializeField] private Vector3 offset = Vector3.zero;
    [TabGroup("Follow Settings"), SerializeField] private Vector3 rotation = Vector3.zero;
    [TabGroup("Follow Settings"), SerializeField, Range(1f, 10f)] private float speed = 7.5f;
    [TabGroup("Follow Settings"), SerializeField, Range(1f, 10f)] private float childSpeed = 7.5f;
    [TabGroup("Follow Settings"), SerializeField, Range(1f, 10f)] private float lookSpeed = 7.5f;
    [TabGroup("Follow Settings"), SerializeField] private Vector3 followScale = Vector3.one;

    // Keep in border system ----> Two object 1: is player, 2: is any object, Camera keep two object in projection area.
    [TabGroup("Follow Settings"), SerializeField] private bool keepInBorder = false;
    [TabGroup("Follow Settings"), SerializeField, ShowIf("keepInBorder")] private LocalCameraPointData minPoint;
    [TabGroup("Follow Settings"), SerializeField, ShowIf("keepInBorder")] private LocalCameraPointData maxPoint;
    [TabGroup("Follow Settings"), SerializeField, ShowIf("keepInBorder")] private Transform referenceObject;
    [TabGroup("Follow Settings"), SerializeField, ShowIf("keepInBorder")] private AnimationCurve transitionCurve;
    [TabGroup("Follow Settings"), SerializeField, ShowIf("keepInBorder")] public float transitionSpeed = 5f;
    private LocalCameraPointData currentPoint = new LocalCameraPointData();
    private float currentT = 0f;

    private float extraFov = 0f;
    [TabGroup("Animation Settings"), SerializeField] private Ease fovEase;
    [TabGroup("Animation Settings"), SerializeField] private Transform animatePivot;

    [TabGroup("Custom Point Settings"), SerializeField] private List<CameraPoint> customPoints = new List<CameraPoint>();
    //[TabGroup("Events"), SerializeField] private BoolEvent victoryEvent;
    //[TabGroup("Events"), SerializeField] private BoolEvent gameOverEvent;
    private CameraPoint currentCustomPoint;

    [HideInInspector] public bool finish = false;

    private Camera[] cameras;

    Vector3 targetPos = Vector3.zero;
    Vector3 defaultRot => rotation;
    Vector3 targetRot = Vector3.zero;
    bool follow = true;
    bool tweenTransition = false;
    
    void Start()
    {
        target = PlayerController.instance.transform;
        cameras = GetComponentsInChildren<Camera>();
        
        if (customPoints.Count > 0 && customPoints[0].point == "Default Point")
            LookFromPoint(0);

        currentPoint.localPosition = childPivot.localPosition;
        currentPoint.localEulerAngles = childPivot.localEulerAngles;

        //victoryEvent.Register(WhenGameEnd);
        //gameOverEvent.Register(WhenGameEnd);

        SetupOptimizedUpdate();
    }

    public override void UpdateMe(float deltaTime)
    {
        if (tweenTransition)
            return;

        if (!finish && currentCustomPoint == null)
        {
            if (keepInBorder)
            {
                targetPos = (PlayerController.GetPosition() + referenceObject.position) * 0.5f;

                float dis = Vector3.Distance(PlayerController.GetPosition(), referenceObject.position);
                float targetT = Mathf.InverseLerp(minPoint.distance, maxPoint.distance, Mathf.Clamp(dis, minPoint.distance, maxPoint.distance));
                currentT = Mathf.Lerp(currentT, targetT, transitionSpeed * deltaTime);
                LerpCameraDataBetweenMinMax(currentT, transitionCurve);
            }
            else
            {
                targetPos = PlayerController.GetPosition()  + offset;
            }

            if (targetRot != defaultRot)
                targetRot = defaultRot;
        }
        
        if (currentCustomPoint != null)
        {          
            Vector3 pos = Vector3.zero;
            Vector3 rot = Vector3.zero;
            currentCustomPoint.GetData(out pos, out rot);

            targetPos = pos;
            targetRot = rot;
            
            if (!tweenTransition)
                LerpCameraData(childSpeed * deltaTime, currentCustomPoint.childPivot);
        }

        if (transform.position != targetPos && !tweenTransition)
            transform.position = AdvancedMath.ChangeVectorBySpeed(transform.position, Vector3.Scale(targetPos, followScale), speed, deltaTime);

        if (transform.eulerAngles != targetRot && !tweenTransition)
            transform.eulerAngles = AdvancedMath.VectorLerpAngles(transform.eulerAngles, targetRot, lookSpeed * deltaTime);
    }


/*
    Sequence lookSeq = null;
    public void LookFrom(string name) {
        if (customPoints.Count == 0)
            return;
            
        int index = customPoints.FindIndex(x => x.point == name);
        if (customPoints[index].point != name)
            return;

        float t = 0f;

        currentCustomPoint = customPoints[index];

        lookSeq = DOTween.Sequence();
        lookSeq.Append(
            DOTween.To(() => t, x => t = x, 1f, 1f)
                .SetEase(Ease.InOutSine)
                .OnUpdate(() => {
                    childPivot.
                })
        );

    }
*/

    public void SetFOVWithAnimation(float targetFOV, bool apply2AllChildCamera = false)
    {
        if(apply2AllChildCamera)
        {
            foreach(Camera cam in cameras)
            {
                DOTween.To(() => cam.fieldOfView, x => cam.fieldOfView = x, targetFOV, 1f)
                    .SetEase(fovEase);
            }
        }
        else
        {
            DOTween.To(() => cameras[0].fieldOfView, x => cameras[0].fieldOfView = x, targetFOV, 1f)
                .SetEase(fovEase);
        }
    }

    public void SetFOV(float targetFOV, bool apply2AllChildCamera = false)
    {
        if(apply2AllChildCamera)
        {
            foreach(Camera cam in cameras)
            {
                cam.fieldOfView = targetFOV + extraFov;
            }
        }
        else
        {
            cameras[0].fieldOfView = targetFOV + extraFov;
        }
    }

    public void SetExtraFOVWithAnimation(float targetFOV, float t = 1f, bool apply2AllChildCamera = false)
    {
        if(apply2AllChildCamera)
        {
            foreach(Camera cam in cameras)
            {
                DOTween.To(() => extraFov, x => extraFov = x, targetFOV, t)
                    .SetEase(fovEase);
            }
        }
        else
        {
            DOTween.To(() => extraFov, x => extraFov = x, targetFOV, t)
                .SetEase(fovEase);
        }
    }

    LocalCameraPointData lastPointData = new LocalCameraPointData();
    private void LerpCameraData(float t, LocalCameraPointData targetPoint, AnimationCurve transition = null)
    {
        currentPoint.localPosition = Vector3.Lerp(childPivot.localPosition, targetPoint.localPosition, t);
        currentPoint.localEulerAngles = AdvancedMath.VectorLerpAngles(childPivot.localEulerAngles, targetPoint.localEulerAngles, Mathf.Clamp01(t));
        currentPoint.fov = Mathf.Lerp(lastPointData.fov, targetPoint.fov, t);

        childPivot.localPosition = currentPoint.localPosition;
        childPivot.localEulerAngles = currentPoint.localEulerAngles;
        SetFOV(currentPoint.fov + extraFov, targetPoint.applyFovToChilds);
    }

    private void LerpCameraDataBetweenMinMax(float t, AnimationCurve transition)
    {
        t = transition.Evaluate(t);

        currentPoint.localPosition = Vector3.Lerp(minPoint.localPosition, maxPoint.localPosition, t);
        currentPoint.localEulerAngles = AdvancedMath.VectorLerpAngles(minPoint.localEulerAngles, maxPoint.localEulerAngles, Mathf.Clamp01(t));
        currentPoint.fov = Mathf.Lerp(minPoint.fov, maxPoint.fov, t);

        childPivot.localPosition = Vector3.Lerp(childPivot.localPosition, currentPoint.localPosition, speed * Time.deltaTime);
        childPivot.localEulerAngles = AdvancedMath.VectorLerpAngles(childPivot.localEulerAngles, currentPoint.localEulerAngles, speed * Time.deltaTime);
        SetFOV(currentPoint.fov + extraFov, true);
    }

    public void LookFromPoint(int index)
    {
        if (index == -1)
        {
            currentCustomPoint = null;
            targetRot = Vector3.zero;
            target = null;
            tweenTransition = false;
        }
        else
        {
            currentCustomPoint = customPoints[index];
            tweenTransition = false;

            try
            {
                target = currentCustomPoint.target;
            }
            catch
            {
                target = null;
            }
        }
    }

    Sequence lookSeq = null;
    public void LookFromPoint(int index, float transition, Ease ease = Ease.Linear)
    {
        if (index == -1)
        {
            currentCustomPoint = null;
            targetRot = Vector3.zero;
            target = null;
            tweenTransition = false;
        }
        else
        {
            currentCustomPoint = customPoints[index];
            try
            {
                target = currentCustomPoint.target;
            }
            catch
            {
                target = null;
            }

            float t = 0f;


            if (lookSeq != null && lookSeq.IsActive())
                lookSeq.Kill();

            lookSeq = DOTween.Sequence();
            tweenTransition = true;
            lastPointData.localPosition = currentPoint.localPosition;
            lastPointData.localEulerAngles = currentPoint.localEulerAngles;
            Vector3 startPos = transform.position;
            Vector3 startRot = transform.eulerAngles;
            Vector3 targetOffset = currentCustomPoint.GetOffset();
            lookSeq.Append(
                DOTween.To(() => t, x => t = x, 1f, transition)
                    .SetEase(ease)
                    .OnUpdate(() => {
                        Vector3 pos = Vector3.zero;
                        Vector3 rot = Vector3.zero;
                        currentCustomPoint.GetData(out pos, out rot);

                        targetPos = PlayerController.GetPosition() + Vector3.Lerp(offset, targetOffset, t);
                        targetRot = rot;

                        transform.position = targetPos;
                        transform.eulerAngles = AdvancedMath.VectorLerpAngles(startRot, targetRot, t);

                        LerpCameraData(t, currentCustomPoint.childPivot);
                    })
                    .OnComplete(() => {
                        lastPointData.localPosition = childPivot.localPosition;
                        lastPointData.localEulerAngles = childPivot.localEulerAngles;
                        tweenTransition = false;
                    })
                    .SetUpdate(UpdateType.Late)
            );
        }
    }

    private void WhenGameEnd()
    {

    }
}