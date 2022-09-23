using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : NetworkBehaviour, INetworkRunnerCallbacks
{
    public float PreGameTime = 3;
    public float RoundTime = 180;

    public enum GameState
    {
        lobby,
        pregame,
        ingame,
        postgame,
    }

    public NetworkRunner runner;
    private List<NetworkBehaviourId> _playerDataNetworkedIds = new List<NetworkBehaviourId>();

    [Networked] private TickTimer gameTimer { get; set; }
    [Networked] private GameState currentState { get; set; }

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
    }

    public override void FixedUpdateNetwork()
    {
        HandleState();
        if (gameTimer.ExpiredOrNotRunning(runner))
            HandleTimer();
    }
    #endregion


    #region Initialization
    void InitializeRoom()
    {
        if (IsServer())
        {
            PlayerSpawners.SpawnPlayers();
            UfoSpawner.SpawnHazards(runner);
        }
    }
    void RestartGame()
    {

    }
    #endregion
    void HandleTimer()
    {
        switch (currentState)
        {
            case GameState.pregame:
                ChangeState(GameState.ingame);
                break;
            case GameState.ingame:
                ChangeState(GameState.postgame);
                break;
        }
    }
    void HandleState()
    {

    }

    void StartTheGame()
    {
        currentState = GameState.ingame;
    }
    void ChangeState(GameState newstate)
    {
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
    }


    #region Lobby
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    private void OnGUI()
    {
        if (runner == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
        else
        {
            if (runner.IsServer)
            {
                if (GUI.Button(new Rect(0, 40, 200, 40), "Play"))
                {
                    ChangeState(GameState.pregame);
                }
            }
        }
    }

    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        // Start or join (depends on gamemode) a session with a specific name
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
    #endregion

    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            NetworkObject networkPlayerObject = PlayerSpawners.Spawn(runner, player);
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }
}
