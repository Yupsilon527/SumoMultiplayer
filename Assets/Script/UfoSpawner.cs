using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UfoSpawner : NetworkSpawner
{
    public void SpawnHazards(NetworkRunner runner)
    {
        Spawn(runner, PlayerRef.None);
    }
}
