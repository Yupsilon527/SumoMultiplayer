using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSpawner : MonoBehaviour
{
    public NetworkPrefabRef Prefab;
    public int SpawnedInstances = 0;
    public virtual NetworkObject Spawn(NetworkRunner runner, PlayerRef player)
    {
        NetworkObject myInstance = runner.Spawn(Prefab, transform.position, Quaternion.identity, player);
        SpawnedInstances++;
        return myInstance;
    }
}