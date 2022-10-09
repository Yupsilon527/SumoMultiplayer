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
    public MasterInput playerInput;
   public ToonInput localInput;
    private void Awake()
    {
            playerInput = new MasterInput();
        localInput = new ToonInput();

        playerInput.Player.Movement.Enable();
        playerInput.Player.Movement.performed += ctx =>
        {
            Vector2 movement = ctx.ReadValue<Vector2>();
            localInput.HorizontalInput = movement.x;
            localInput.VerticalInput = movement.y;
        };
        playerInput.Player.NormalAttack.Enable();
        playerInput.Player.NormalAttack.performed += ctx =>
        {
            localInput.Buttons.Set(ToonInput.Button.Weak, ctx.ReadValueAsButton());
        };
        playerInput.Player.StrongAttack.Enable();
        playerInput.Player.StrongAttack.performed += ctx =>
        {
            localInput.Buttons.Set(ToonInput.Button.Strong, ctx.ReadValueAsButton());
        };

        playerInput.Player.Parry.Enable();
        playerInput.Player.Parry.performed += ctx =>
        {
            localInput.Buttons.Set(ToonInput.Button.Parry, ctx.ReadValueAsButton());
        };

        playerInput.Player.Dash.Enable();
        playerInput.Player.Dash.performed += ctx =>
        {
            localInput.Buttons.Set(ToonInput.Button.Dash, ctx.ReadValueAsButton());
        };
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
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
