using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSpawner : NetworkBehaviour
{
    public NetworkPrefabRef Prefab = NetworkPrefabRef.Empty;
    public int SpawnedInstances = 0;
    public virtual NetworkObject Spawn( PlayerRef player)
    {
        return Spawn(transform.position, player);
    }
    public virtual NetworkObject Spawn(Vector3 position, PlayerRef player)
    {
        NetworkObject myInstance = Runner.Spawn(Prefab, position, Quaternion.identity, player);
        SpawnedInstances++;
        return myInstance;
    }
}