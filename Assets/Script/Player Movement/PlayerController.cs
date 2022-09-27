using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ToonActionController;

public class PlayerController : NetworkBehaviour
{
    public NetworkString<_16> NickName { get; private set; }

    public ToonMovement mover;
    public ToonActionController actionman;
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
        Damage = 0;
        Rage = 0;
        transform.position = StartPosition;
    }
    public override void FixedUpdateNetwork()
    {
        HandleScoreUpdate();
    }
    #endregion
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

    public void AddToScore(float points)
    {
        Score += points;
    }

    public bool CanAct()
    {
        return GameController.main.currentState == GameController.GameState.ingame && actionman.currentAction == PlayerAction.free;
    }

    public bool CanMove()
    {
        return GameController.main.currentState == GameController.GameState.ingame && (actionman.currentAction == PlayerAction.free || actionman.currentAction == PlayerAction.charging);
    }


    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetNickName(string nickName)
    {
        if (string.IsNullOrEmpty(nickName)) return;
        NickName = nickName;
    }
    #region UI and Update RPC
    /*
    [HideInInspector]
    [Networked(OnChanged = nameof(OnNickNameChanged))]


    [HideInInspector]
    [Networked(OnChanged = nameof(OnScoreChanged))]
    public static void OnNickNameChanged(Changed<PlayerController> playerInfo)
    {
        playerInfo.Behaviour._overviewPanel.UpdateNickName(playerInfo.Behaviour.Object.InputAuthority,
            playerInfo.Behaviour.NickName.ToString());
    }*/


    [Networked(OnChanged = nameof(OnScoreChanged))]
    public float Score { get; private set; }
    public static void OnScoreChanged(Changed<PlayerController> playerInfo)
    {
        UIController.main.UpdatePlayerUI(playerInfo.Behaviour.Object.InputAuthority);
    }


    [Networked(OnChanged = nameof(OnRageChanged))]
    public float Rage { get; private set; }
    public static void OnRageChanged(Changed<PlayerController> playerInfo)
    {
        UIController.main.UpdatePlayerUI(playerInfo.Behaviour.Object.InputAuthority);
    }
    public void BuildUpRage(float rage)
    {
        Rage = Mathf.Clamp(Rage+rage,0,100);
    }
    public bool HasEnoughRage(float needed)
    {
        return Rage > needed;
    }

    [Networked(OnChanged = nameof(OnDamageChanged))]
    public float Damage { get; private set; }
    public void TakeDamageAndKnockback(float damage, float knockback, Vector3 kbCenter)
    {
        Debug.Log("Player " + name + " hit for " + damage + " damage and " + knockback + " knockback strength!");

        float damageDelta = damage / 20;
        KnockBack((transform.position - kbCenter).normalized, knockback * (.8f + damageDelta * .2f), 1 * (.5f + damage * .5f));

        Damage += damage;
    }
    public static void OnDamageChanged(Changed<PlayerController> playerInfo)
    {
        UIController.main.UpdatePlayerUI(playerInfo.Behaviour.Object.InputAuthority);
    }
    [Networked(OnChanged = nameof(OnEnterAbductionBeam))]
    public bool IsAbducted { get; private set; }
    public static void OnEnterAbductionBeam(Changed<PlayerController> playerInfo)
    {

    }
    #endregion
    #region Knockback
    void KnockBack(Vector3 direction, float strength, float duration)
    {
        actionman.BeginAction(PlayerAction.stagger, duration);
        rigidbody.velocity = direction * strength;
    }
    #endregion
}
