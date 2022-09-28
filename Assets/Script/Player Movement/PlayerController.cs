using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ToonActionController;

public class PlayerController : NetworkBehaviour, IRespawnable
{
    public ToonMovement mover;
    public ToonActionController actionman;
    public PlayerDamageable damageable;
    public Rigidbody rigidbody ;
    public AudioSource audio;
    private void Awake()
    {
        if (mover==null)
        mover =   GetComponent<ToonMovement>();
        if (actionman == null)
            actionman = GetComponent<ToonActionController>();
        if (rigidbody == null)
            rigidbody = GetComponent<Rigidbody>();
        if (damageable == null)
            damageable = GetComponent<PlayerDamageable>();
        if (audio == null)
            audio = GetComponent<AudioSource>();
    }

    public override void Spawned()
    {
        Initalize();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
    }
    void Initalize()
    {
        if (Object.HasStateAuthority)
        {
            StartPosition = transform.position;
            Respawn();
        }
        UIController.main.AddPlayer(Object.InputAuthority, this);
    }
    #region Respawn
    public Vector3 StartPosition { get; private set; }
    public void Respawn()
    {
        Score = 0;
        transform.position = StartPosition;
    }
    public override void FixedUpdateNetwork()
    {
        HandleScoreUpdate();
    }
    #endregion


    public bool CanAct()
    {
        return GameController.main.currentState == GameController.GameState.ingame && actionman.currentAction == PlayerAction.free;
    }

    public bool CanMove()
    {
        return GameController.main.currentState == GameController.GameState.ingame && (actionman.currentAction == PlayerAction.free || actionman.currentAction == PlayerAction.charging);
    }
    #region Nickname
       public NetworkString<_16> NickName { get; private set; }
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetNickName(string nickName)
    {
        if (string.IsNullOrEmpty(nickName)) return;
        NickName = nickName;
    }
    #endregion
    #region Score

    [Networked(OnChanged = nameof(OnEnterAbductionBeam))]
    public bool IsAbducted { get; private set; }
    public static void OnEnterAbductionBeam(Changed<PlayerController> playerInfo)
    {

    }
    public void AddToScore(float points)
    {
        Score += points;
    }

    [Networked(OnChanged = nameof(OnScoreChanged))]
    public float Score { get; private set; }
    public static void OnScoreChanged(Changed<PlayerController> playerInfo)
    {
        UIController.main.UpdatePlayerUI(playerInfo.Behaviour.Object.InputAuthority);
    }
    void HandleScoreUpdate()
    {
        if (Object.HasStateAuthority)
        {
            if (UfoController.main == null)
                return;
            Vector2 ufoDelta = new Vector2(transform.position.x - UfoController.main.transform.position.x, transform.position.z - UfoController.main.transform.position.z);

            IsAbducted = ufoDelta.sqrMagnitude < UfoController.main.AbductionRadius * UfoController.main.AbductionRadius;
            if (IsAbducted)
            {
                AddToScore(Runner.DeltaTime);
            }
        }
    }

    #endregion
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
        actionman.BeginAction(PlayerAction.stagger, duration);
        rigidbody.velocity = direction * strength;
    }
    #endregion
}
