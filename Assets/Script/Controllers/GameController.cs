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
    public float WinScore = 100;

    public enum GameState
    {
        lobby,
        pregame,
        ingame,
        postgame,
    }


    [Networked] public TickTimer gameTimer { get; set; }
    [Networked] public GameState currentState { get; set; }

    public PlayerSpawn PlayerSpawners;
    public UfoSpawner UfoSpawner;

    #region NetworkFunctions
    public override void Spawned()
    {
        InitializeRoom();
        UIController.main.ShowInGameScreen();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            if (currentState == GameState.lobby && CheckAllPlayersLoaded())
            {
                StartNewGame();
            }
            else
            {
                PlayerSpawners.RespawnAllPlayers();
            }
        }

        UIController.main.UpdateTimer();
        if (gameTimer.Expired(Runner))
        {
            HandleTimer();
        }
    }
    bool CheckAllPlayersLoaded()
    {
        foreach (PlayerRef player in Runner.ActivePlayers)
        {
            if (!PlayerSpawners.RegisteredPlayers.ContainsKey(player))
                return false;
        }
        return true;
    }
    #endregion
    #region Game Start And Restart
     void HandleRestart()
    {
        if (!Object.HasStateAuthority) return;

        foreach (IRespawnable respawnable in UfoController.main.GetComponents<IRespawnable>())
        {
            respawnable.Respawn();
        }
        foreach (KeyValuePair<PlayerRef, PlayerController> Player in PlayerSpawners.RegisteredPlayers)
            foreach (IRespawnable respawnable in Player.Value.GetComponents<IRespawnable>())
            {
                respawnable.Respawn();
            }
        WinningPlayer = null;
    }
    #endregion
    #region Initialization
    void InitializeRoom()
    {
        if (Object.HasStateAuthority)
        {
            if (Object.HasStateAuthority)
            {
                PlayerSpawners.InitSpawns();
                UfoSpawner.SpawnHazards();
            }
        }
    }
    public void StartNewGame()
    {
        HandleRestart();
        ChangeState(GameState.pregame);
        UIController.main.ShowInGameScreen();
    }
    [HideInInspector][Networked] public PlayerController WinningPlayer { get; set; }
    public void CheckGameOver()
    {
        if (Object.HasStateAuthority) 
            DeclareWinner(false);

        if (WinningPlayer != null)
        {
            ChangeState(GameState.postgame);
        }
    }
    void DeclareWinner(bool forced)
    {
        foreach (KeyValuePair<PlayerRef, PlayerController> Player in PlayerSpawners.RegisteredPlayers)
        {
            if (forced ||Player.Value.Score >= WinScore)
            {
                if (WinningPlayer == null || WinningPlayer.Score < Player.Value.Score)
                {
                    WinningPlayer = Player.Value;
                }
            }
        }
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
                DeclareWinner(true);
                ChangeState(GameState.postgame);
                break;
            default:
                gameTimer = default;
                break;
        }
    }

    public void ChangeState(GameState newstate)
    {
        Debug.Log("[GameController] Change state to " + newstate);
        switch (newstate)
        {
            case GameState.pregame:
                if (Object.HasStateAuthority)
                {
                    PlayerSpawners.RespawnAllPlayers();
                    gameTimer = TickTimer.CreateFromSeconds(Runner, PreGameTime);
                }
                break;
            case GameState.ingame:
                if (Object.HasStateAuthority && RoundTime>0)
                {
                    gameTimer = TickTimer.CreateFromSeconds(Runner, RoundTime);
                }
                break;
            case GameState.postgame:
                UIController.main.ShowEndOfGameScreen();
                break;
        }
        currentState = newstate;
    }

    #endregion
}
