using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ToonActionController : NetworkBehaviour, IRespawnable
{
    #region Settings
    [Header("General Stats")]
    public float RegularDrag = 0;
    public float StaggerDrag = 0;
    public float RegularBounce = 0;
    public float StaggerBounce = 0;

    [Header("General Attack")]
    public float AttackContactDistance = 1f;
    public float AttackContactRadius = 1f;
    public LayerMask AttackHitMask;

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
    public float SmashAttackChargeStart = 1f;
    public float SmashAttackContactStart = 1f;
    public float SmashAttackContactEnd = 1f;
    public float SmashAttackDamage = 1f;
    public float SmashAttackKnockback = 1f;
    public float SmashAttackRageGrowth = 1f;
    public float SmashAttackRageMultiplier = 1f;
    public float SmashAttackRageBuildupTime = 1f;

    [Header("Parry")]
    public AudioClip ParryContactSound;
    public float ParryDuration = .33f;
    public float ParryStagger = .5f;
    public float ParryCooldown = 1;
    public float ParryKnockbackStrength = 1;
    public float ParryKnockbackDuration = 1;
    public float ParryRage = 1;

    [Header("Dodge")]
    public AudioClip DodgeSound;
    public float DashDuration = .33f;
    public float DashStagger = .5f;
    public float DashSpeed = 1;
    public float DashCooldown = 1;
    #endregion


    #region General
    private PlayerController controller = null;
    private Collider collider = null;
    public override void Spawned()
    {
        controller = GetComponent<PlayerController>();
        collider = GetComponent<Collider>();
    }
    public void Respawn()
    {
        if (!Object.HasStateAuthority) return;
        currentAction = PlayerAction.free;
        actionTime = TickTimer.None;
        actionDirection = Vector3.zero;
        PlayerHits.Clear();
        FrameHits.Clear();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        if (Runner.TryGetInputForPlayer(Object.InputAuthority, out ToonInput input))
        {
            HandlePlayerInput(input);
        }
        HandleAction();
    }
    #endregion
    #region Player Input
    [Networked] private NetworkButtons _buttonsPrevious { get; set; }

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
                BeginAction(PlayerAction.charging, SmashAttackChargeStart);
            }
            if (input.Buttons.WasPressed(_buttonsPrevious, ToonInput.Button.Parry))
            {
                if (dashCooldown.ExpiredOrNotRunning(Runner))
                {
                    BeginAction(PlayerAction.parry, ParryStagger);
                }
            }
            if (input.Buttons.WasPressed(_buttonsPrevious, ToonInput.Button.Dash))
            {
                if (dashCooldown.ExpiredOrNotRunning(Runner))
                {
                    BeginAction(PlayerAction.dash, DashStagger);
                }
            }
        }
        if (currentAction == PlayerAction.charging && !input.Buttons.IsSet(ToonInput.Button.Strong) && actionTime.ExpiredOrNotRunning(Runner))
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
    [Networked(OnChanged = nameof(PlayerActionChange))]
    public PlayerAction currentAction { get; set; }
    TickTimer actionTime;
    Vector3 actionDirection = Vector3.zero;
    public void BeginAction(PlayerAction nState)
    {
        actionTime = TickTimer.None;
        currentAction = nState;
        Debug.Log(name + " return to state " + nState);
        UpdatePhysics();
    }
    public void BeginAction(PlayerAction nState, float dur)
    {
        actionTime = TickTimer.CreateFromSeconds(Runner, dur);
        actionDirection = new Vector3(controller.mover.moveDir.x,0, controller.mover.moveDir.y).normalized;
        Debug.Log(name + " begin action " + nState+" for "+dur+" seconds.");

        switch (nState)
        {
            case PlayerAction.attack:
                controller.RpcPlaySound(SlashSound);
                PlayerHits.Clear();
                break;
            case PlayerAction.strongattack:
                controller.RpcPlaySound(SmashSound);
                PlayerHits.Clear();
                break;
            case PlayerAction.parry:
            case PlayerAction.dash:
                controller.RpcPlaySound(DodgeSound);
                dashCooldown = TickTimer.CreateFromSeconds(Runner, (currentAction == PlayerAction.dash) ? DashCooldown : ParryCooldown);
                if (actionDirection.x == 0 && actionDirection.y == 0)
                    actionDirection = controller.mover.GetVectorForward();
                break;
            case PlayerAction.stagger:
                controller.RpcPlaySound(SlashSound);
                PlayerHits.Clear();
                break;
        }
        currentAction = nState;
        UpdatePhysics();
    }
    void UpdatePhysics()
    {
        controller.rigidbody.drag = (currentAction == PlayerAction.stagger) ? StaggerDrag : RegularDrag;
        collider.material.bounciness = (currentAction == PlayerAction.stagger) ? StaggerBounce : RegularBounce;
    }
    void HandleAction()
    {
        switch (currentAction)
        {
            case PlayerAction.attack:
            case PlayerAction.strongattack:

                float remainTime = actionTime.RemainingTime(Runner) ?? 0;
                if ((currentAction ==  PlayerAction.attack && SlashAttackDuration - remainTime >= SlashAttackContactStart && SlashAttackDuration - remainTime <= SlashAttackContactEnd) ||
                    (currentAction == PlayerAction.strongattack && SmashAttackDuration - remainTime >= SmashAttackContactStart && SmashAttackDuration - remainTime <= SmashAttackContactEnd))
                {
                    AttackFrame();
                }

                if (actionTime.ExpiredOrNotRunning(Runner))
                {
                    ChargeBuildUp = 0;
                    BeginAction(PlayerAction.free);
                }
                break;
            case PlayerAction.charging:
                if (!actionTime.ExpiredOrNotRunning(Runner))
                    break;                
                float percent = 1f / SmashAttackRageBuildupTime * Runner.DeltaTime;
                controller.damageable.BuildUpRage(-controller.damageable.RageMax * percent);
                ChargeBuildUp = Mathf.Min(ChargeBuildUp + percent, 1);
                if (ChargeBuildUp >= 1 || controller.damageable.Rage <= 0)
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
    public static void PlayerActionChange(Changed<ToonActionController> playerInfo)
    {
        if (playerInfo.Behaviour.controller == null)
            return;
        switch (playerInfo.Behaviour.currentAction)
        {
            case PlayerAction.attack:
                playerInfo.Behaviour.controller.animations.PlaySpecific("hit");
                break;
            case PlayerAction.strongattack:
                playerInfo.Behaviour.controller.animations.PlaySpecific("chargeHit");
                break;
            case PlayerAction.parry:
                playerInfo.Behaviour.controller.animations.PlaySpecific("parry");
                break;
            case PlayerAction.dash:
                playerInfo.Behaviour.controller.animations.PlaySpecific("dodge");
                break;

        }
        playerInfo.Behaviour.controller.animations.SetStaggered(playerInfo.Behaviour.currentAction == PlayerAction.stagger);
        playerInfo.Behaviour.controller.animations.SetCharging(playerInfo.Behaviour.currentAction == PlayerAction.charging);
    }

    public bool IsActing()
    {
        return currentAction == PlayerAction.free || !actionTime.ExpiredOrNotRunning(Runner);
    }
    #endregion
    #region Dashing and Parrying
    TickTimer dashCooldown;
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

        bool strongattack = currentAction == PlayerAction.strongattack;

        Vector3 center = transform.position + controller.mover.GetVectorForward() * AttackContactDistance;

        float radiusScale = strongattack ? (1f + ChargeBuildUp * SmashAttackRageGrowth) : 1;
        int count = Runner.LagCompensation.OverlapSphere(center, AttackContactRadius * radiusScale,
            Object.InputAuthority, FrameHits, AttackHitMask.value);

        if (count <= 0) return;

        foreach (LagCompensatedHit hit in FrameHits)
        {
            if (hit.GameObject.TryGetComponent(out PlayerController enemy))
            {
                    ProcessHit(enemy, strongattack);
            }
        }
    }
    void ProcessHit(PlayerController victim, bool strongAttack)
    {
        if (victim != controller && !PlayerHits.Contains(victim))
        {
            PlayerHits.Add(victim);
            if (victim.actionman.IsDodging())
                return;

            float damage = strongAttack ? SmashAttackDamage : SlashAttackDamage;
            float knockback = strongAttack ? SmashAttackKnockback : SlashAttackKnockback;

            if (!strongAttack)
            {
                if (victim.actionman.IsParrying())
                {
                    victim.damageable.BuildUpRage(victim.actionman.ParryRage);
                    victim.RpcPlaySound(ParryContactSound);
                    controller.damageable.KnockBack((transform.position - victim.transform.position), ParryKnockbackDuration, ParryKnockbackStrength, true);
                    return;
                }
                else
                {
                    if (PlayerHits.Count == 1)
                    {
                        controller.damageable.BuildUpRage(SlashAttackRageInitial);
                        controller.damageable.KnockBack(-controller.mover.GetVectorForward(), 1, SlashAttackSelfPush,false);
                    }
                    controller.damageable.BuildUpRage(SlashAttackRageConsecutive);
                }
            }
            else
            {
                float rageModifier = SmashAttackRageMultiplier * ChargeBuildUp ;
                damage *= 1 + rageModifier;
                knockback *= 1 + rageModifier;
            }
            victim.damageable.TakeDamageAndKnockback(damage, knockback, transform.position);
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
