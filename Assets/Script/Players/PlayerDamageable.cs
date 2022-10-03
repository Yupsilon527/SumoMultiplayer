using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ToonActionController;

public class PlayerDamageable : NetworkBehaviour, IRespawnable
{
    private PlayerController controller = null;

    public float DamageKnockbackMultiplier = 1 / 20;
    public float RageMax = 100;
    public float DamageToRage = .2f;

    public float DamageRegen = 20;
    public float DamageRegenTime = 5;

    public override void Spawned()
    {
        controller = GetComponent<PlayerController>();
    }
    public void Respawn()
    {
        Rage = 0;
        Damage = 0;
    }
    public override void FixedUpdateNetwork()
    {
        if (damageRegenTimer.Expired(Runner))
        {
            HandleDamageRegen();
        }
    }
    #region Rage

    [Networked(OnChanged = nameof(OnRageChanged))]
    public float Rage { get; private set; }
    public static void OnRageChanged(Changed<PlayerDamageable> playerInfo)
    {
        UIController.main.UpdatePlayerUI(playerInfo.Behaviour.Object.InputAuthority);
    }
    public void BuildUpRage(float buildup)
    {
        Rage = Mathf.Clamp(Rage + buildup, 0, RageMax);
    }
    #endregion
    #region Damage


    [Networked(OnChanged = nameof(OnDamageChanged))]
    public float Damage { get; private set; }
    TickTimer damageRegenTimer;
    public static void OnDamageChanged(Changed<PlayerDamageable> playerInfo)
    {
        UIController.main.UpdatePlayerUI(playerInfo.Behaviour.Object.InputAuthority);
    }
    public void TakeDamageAndKnockback(float damage, float knockback, Vector3 kbCenter)
    {
        Debug.Log("Player " + name + " hit for " + damage + " damage and " + knockback + " knockback strength!");

        Damage += damage;
        BuildUpRage(damage * DamageToRage);
        ScaledKnockBack(kbCenter, knockback);

        RefreshRegenTimer();
    }
    void HandleDamageRegen()
    {
        if (Damage > DamageRegen)
            Damage -= DamageRegen;
        else
            Damage = 0;
        RefreshRegenTimer();
    }
    void RefreshRegenTimer()
    {

        damageRegenTimer = TickTimer.CreateFromSeconds(Runner, DamageRegenTime);
    }
    #endregion
    #region Knockback
    public void ScaledKnockBack(Vector3 center, float strength)
    {
        float damageDelta = Damage * DamageKnockbackMultiplier;
        KnockBack((transform.position - center).normalized, strength * (1 + damageDelta), 1 * (.5f + damageDelta * .33f),true);
    }
    public void KnockBack(Vector3 direction, float strength, float duration, bool stagger)
    {
        if (stagger)
            controller.actionman.BeginAction(PlayerAction.stagger, duration);
        controller.rigidbody.velocity = direction.normalized * strength;
    }
    #endregion
}
