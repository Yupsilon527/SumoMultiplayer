using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : NetworkBehaviour
{
    public static GameController main;
    private void Awake()
    {
        main = this;
    }
    public float PreGameTime = 3;
    public float RoundTime = 180;

    public enum GameState
    {
        lobby,
        pregame,
        ingame,
        postgame,
    }

    private List<NetworkBehaviourId> _playerDataNetworkedIds = new List<NetworkBehaviourId>();

    [Networked] private TickTimer gameTimer { get; set; }
    [Networked] public GameState currentState { get; set; }

    public PlayerSpawn PlayerSpawners;
    public UfoSpawner UfoSpawner;

    bool IsServer()
    {
        return Object.HasStateAuthority;
    }
    #region NetworkFunctions
    public override void Spawned()
    {
        InitializeRoom();
        StartTheGame();
    }

    public override void FixedUpdateNetwork()
    {
        HandleState();
        if (gameTimer.Expired(Runner))
        {
            HandleTimer();
        }
    }
    #endregion


    #region Initialization
    void InitializeRoom()
    {
        if (IsServer())
        {
            PlayerSpawners.SpawnPlayers();
            UfoSpawner.SpawnHazards();
        }
    }
    void StartTheGame()
    {
        if (IsServer())
            ChangeState(GameState.pregame);
    }
    void RestartGame()
    {

    }
    #endregion
    #region States
    void HandleTimer()
    {
        Debug.Log("[GameController] Handle timer expire at state: "+currentState);
        switch (currentState)
        {
            case GameState.pregame:
                ChangeState(GameState.ingame);
                break;
            case GameState.ingame:
                ChangeState(GameState.postgame);
                break;
        }
        gameTimer = default;
    }
    void HandleState()
    {

    }

    public void ChangeState(GameState newstate)
    {
        Debug.Log("[GameController] Change state to " + newstate);
        switch (newstate)
        {
            case GameState.pregame:
                if (IsServer())
                gameTimer = TickTimer.CreateFromSeconds(Runner, PreGameTime);
                break;
            case GameState.ingame:
                if (IsServer())
                    gameTimer = TickTimer.CreateFromSeconds(Runner, RoundTime);
                break;
        }
        currentState = newstate;
    }
    #endregion
}
