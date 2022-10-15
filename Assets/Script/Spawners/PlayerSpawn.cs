using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : NetworkSpawner, IPlayerJoined, IPlayerLeft, ISpawned
{
    #region Network Object
    public override void Spawned()
    {

    }

    public void PlayerJoined(PlayerRef player)
    {
        if (!Object.HasStateAuthority) return;
        if (GameController.main.currentState != GameController.GameState.pregame) return;
            SpawnPlayer(player);
    }

    public void PlayerLeft(PlayerRef player)
    {
        DespawnPlayer(player);
    }
    #endregion

    #region Register Players
    public Dictionary<PlayerRef, PlayerController> RegisteredPlayers = new Dictionary<PlayerRef, PlayerController>();
    public void RegisterPlayer(PlayerRef playerRef, PlayerController controller)
    {
        if (RegisteredPlayers.ContainsKey(playerRef)) return;
        if (controller == null) return;

        RegisteredPlayers.Add(playerRef, controller);
    }
    public void RemovePlayer(PlayerRef playerRef)
    {
        if (RegisteredPlayers.TryGetValue(playerRef, out PlayerController controller) == false) return;

        if (controller != null)
        {
            controller.gameObject.SetActive(false);
        }

        RegisteredPlayers.Remove(playerRef);
    }
    #endregion
    #region Spawns
    private Transform[] spawnPoints = null;
    public void InitSpawns()
    {
        spawnPoints =new Transform[transform.childCount];
        for (int i = 0; i<spawnPoints.Length; i++)
        {
            spawnPoints[i] = transform.GetChild(i);
        }
    }

    public void RespawnAllPlayers()
    {

        foreach (PlayerRef player in Runner.ActivePlayers)
        {
                SpawnPlayer(player);
        }
    }

    private void SpawnPlayer(PlayerRef player)
    {
        if (RegisteredPlayers.ContainsKey(player) && RegisteredPlayers[player] != null)
            return;

        Transform spawnPoint = spawnPoints[player.PlayerId % spawnPoints.Length];

        NetworkObject playerObject = Spawn(spawnPoint.position, player);
        Runner.SetPlayerObject(player, playerObject);
        if (playerObject.TryGetComponent(out PlayerController controller))
        {
            RegisterPlayer(player, controller);
        }
    }

    private void DespawnPlayer(PlayerRef player)
    {
        if (Runner.TryGetPlayerObject(player, out var playerNetworkObject))
        {
            Runner.Despawn(playerNetworkObject);
        }
        RemovePlayer(player);
        Runner.SetPlayerObject(player, null);
    }
    #endregion
}
