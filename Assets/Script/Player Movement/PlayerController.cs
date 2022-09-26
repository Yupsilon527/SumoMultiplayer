using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public NetworkString<_16> NickName { get; private set; }

    ToonMovement mover;
    ToonAttacker attacker;
    private void Awake()
    {
        if (mover==null)
        mover =   GetComponent<ToonMovement>();
        if (attacker == null)
            attacker = GetComponent<ToonAttacker>();
    }

    public Vector3 StartPosition { get; private set; }
    public override void Spawned()
    {
        Initalize();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
    }
    public override void FixedUpdateNetwork()
    {
        HandleScoreUpdate();
    }
    void Initalize()
    {
        if (Object.HasStateAuthority)
        {
            StartPosition = transform.position;
            UIController.main.AddPlayer(Object.InputAuthority, this);
            Respawn();
        }
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

    public void Respawn()
    {
        Score = 0;
        Damage = 0;
        transform.position = StartPosition;
    }

   public bool CanAct()
    {
        return GameController.main.currentState == GameController.GameState.ingame && !attacker.IsStaggered();
    }

    public void AddToScore(float points)
    {
        Score += points;
    }

    public void TakeDamage(float damage)
    {
        Damage += damage;
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

    [Networked(OnChanged = nameof(OnDamageChanged))]
    public float Damage { get; private set; }
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
}
