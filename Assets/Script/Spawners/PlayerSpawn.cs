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
    public void SpawnPlayers()
    {
        if (!Object.HasStateAuthority) return;
        InitSpawns();

        foreach (PlayerRef player in Runner.ActivePlayers)
        {
            SpawnPlayer(player);
        }

    }

    public void PlayerJoined(PlayerRef player)
    {
       if (GameController.main.currentState >= GameController.GameState.pregame) return;
        SpawnPlayer(player);
    }

    public void PlayerLeft(PlayerRef player)
    {
        DespawnPlayer(player);
    }
    #endregion

    #region Spawns
    private Transform[] spawnPoints = null;
    void InitSpawns()
    {
        spawnPoints =new Transform[transform.childCount];
        for (int i = 0; i<spawnPoints.Length; i++)
        {
            spawnPoints[i] = transform.GetChild(i);
        }
    }

    private void SpawnPlayer(PlayerRef player)
    {
        Transform spawnPoint = spawnPoints[player % spawnPoints.Length];

        var playerObject = Spawn(spawnPoint.position, player);
        Runner.SetPlayerObject(player, playerObject);
    }

    private void DespawnPlayer(PlayerRef player)
    {
        if (Runner.TryGetPlayerObject(player, out var playerNetworkObject))
        {
            Runner.Despawn(playerNetworkObject);
        }
        Runner.SetPlayerObject(player, null);
    }
    #endregion
}
