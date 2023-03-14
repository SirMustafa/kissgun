using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Dreamteck.Splines;
using ElCapitan.StatePatterns;
using MadLab;
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCCharacter : Character
{
    [HideInInspector] public Rigidbody rb;
    public Health target = null;
    public bool doAction = false;
    public int attackTargetId = 0;
    [HideInInspector] public List<NPCCharacter> npcTargets = new List<NPCCharacter>();
    [HideInInspector] public Vector3 myVelocity;
    [HideInInspector] public Vector3 myDir;
    [HideInInspector] public int stageIndex;
    [HideInInspector] public float nextAttack;
    [HideInInspector] public float currentSpeed;
    public bool inLove { get; private set; } = false;
    public Vector3 targetPos => target.transform.position;

    private BaseState currentState;

    public NPCIdleState idleState;
    public NPCMoveState moveState;
    public NPCJumpState jumpState;
    public NPCWanderState wanderState;
    public NPCAttackState attackState;
    public NPCStageActionState stageActionState;
    private NPCDeathState deathState;

    [TabGroup("Physics")] public FloatVariable speed;
    [TabGroup("Physics")] public FloatVariable attackRadius;
    [TabGroup("Physics")] public float attackRate;
    [TabGroup("Physics")] public DynamicRagdoll ragdoll;
    [TabGroup("Physics")] public Rigidbody helmet;

    [TabGroup("Character")] public Health loveHealth;
    [TabGroup("Character")] public  IntVariable damage;
    [TabGroup("Character")] public bool jump;
    [TabGroup("Character")] public float jumpDelay;
    [TabGroup("Character")] public Transform jumpPoint;
    [TabGroup("Character")] public Vector2 scaleBounds;

    [TabGroup("Character"), SerializeField]
    protected EnemyArmor[] armors;

    [TabGroup("Effects")] public Animator anim;
    [TabGroup("Effects")] public AudioSource source;
    [TabGroup("Effects")] public AudioClip punchSfx;
    [TabGroup("Effects")] public AudioClip fallInLoveSfx;
    [TabGroup("Effects")] public Vector2 punchPitchRange;
    [TabGroup("Effects")] public Vector2 lovePitchRange;
    [TabGroup("Effects"), SerializeField] protected SkinnedMeshRenderer body;
    [TabGroup("Effects"), SerializeField] protected ObjectsData emotions;
    [TabGroup("Effects"), SerializeField] protected GameObject heartEyes;
    [TabGroup("Effects"), SerializeField] protected Material deadMaterial;

    private Tween bodyMaterailTween;
    private float startHealth;

    [Button]
    private void UpdateArmors()
    {
        for (int i = 0; i < armors.Length; i++)
        {
            armors[i].SetArmor();
        }
    }
    
    public bool ItsMakeDanger()
    {
        return !(currentState == deathState || inLove);
    }

    #region Mono Behaviours

    private void Start()
    {
        CreateStates();
        SetState(idleState);
        currentSpeed = speed.Value;
        startHealth = loveHealth.health;

        float scaleT = 0;
        float armorCount = 0;
        for (int i = 0; i < armors.Length; i++)
        {
            if (armors[i].isArmored)
                armorCount++;
        }

        scaleT = armorCount / armors.Length;
        transform.localScale = Vector3.one * Mathf.Lerp(scaleBounds.x, scaleBounds.y, scaleT);
        rb = GetComponent<Rigidbody>();

        if (!anim)
            anim = GetComponentInChildren<Animator>();

        anim.Play("Movement", 0, Random.Range(0f, 1f));
    }

    private void OnDisable()
    {
        currentState.Dispose();
    }

    #endregion

    #region States Managements

    private void CreateStates()
    {
        idleState = new NPCIdleState(this);
        moveState = new NPCMoveState(this);
        jumpState = new NPCJumpState(this);
        wanderState = new NPCWanderState(this);
        attackState = new NPCAttackState(this);
        stageActionState = new NPCStageActionState(this);
        deathState = new NPCDeathState(this);
    }

    public void SetState(BaseState state)
    {
        currentState?.Dispose();

        currentState = state;
        currentState.Initialize();
    }

    #endregion

    public void SetTarget(Health target)
    {
        this.target = target;
        attackTargetId = target.GetInstanceID();
    }
    
    public bool CanFight()
    {
        var result = npcTargets.Count > 0 ? (npcTargets[0].health.isAlive && (inLove ? !npcTargets[0].inLove : npcTargets[0].inLove))
                : target && target.isAlive;
        return result;
    }

    public void TakeLoveDamage(int damage)
    {
        loveHealth.TakeDamage(damage);
        float durability = loveHealth.health / startHealth;
        bodyMaterailTween?.Kill();
        bodyMaterailTween = DOTween.To(() =>
            body.materials[0].GetFloat("_Durability"), x => body.materials[0].SetFloat("_Durability", x), durability, 0.25f);
    }

    public void Fight(NPCCharacter npc)
    {
        npcTargets.Add(npc);
        SetTarget(npc.health);
        var attack = Vector3.Distance(target.transform.position, transform.position) <= attackRadius.Value;
        if (attack)
            SetState(attackState);
        else
            SetState(moveState);
    }

    public void WakeUp()
    {
        if (jump)
        {
            SetState(jumpState);
        }
        else
        {
            SetTarget(PlayerController.instance.health);
            SetState(moveState);
        }
        SetEmotion("Angry");
    }

    public void LookPlayer(SplineFollower follower = null)
    {
        stageActionState.doAction = false;
        SetState(stageActionState);
        target = PlayerController.instance.health;
        if (follower is { })
            follower.onEndReached += d => { currentState.Dispose(); };
    }
    
    /// <summary>
    /// For Stage Action
    /// </summary>
    /// <param name="position"></param>
    /// <param name="eulerAngles"></param>
    /// <param name="duration"></param>
    /// <param name="ease"></param>
    /// <param name="OnComplete"></param>
    /// <param name="waitForRotation"></param>
    public void GoTo(Vector3 position, Vector3 eulerAngles, float duration = 0f, Ease ease = Ease.InOutSine,
        simpleDelegate OnComplete = null, bool waitForRotation = false)
    {
        myDir = (position - transform.position).normalized;
        
        stageActionState.doAction = true;
        SetState(stageActionState);
        
        float animSpeed = anim.GetFloat("Speed");
        DOTween.To(() => animSpeed, x => animSpeed = x, 1f, 0.25f)
            .OnUpdate(() => { anim.SetFloat("Speed", animSpeed); });

        transform.DOMove(position, duration)
            .SetEase(ease)
            .OnUpdate(LookToTarget)
            .OnComplete(() =>
            {
                if (!waitForRotation && OnComplete != null)
                    OnComplete();

                var animSpeed = anim.GetFloat("Speed");
                DOTween.To(() => animSpeed, x => animSpeed = x, 0f, 0.25f)
                    .OnUpdate(() => { anim.SetFloat("Speed", animSpeed); });

                transform.DORotate(eulerAngles, 0.25f)
                    .OnComplete(() =>
                    {
                        if (waitForRotation && OnComplete != null)
                            OnComplete();
                    });
            });
    }

    public void CalculateDir()
    {
        if (target)
            myDir = AdvancedMath.ScaleTopdown(targetPos - transform.position).normalized;
    }

    public void LookToTarget()
    {
        var targetRot = Quaternion.LookRotation(myDir, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 10f * Time.deltaTime);
    }

    public void AttackToTarget()
    {
        if (target.GetInstanceID() == attackTargetId)
        {
            target.TakeDamage(damage.Value);
            source.pitch = Random.Range(punchPitchRange.x, punchPitchRange.y);
            source.PlayOneShot(punchSfx);
        }
    }

    public void KillMe(bool setupRagdoll = false)
    {
        SetState(deathState);
        gameObject.SetActive(false);
        if (setupRagdoll)
        {
            ragdoll.SetupRagdoll(Vector3.zero);
        }

        if (!inLove)
            StageMaster.instance.SomeEnemyKilled(stageIndex);
        else
        {
            heartEyes.SetActive(false);
            inLove = false;
        }

        if (armors[0].isArmored)
        {
            armors[0].model.SetActive(false);
            Vector3 force = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)).normalized * Random.Range(5, 10);
            helmet.transform.SetParent(null);
            helmet.gameObject.SetActive(true);
            helmet.AddForce(force, ForceMode.Impulse);
        }
        
        body.material = deadMaterial;
        SetEmotion("Dead");
    }

    [Button("Fall in Love")]
    public void FallInLove()
    {
        if (inLove)
            return;

        inLove = true;
        currentSpeed = speed.Value * 1.5f;

        source.pitch = Random.Range(lovePitchRange.x, lovePitchRange.y);
        source.PlayOneShot(fallInLoveSfx);
        
        float inLoveValue = anim.GetFloat("InLove");
        DOTween.To(() => inLoveValue, x => inLoveValue = x, 1f, 0.4f)
            .OnUpdate(() => { anim.SetFloat("InLove", inLoveValue); });

        StageMaster.instance.SomeEnemyKilled(stageIndex);

        NPCCharacter enemy = StageMaster.instance.GetStage(stageIndex).alivedEnemyCount.Value > 0
            ? StageMaster.instance.GetStage(stageIndex).ClosestEnemy(transform.position)
            : null;

        npcTargets.Clear();
        if (enemy)
        {
            SetTarget(enemy.health);
            npcTargets.Add(enemy);
        }

        health.health = (int) (health.health * 1.5f);

        heartEyes.SetActive(true);
        //SetBodyMaterial(inLoveMaterial);
        SetEmotion("Love");
    }

    #region Visual Methods

    private void SetEmotion(string emotion)
    {
        if (body.materials.Length > 1)
            body.materials[1].SetTexture("_Albedo", emotions.GetObject(emotion) as Texture);
    }

    private void SetBodyMaterial(Material targetMat, float duration = 1f, Ease ease = Ease.InOutSine)
    {
        var startMaterial = body.sharedMaterials[0];

        var t = 0f;
        DOTween.To(() => t, x => t = x, 1f, duration)
            .SetEase(ease)
            .OnUpdate(() => { body.material.Lerp(startMaterial, targetMat, t); });
    }

    #endregion
}