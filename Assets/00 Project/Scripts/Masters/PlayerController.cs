using System;
using Dreamteck.Splines;
using MadLab.Sensor;
using Sirenix.OdinInspector;
using UniRx;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class PlayerController : Character
{
    #region Singleton
    public static PlayerController instance = null;
    private void Awake() {
        instance = this;
    }
    #endregion

    [TabGroup("Physics"), SerializeField] private ML_Raycaster raycaster;
    [TabGroup("Physics"), SerializeField] private Camera myCam;
    [TabGroup("Physics"), SerializeField] private Gun weapon;
    [TabGroup("Physics"), SerializeField] private Animator anim;
    
    [TabGroup("Events"), SerializeField] private BoolEvent onGameStart;
    [TabGroup("Events"), SerializeField] private BoolEvent onVictory;
    [TabGroup("Events"), SerializeField] private Vector2Event onPointerDown;
    [TabGroup("Events"), SerializeField] private Vector2Event onPointerUp;
    [TabGroup("Events"), SerializeField] private Vector2Variable crosshairPosition;


    private bool isKilled = false;
    private bool isStarted = false;
    private bool passingStage = false;
    private bool isPointerDown = false;
    private IDisposable attackDisposable;
    
    public static Vector3 GetPosition() {
        return instance.transform.position;
    }

    #region Start & Update

    private void Start()
    {
        onPointerDown.Register(OnPointerDown);
        onPointerUp.Register(OnPointerUp);
        onGameStart.Register(OnGameStart);
        onVictory.Register(OnGameVictory);
    }

    #endregion
    
    private void Attack()
    {
        // Calculate direction
        Ray ray = myCam.ScreenPointToRay(crosshairPosition.Value);
        RaycastHit hit;

        Vector3 hitPoint = Vector3.zero;
        
        if (raycaster.SendRay(ray.direction.normalized, out hit))
        {
            hitPoint = hit.point;
        }
        else
        {
            hitPoint = raycaster.GetHitPoint();
        }
        
        weapon.RotateWeapon(hitPoint);
        weapon.Attack(hitPoint);
    }
    
    public void GoToNextStage(SplineFollower follower)
    {
        follower.follow = true;
        weapon.SetSpeed(1f);
        weapon.DefaultRotation();
        passingStage = true;
        attackDisposable?.Dispose();
        UIMaster.instance.crosshair.ResetToCenter();

        follower.onEndReached += d =>
        {
            weapon.SetSpeed(0f);
            passingStage = false;
            follower.follow = false;
            StageMaster.instance.StartStage();
            if (isPointerDown)
                OnPointerDown();
            UIMaster.instance.crosshair.Setup();
        };
    }

    #region Inputs

    private void OnPointerDown()
    {
        isPointerDown = true;
        if (passingStage)
            return;

        attackDisposable = Observable.EveryUpdate()
            .TakeUntilDisable(this)
            .Subscribe(_ =>
            {
                Attack();
            });
    }

    private void OnPointerUp()
    {
        attackDisposable?.Dispose();
        isPointerDown = false;
    }

    #endregion
    
    #region Game Events
    
    private void OnGameStart()
    {
        isStarted = true;
    }

    private void OnGameVictory()
    {
        isStarted = false;
    }

    #endregion

    public void KillMe()
    {
        if (isKilled)
            return;
        
        isKilled = true;
        isStarted = false;
        GameManager.instance.GameOver();
        anim.Play("Die");
    }
}
