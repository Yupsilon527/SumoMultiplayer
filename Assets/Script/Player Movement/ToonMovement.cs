using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToonMovement : NetworkBehaviour
{
    public float MoveSpeed = 0;
    public float MoveAcceleration = 0;

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
        Vector3 rigidbodyvelocity = rigidbody.velocity;
        Vector2 moveDir = new Vector2(input.HorizontalInput, input.VerticalInput);

        //acceleration
        float deltaAcceleration = MoveAcceleration * Runner.DeltaTime;
        rigidbodyvelocity += new Vector3(moveDir.x * deltaAcceleration, 0, moveDir.y * deltaAcceleration);

        //movespeed cap
        rigidbodyvelocity = Vector3.ClampMagnitude(rigidbodyvelocity, MoveSpeed);

        rigidbody.velocity = rigidbodyvelocity;
    }
    void CheckScreenBounds()
    {
    }
}
