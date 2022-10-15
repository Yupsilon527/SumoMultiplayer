using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToonMovement : NetworkBehaviour, IRespawnable
{
    public float MoveSpeed = 0;
    public float MoveAcceleration = 0;
    public float ChargeMultiplier = 0;

    private PlayerController controller = null;

    public override void Spawned()
    {
        controller = GetComponent<PlayerController>();
    }
    public void Respawn()
    {
        if (!Object.HasStateAuthority) return;
        moveDir *= 0;
        controller.rigidbody.velocity *= 0;
        facesRight = transform.position.x < 0;
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            if (controller.CanMove() && Runner.TryGetInputForPlayer(Object.InputAuthority, out ToonInput input))
            {
                Move(input);
            }
        }
    }
   [Networked(OnChanged =nameof(OnPlayerFacingChange))] 
    public bool facesRight { get; set; }
    [Networked(OnChanged = nameof(OnPlayerWalkingChange))]
    public Vector2 moveDir { get; set; }
    void Move(ToonInput input)
    {
        float speedMult = controller.actionman.IsChargingAttack() ? ChargeMultiplier : 1;

        Vector3 rigidbodyvelocity = controller.rigidbody.velocity;

        moveDir = new Vector2(input.HorizontalInput, input.VerticalInput);
        if (moveDir.x != 0)
        {
            facesRight = moveDir.x > 0;
        }

        //acceleration
        float deltaAcceleration = MoveAcceleration * Runner.DeltaTime / speedMult;
        rigidbodyvelocity += new Vector3(moveDir.x * deltaAcceleration, 0, moveDir.y * deltaAcceleration);

        //movespeed cap
        rigidbodyvelocity = Vector3.ClampMagnitude(rigidbodyvelocity, MoveSpeed * speedMult);

        controller.rigidbody.velocity = rigidbodyvelocity;
    }
    public Vector3 GetVectorForward()
    {
        return facesRight ? Vector3.right : Vector3.left;
    }
    public static void OnPlayerFacingChange(Changed<ToonMovement> playerInfo)
    {
        if (playerInfo.Behaviour.controller == null)
            return;
        playerInfo.Behaviour.controller.animations.SetFacing(playerInfo.Behaviour.facesRight);
    }
    public static void OnPlayerWalkingChange(Changed<ToonMovement> playerInfo)
    {
        if (playerInfo.Behaviour.controller == null)
            return;
        playerInfo.Behaviour.controller.animations.SetWalking(playerInfo.Behaviour.moveDir.x != 0 || playerInfo.Behaviour.moveDir.y != 0);
    }
}
