using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToonMovement : NetworkBehaviour
{
    public float MoveSpeed = 0;
    public float MaxSpeed = 0;
    public float AngSpeed = 0;

    private Rigidbody rigidbody = null;
    private PlayerController controller = null;

    public override void Spawned()
    {
        rigidbody = GetComponent<Rigidbody>();
        controller = GetComponent<PlayerController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.TryGetInputForPlayer(Object.InputAuthority, out ToonInput input))
        {
            Move(input);
        }

        CheckScreenBounds();
    }

    void Move(ToonInput input)
    {
        Quaternion rot = rigidbody.rotation *
                         Quaternion.Euler(0, input.HorizontalInput * AngSpeed * Runner.DeltaTime, 0);
        rigidbody.MoveRotation(rot);

        Vector3 force = (rot * Vector3.forward) * input.VerticalInput * MoveSpeed * Runner.DeltaTime;
        rigidbody.AddForce(force);

        if (rigidbody.velocity.magnitude > MaxSpeed)
        {
            rigidbody.velocity = rigidbody.velocity.normalized * MaxSpeed;
        }
    }
    void CheckScreenBounds()
    {
    }
}
