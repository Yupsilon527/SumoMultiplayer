using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToonController : NetworkBehaviour
{
    private Rigidbody rigidbody = null;
    private PlayerController controller = null;

    public override void Spawned()
    {
        rigidbody = GetComponent<Rigidbody>();
        controller = GetComponent<PlayerController>();
    }
}
