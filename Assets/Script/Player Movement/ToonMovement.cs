using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToonMovement : NetworkBehaviour
{
    public float MoveSpeed = 0;
    public float MoveAcceleration = 0;
    public float ChargeMultiplier = 0;

    private PlayerController controller = null;

    public override void Spawned()
    {
        controller = GetComponent<PlayerController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (controller.CanMove() && Runner.TryGetInputForPlayer(Object.InputAuthority, out ToonInput input))
        {
            Move(input);
        }
        CheckScreenBounds();
    }
    public Vector2 moveDir;
    void Move(ToonInput input)
    {
        float speedMult = controller.actionman.IsChargingAttack() ? ChargeMultiplier : 1;

        Vector3 rigidbodyvelocity = controller.rigidbody.velocity;

        moveDir = new Vector2(input.HorizontalInput, input.VerticalInput);

        //acceleration
        float deltaAcceleration = MoveAcceleration * Runner.DeltaTime / speedMult;
        rigidbodyvelocity += new Vector3(moveDir.x * deltaAcceleration, 0, moveDir.y * deltaAcceleration);

        //movespeed cap
        rigidbodyvelocity = Vector3.ClampMagnitude(rigidbodyvelocity, MoveSpeed * speedMult);

        controller.rigidbody.velocity = rigidbodyvelocity;
    }
    void CheckScreenBounds()
    {
    }
}
