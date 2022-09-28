using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToonActionController : NetworkBehaviour, IRespawnable
{
    #region Settings
    [Header("General Attack")]
    public float AttackContactDistance = 1f;
    public float AttackContactRadius = 1f;
    public LayerMask AttackHitMask;
    public float ABC = 1f;
    public float DEF = 1f;

    [Header("Regular Attack")]
    public AudioClip SlashSound;
    public float SlashAttackDuration = 1f;
    public float SlashAttackContactStart = 1f;
    public float SlashAttackContactEnd = 1f;
    public float SlashAttackDamage = 1f;
    public float SlashAttackKnockback = 1f;
    public float SlashAttackRageInitial = 1f;
    public float SlashAttackRageConsecutive = 1f;
    public float SlashAttackSelfPush = 1f;

    [Header("Strong Attack")]
    public AudioClip SmashSound;
    public float SmashAttackDuration = 1f;
    public float SmashAttackContactStart = 1f;
    public float SmashAttackContactEnd = 1f;
    public float SmashAttackDamage = 1f;
    public float SmashAttackKnockback = 1f;

    [Header("Parry")]
    public AudioClip ParryContactSound;
    public float ParryDuration = .33f;
    public float ParryStagger = .5f;

    [Header("Dodge")]
    public AudioClip DodgeSound;
    public float DashDuration = .33f;
    public float DashStagger = .5f;
    public float DashSpeed = 1;
    #endregion


    #region General
    private PlayerController controller = null;
    public override void Spawned()
    {
        controller = GetComponent<PlayerController>();
    }
    public void Respawn()
    {
        currentAction = PlayerAction.free;
        actionTime = TickTimer.None;
        actionDirection = Vector3.zero;
        PlayerHits.Clear();
        FrameHits.Clear();
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.TryGetInputForPlayer(Object.InputAuthority, out ToonInput input))
        {
            HandlePlayerInput(input);
        }
        HandleAction();
    }
    #endregion
    #region Player Input
    [Networked] private NetworkButtons _buttonsPrevious { get; set; }
    public float ChargeBuildUp1 { get => ChargeBuildUp; set => ChargeBuildUp = value; }

    void HandlePlayerInput(ToonInput input)
    {
        if (controller.CanAct())
        {
            if (input.Buttons.WasPressed(_buttonsPrevious, ToonInput.Button.Weak))
            {
                BeginAction(PlayerAction.attack, SlashAttackDuration);
            }
            if (input.Buttons.WasPressed(_buttonsPrevious, ToonInput.Button.Strong))
            {
                BeginAction(PlayerAction.charging);
            }
            if (input.Buttons.WasPressed(_buttonsPrevious, ToonInput.Button.Parry))
            {
                BeginAction(PlayerAction.parry, ParryStagger);
            }
            if (input.Buttons.WasPressed(_buttonsPrevious, ToonInput.Button.Dash))
            {
                BeginAction(PlayerAction.dash, DashStagger);
            }
        }
        if (currentAction == PlayerAction.charging && !input.Buttons.IsSet(ToonInput.Button.Strong))
        {
            BeginAction(PlayerAction.strongattack, SmashAttackDuration);
        }
    }
    #endregion

    #region Actions
    public enum PlayerAction
    {
        free = 0,
        stagger = 1,
        attack = 2,
        charging = 3,
        strongattack = 4,
        parry = 5,
        dash = 6
    }

    [HideInInspector]
    public PlayerAction currentAction = PlayerAction.free;
    TickTimer actionTime;
    Vector3 actionDirection = Vector3.zero;
    public void BeginAction(PlayerAction nState)
    {
        actionTime = TickTimer.None;
        currentAction = nState;
        Debug.Log(name + " return to state " + nState);
    }
    public void BeginAction(PlayerAction nState, float dur)
    {
        actionTime = TickTimer.CreateFromSeconds(Runner, dur);
        currentAction = nState;
        actionDirection = new Vector3(controller.mover.moveDir.x,0, controller.mover.moveDir.y);
        Debug.Log(name + " begin action " + nState+" for "+dur+" seconds.");

        switch (currentAction)
        {
            case PlayerAction.attack:
                controller.audio.PlayOneShot(SlashSound);
                PlayerHits.Clear();
                break;
            case PlayerAction.strongattack:
                controller.audio.PlayOneShot(SmashSound);
                PlayerHits.Clear();
                break;
            case PlayerAction.parry:
            case PlayerAction.dash:
                controller.audio.PlayOneShot(DodgeSound);
                break;
            case PlayerAction.stagger:
                controller.audio.PlayOneShot(SlashSound);
                PlayerHits.Clear();
                break;
        }
        }
    void HandleAction()
    {
        switch (currentAction)
        {
            case PlayerAction.attack:
            case PlayerAction.strongattack:

                float remainTime = actionTime.RemainingTime(Runner) ?? 0;
                if ((currentAction ==  PlayerAction.attack && SlashAttackDuration - remainTime > SlashAttackContactStart && SlashAttackDuration - remainTime < SlashAttackContactEnd) ||
                    (currentAction == PlayerAction.strongattack && SmashAttackDuration - remainTime > SmashAttackContactStart && SmashAttackDuration - remainTime < SmashAttackContactEnd))
                {
                    AttackFrame();
                }

                if (actionTime.ExpiredOrNotRunning(Runner))
                    BeginAction(PlayerAction.free);
                break;
            case PlayerAction.charging:
                controller.BuildUpRage(ABC);
                ChargeBuildUp = Mathf.Max(ChargeBuildUp + DEF,1);
                if (ChargeBuildUp >= 1 || controller.Rage <= 0)
                {
                    BeginAction(PlayerAction.strongattack, SmashAttackDuration);
                }
                break;
            case PlayerAction.parry:
                if (actionTime.ExpiredOrNotRunning(Runner))
                    BeginAction(PlayerAction.free);
                break;
            case PlayerAction.dash:
                controller.rigidbody.velocity = actionDirection * DashSpeed;
                if (actionTime.ExpiredOrNotRunning(Runner))
                {
                    BeginAction(PlayerAction.free);
                    controller.rigidbody.velocity *= 0;
                }
                break;
            case PlayerAction.stagger:
                if (actionTime.ExpiredOrNotRunning(Runner))
                {
                    BeginAction(PlayerAction.free);
                    controller.rigidbody.velocity *= 0;
                }
                break;
        }
    }

    public bool IsActing()
    {
        return currentAction == PlayerAction.free || !actionTime.ExpiredOrNotRunning(Runner);
    }
    #endregion
    #region Dashing and Parrying
    public bool IsDodging()
    {
        return currentAction == PlayerAction.dash && actionTime.RemainingTime(Runner) > DashStagger - DashDuration;
    }
    public bool IsParrying()
    {
        return currentAction == PlayerAction.parry && actionTime.RemainingTime(Runner) > ParryStagger - ParryDuration;
    }
    #endregion
    #region Attacking
    private List<PlayerController> PlayerHits = new List<PlayerController>();
    private List<LagCompensatedHit> FrameHits = new List<LagCompensatedHit>();
    void AttackFrame()
    {
        FrameHits.Clear();

        Vector3 center = transform.position;
        
        int count = Runner.LagCompensation.OverlapSphere(center, AttackContactRadius,
            Object.InputAuthority, FrameHits, AttackHitMask.value);
        Debug.Log(count);

        if (count <= 0) return;

        foreach (LagCompensatedHit hit in FrameHits)
        {
            if (hit.GameObject.TryGetComponent(out PlayerController enemy))
            {
                    ProcessHit(enemy, currentAction == PlayerAction.strongattack);
            }
        }
    }
    void ProcessHit(PlayerController sucker, bool strongAttack)
{
    if (sucker != controller && !PlayerHits.Contains(sucker))
    {
        PlayerHits.Add(sucker);
            sucker.TakeDamageAndKnockback(strongAttack ? SmashAttackDamage : SlashAttackDamage, strongAttack ? SmashAttackKnockback : SlashAttackKnockback,transform.position);
            if (!strongAttack)
            {
                if (PlayerHits.Count == 1)
                {
                    controller.BuildUpRage(SlashAttackRageInitial);
                    controller.KnockBack(Vector3.right, 1, SlashAttackSelfPush);
                }
                controller.BuildUpRage(SlashAttackRageConsecutive);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + Vector3.right * AttackContactDistance, AttackContactRadius);
    }

    #endregion
    #region Charging Attacks
    [Networked] private float ChargeBuildUp { get; set; }
    public bool IsChargingAttack()
    {
        return currentAction == PlayerAction.charging;
    }
    #endregion
}
