using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ToonInput;
using Random = UnityEngine.Random;

public class LobbyController : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner _networkRunnerPrefab = null;
    public NetworkRunner runner;
    public int MaxPlayersCount = 4;

    [Header("Components")]
    public GameObject MainMenu;
    public GameObject HostMenu;
    public GameObject JoinMenu;
    public GameObject LobbyMenu;

    public enum LobbyWindow
    {
        Main,
        Host,
        Join,
        Lobby
    }

    public void ChangeMenu(int Window)
    {
        ChangeMenu((LobbyWindow)Window);
    }

    public void ChangeMenu(LobbyWindow Window)
    {
        MainMenu.gameObject.SetActive(Window == LobbyWindow.Main);
        HostMenu.gameObject.SetActive(Window == LobbyWindow.Host);
        JoinMenu.gameObject.SetActive(Window == LobbyWindow.Join);
        LobbyMenu.gameObject.SetActive(Window == LobbyWindow.Lobby);
    }

    #region Lobby
    private void Awake()
    {
        ChangeMenu(LobbyWindow.Main);
            OnRoomNameChanged("Room " + Random.Range(1, 100));
    }

    string SessionName = "TestRoom";
    public void OnRoomNameChanged(string newName)
    {
        SessionName = newName;
        foreach (TMP_InputField field in GetComponentsInChildren<TMP_InputField>())
        {
            field.text = SessionName;
        }
    }
    public void OnHostButtonPressed()
    {
        StartGame(GameMode.Host);
    }
    public void OnJoinButtonPressed()
    {
        StartGame(GameMode.Client);
    }
    public void OnDisconnectButtonPressed()
    {
        runner.Shutdown();
    }

    async void StartGame(GameMode mode)
    {
        runner = FindObjectOfType<NetworkRunner>();
        if (runner == null)
        {
            runner = Instantiate(_networkRunnerPrefab);
        }
        //runner.AddCallbacks(this);
        runner.ProvideInput = true;
        Debug.Log("JoinSession " + SessionName);

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = SessionName,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>(),
            PlayerCount = MaxPlayersCount
        });
        Debug.Log("Joined " + SessionName);
        ChangeMenu(LobbyWindow.Lobby);
    }
    #endregion

    public void OnConnectedToServer(NetworkRunner runner)
    {
        ChangeMenu(LobbyWindow.Lobby);
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        ChangeMenu(LobbyWindow.Main);
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        ChangeMenu(LobbyWindow.Main);
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
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
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
        ChangeMenu(LobbyWindow.Main);
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

}
