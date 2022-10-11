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
    public PlayerAnimationController animations;
    public CharacterResolver character;
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
        if (animations == null)
            animations = GetComponent<PlayerAnimationController>();
        if (audio == null)
            audio = GetComponent<AudioSource>();

        if (character == null)
            character = GetComponent<CharacterResolver>();
    }

    public override void Spawned()
    {
        Initalize();
    }
    void Initalize()
    {
        StartPosition = transform.position;
        if (Object.HasStateAuthority)
        {
            Respawn();
        }
        InitalizeCharacter();
        UIController.main.AddPlayer(Object.InputAuthority, this);
    }
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        UIController.main.RemovePlayer(Object.InputAuthority);
    }
    #region Respawn
    public Vector3 StartPosition { get; private set; }
    public void Respawn()
    {
        Score = 0;
        if (TryGetComponent(out NetworkRigidbody netrig))
        {
            netrig.TeleportToPosition(StartPosition);
        }
    }
    public override void FixedUpdateNetwork()
    {
        HandleScoreUpdate();
        animations.SetHighlighted(GameController.main.currentState == GameController.GameState.pregame && Object.HasInputAuthority);
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

    public float ScoreDecay = .5f;
    [Networked(OnChanged = nameof(OnEnterAbductionBeam))]
    public bool IsAbducted { get; private set; }
    public static void OnEnterAbductionBeam(Changed<PlayerController> playerInfo)
    {

    }
    public void AddToScore(float points)
    {
        Score = Mathf.Max(Score + points,0);
        GameController.main.CheckGameOver();
    }

    [Networked(OnChanged = nameof(OnScoreChanged))]
    public float Score { get; private set; }
    public static void OnScoreChanged(Changed<PlayerController> playerInfo)
    {
        UIController.main.UpdatePlayerUI(playerInfo.Behaviour.Object.InputAuthority);
    }
    void HandleScoreUpdate()
    {
        if (Object.HasStateAuthority && GameController.main.currentState == GameController.GameState.ingame)
        {
            if (UfoController.main == null)
                return;
            Vector2 ufoDelta = new Vector2(transform.position.x, transform.position.z) - UfoController.main.realPosition;

            IsAbducted = ufoDelta.sqrMagnitude < UfoController.main.AbductionRadius * UfoController.main.AbductionRadius;
            if (IsAbducted)
            {
                AddToScore(Runner.DeltaTime);
            }
            else 
            {
                AddToScore( - Runner.DeltaTime * ScoreDecay);
            }
        }
    }

    #endregion
    #region Character
    [Networked]    int CharacterID { get; set; }
    void InitalizeCharacter()
    {
        if (Object.HasStateAuthority)
        {
            int MaxChars = character.SwappableCharacters.Length;

            CharacterID = Random.Range(0, MaxChars - 1);
            while (CharacterExists(CharacterID))
            {
                CharacterID = (CharacterID + 1) % MaxChars;
            }
        }
        character.ChangeCharacter(CharacterID);
    }
    bool CharacterExists(int chID)
    {
        foreach (KeyValuePair<PlayerRef, PlayerController> player in GameController.main.PlayerSpawners.RegisteredPlayers)
        {
            if (player.Value!= this && player.Value.CharacterID == chID)
                return true;
        }

        return false;
    }
    #endregion
}
