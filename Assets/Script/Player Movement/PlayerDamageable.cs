using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ToonActionController;

public class PlayerDamageable : NetworkBehaviour, IRespawnable
{
    private PlayerController controller = null;

    public override void Spawned()
    {
        controller = GetComponent<PlayerController>();
    }
    public void Respawn()
    {
     //   Rage = 0;
      //  Damage = 0;
    }
}
