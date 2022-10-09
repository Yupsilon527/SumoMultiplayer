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
        playerInput.Player.Movement.performed += ReadMovement;
        playerInput.Player.Movement.canceled += ReadMovement;

        playerInput.Player.NormalAttack.Enable();
        playerInput.Player.NormalAttack.performed += ReadNormalAttack;
        playerInput.Player.NormalAttack.canceled += ReadNormalAttack;

        playerInput.Player.StrongAttack.Enable();
        playerInput.Player.StrongAttack.performed += ReadStrongAttack;
        playerInput.Player.StrongAttack.canceled += ReadStrongAttack;

        playerInput.Player.Parry.Enable();
        playerInput.Player.Parry.performed += ReadParry;
        playerInput.Player.Parry.canceled += ReadParry;

        playerInput.Player.Dash.Enable();
        playerInput.Player.Dash.performed += ReadDash;
        playerInput.Player.Dash.canceled += ReadDash;
    }
    void ReadMovement(InputAction.CallbackContext ctx)
    {
        Vector2 movement = ctx.ReadValue<Vector2>();
        localInput.HorizontalInput = movement.x;
        localInput.VerticalInput = movement.y;
    }
    void ReadNormalAttack(InputAction.CallbackContext ctx)
    {
        localInput.Buttons.Set(ToonInput.Button.Strong, ctx.ReadValueAsButton());
    }
    void ReadStrongAttack(InputAction.CallbackContext ctx)
    {
        localInput.Buttons.Set(ToonInput.Button.Strong, ctx.ReadValueAsButton());
    }
    void ReadDash(InputAction.CallbackContext ctx)
    {
        localInput.Buttons.Set(ToonInput.Button.Dash, ctx.ReadValueAsButton());
    }
    void ReadParry(InputAction.CallbackContext ctx)
    {
        localInput.Buttons.Set(ToonInput.Button.Parry, ctx.ReadValueAsButton());
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
