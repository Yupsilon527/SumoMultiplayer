using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToonMovement : NetworkBehaviour
{
    public float MoveSpeed = 0;
    public float MaxSpeed = 0;

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
        Vector2 moveDir = new Vector2(input.HorizontalInput, input.VerticalInput);
        moveDir = moveDir.normalized;

        Vector3 force = (moveDir.x * Vector3.right + moveDir.y * Vector3.forward) * MoveSpeed * Runner.DeltaTime;
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
