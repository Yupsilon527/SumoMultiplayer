using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class ToonInputPoller : MonoBehaviour, INetworkRunnerCallbacks
{
    public PlayerInput playerInput;
    private void Awake()
    {
        if (playerInput==null)
        playerInput = GetComponent<PlayerInput>();
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        ToonInput localInput = new ToonInput();

        var inputAction = playerInput.actions.FindAction("ASD");

        localInput.HorizontalInput = playerInput.actions.FindAction("Horizontal").ReadValue<float>();
        localInput.VerticalInput = playerInput.actions.FindAction("Vertical").ReadValue<float>();
        localInput.Buttons.Set(ToonInput.Button.Weak, playerInput.actions.FindAction("Normal Attack").IsPressed());
        localInput.Buttons.Set(ToonInput.Button.Strong, playerInput.actions.FindAction("Strong Attack").IsPressed());
        localInput.Buttons.Set(ToonInput.Button.Dash, playerInput.actions.FindAction("Dash").IsPressed());
        localInput.Buttons.Set(ToonInput.Button.Parry, playerInput.actions.FindAction("Parry").IsPressed());

        input.Set(localInput);
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
        byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
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
}
