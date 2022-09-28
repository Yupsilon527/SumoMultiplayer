using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ToonActionController;

public class PlayerDamageable : NetworkBehaviour, IRespawnable
{
    private PlayerController controller = null;

    public override void Spawned()
    {
        controller = GetComponent<PlayerController>();
    }
    public void Respawn()
    {
        Rage = 0;
        Damage = 0;
    }
    #region Rage

    [Networked(OnChanged = nameof(OnRageChanged))]
    public float Rage { get; private set; }
    public static void OnRageChanged(Changed<PlayerController> playerInfo)
    {
        UIController.main.UpdatePlayerUI(playerInfo.Behaviour.Object.InputAuthority);
    }
    public void BuildUpRage(float buildup)
    {
        Rage = Mathf.Clamp(Rage + buildup, 0, 100);
    }
    #endregion
    #region Damage


    [Networked(OnChanged = nameof(OnDamageChanged))]
    public float Damage { get; private set; }
    public static void OnDamageChanged(Changed<PlayerController> playerInfo)
    {
        UIController.main.UpdatePlayerUI(playerInfo.Behaviour.Object.InputAuthority);
    }
    public void TakeDamageAndKnockback(float damage, float knockback, Vector3 kbCenter)
    {
        Debug.Log("Player " + name + " hit for " + damage + " damage and " + knockback + " knockback strength!");

        RecieveKnockBack(kbCenter, knockback);
        Damage += damage;
    }
    #endregion
    #region Knockback
    public void RecieveKnockBack(Vector3 center, float strength)
    {
        float damageDelta = Damage / 20;
        KnockBack((transform.position - center).normalized, strength * (.8f + damageDelta * .2f), 1 * (.5f + damageDelta * .5f));
    }
    public void KnockBack(Vector3 direction, float strength, float duration)
    {
        controller.actionman.BeginAction(PlayerAction.stagger, duration);
        controller.rigidbody.velocity = direction * strength;
    }
    #endregion
}
