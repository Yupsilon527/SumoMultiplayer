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
        // You can do cleaner code
        if(SoundSelector.Instance!=null)
            SoundSelector.Instance.PlayGameMusicScheduled(AudioSettings.dspTime+2);
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

    public PlayerSpawn PlayerSpawners;
    public UfoSpawner UfoSpawner;

    #region NetworkFunctions
    public override void Spawned()
    {
        InitializeRoom();
        if (TryGetComponent(out GameDisconnectManager gdman))
            Runner.AddCallbacks(gdman);
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

        foreach (IRespawnable respawnable in UfoController.main.GetComponents<IRespawnable>())
        {
            respawnable.Respawn();
        }
        UfoController.main.Respawn();
        foreach (KeyValuePair<PlayerRef, PlayerController> Player in PlayerSpawners.RegisteredPlayers)
        {
            Player.Value.Respawn();
            foreach (IRespawnable respawnable in Player.Value.GetComponents<IRespawnable>())
            {
                respawnable.Respawn();
            }
        }
        if (Object.HasStateAuthority)
        {

            PlayerSpawners.RespawnAllPlayers();
            gameTimer = TickTimer.CreateFromSeconds(Runner, PreGameTime);
            WinningPlayer = null;
        }
    }
    #endregion
    #region Initialization
    void InitializeRoom()
    {
        if (Object.HasStateAuthority)
        {
                PlayerSpawners.InitSpawns();
                UfoSpawner.SpawnHazards();
            
        }
        if (currentState == GameState.ingame)
        {
            UIController.main.ShowInGameScreen();
        }
    }
    public void StartNewGame()
    {
        currentState = GameState.pregame;

        //You can do cleaner code--------------------------------------------------------
        if(SoundSelector.Instance!=null)
            SoundSelector.Instance.floatValue=0;
    }
    #endregion
    #region Winning and Game Over
    [HideInInspector][Networked] public PlayerController WinningPlayer { get; set; }
    public void CheckGameOver()
    {
        if (Object.HasStateAuthority) 
            DeclareWinner(false);

        if (WinningPlayer != null)
        {
            currentState = GameState.postgame;
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
    void PlayEndOfGameAnimation()
    {
        if (WinningPlayer != null)
        {
            WinningPlayer.animations.PlaySpecific("Victory");
            WinningPlayer.animations.SetAbducted(false);
            if (UfoController.main != null)
            {
                UfoController.main.AbductCharacter(WinningPlayer.character.GetCurrentCharacter());
            }
        }
    }
    #endregion
    #region States
    void HandleTimer()
    {
        //Debug.Log("[GameController] Handle timer expire at state: "+currentState);
        switch (currentState)
        {
            case GameState.pregame:
                currentState = GameState.ingame;
                break;
            case GameState.ingame:
                if (RoundTime > 0)
                {
                    DeclareWinner(true);
                    currentState = GameState.postgame;
                }
                break;
            default:
                gameTimer = default;
                break;
        }
    }

    [Networked(OnChanged = nameof(ChangeState))] public GameState currentState { get; set; }
    public static void ChangeState(Changed<GameController> gameInfo)
    {
        GameController game = gameInfo.Behaviour;

        Debug.Log("[GameController] Change state to " + game.currentState);
        switch (game.currentState)
        {
            case GameState.pregame:
                game.HandleRestart();
                UIController.main.ShowInGameScreen();
                break;
            case GameState.ingame:
                if (game.Object.HasStateAuthority && game.RoundTime >0)
                {
                    game.gameTimer = TickTimer.CreateFromSeconds(game.Runner, game.RoundTime);
                }
                break;
            case GameState.postgame:
                UIController.main.ShowEndOfGameScreen();
                game.PlayEndOfGameAnimation();
                break;
        }
    }

    #endregion
}
