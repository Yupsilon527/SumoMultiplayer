using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameRoomController : MonoBehaviour, INetworkRunnerCallbacks
{
    public string GameScene = "Game";
    public TextMeshProUGUI RoomName;
    public TextMeshProUGUI PlayerLabel;
    public Button PlayButton;
    NetworkRunner runner; 

    private void OnEnable()
    {
        foreach (NetworkRunner NR in NetworkRunner.Instances)
        {
            NR.AddCallbacks(this);
            runner = NR;
        }
        StartCoroutine(WaitForServer());
    }

    IEnumerator WaitForServer()
    {
        RoomName.text = "...";
        PlayerLabel.text = "...";
        while (runner == null || !runner.SessionInfo.IsValid)
        {
            yield return new WaitForEndOfFrame();
        }
        UpdateRoomInfo();
        UpdatePlayerInfo();
    }

    public void OnPlayPressed()
    {
        if (runner != null && runner.IsServer)
        {
            runner.SessionInfo.IsVisible = false;
            runner.SetActiveScene(GameScene);            
        }
    }

    void UpdateRoomInfo()
    {
        if (runner == null) return;
        if (runner.SessionInfo.IsVisible)
            RoomName.text = "Public Room";
        else
            RoomName.text = runner.SessionInfo.Name;
        PlayButton.interactable = runner.IsServer;
    }
    void UpdatePlayerInfo()
    {
        if (runner == null) return;
        PlayerLabel.text = "Players: " + runner.ActivePlayers.Count();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        UpdateRoomInfo();
        UpdatePlayerInfo();
    }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        UpdateRoomInfo();
        UpdatePlayerInfo();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        UpdatePlayerInfo();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        gameObject.SetActive(false);
    }
    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        gameObject.SetActive(false); 
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }



    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
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


    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

}
