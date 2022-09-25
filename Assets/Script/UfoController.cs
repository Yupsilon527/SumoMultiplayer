using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UfoController : NetworkBehaviour
{
    public float AbductionRadius = 1;

    public float MinWaitInterval = 6;
    public float MaxWaitInterval = 6;
    public float MinMoveCircle = .5f;
    public float MoveCircle = 3;
    public float MoveSpeed = 3;

    public static UfoController main;
    [Networked] private TickTimer behaviorTimer { get; set; }

    public Vector2 desiredPosition = Vector2.zero;
    public Vector2 realPosition = Vector2.zero;

    private void Awake()
    {
        main = this;
    }
    public override void Spawned()
    {
        OnGameReset();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority && GameController.main.currentState == GameController.GameState.ingame)
        {
            MoveUfo();
        }
    }
    public void OnGameReset()
    {
        if (Object.HasStateAuthority)
        {
            realPosition = Vector2.zero;
            transform.position = new Vector3(realPosition.x, transform.position.y, realPosition.y);

            desiredPosition = Vector2.zero;
        }
    }
    void MoveUfo()
    {
        float displacement = MoveSpeed * Runner.DeltaTime;
        if ((realPosition - desiredPosition).sqrMagnitude < displacement * displacement)
        {
            if (desiredPosition != realPosition)
            {
                realPosition = desiredPosition;
                transform.position = new Vector3(realPosition.x, transform.position.y, realPosition.y);

                float wait = GetRandomWaitInterval();
                if (wait > 0)
                {
                    behaviorTimer = TickTimer.CreateFromSeconds(Runner, wait);
                }
            }
            else if (behaviorTimer.ExpiredOrNotRunning(Runner))
            {
                MoveToNextRandomLocation();
            }
        }
        else
        {
            realPosition += (desiredPosition - realPosition).normalized * displacement;
            transform.position = new Vector3(realPosition.x, transform.position.y, realPosition.y);
        }
    }
    float GetRandomWaitInterval()
    {
        return Random.Range(MinWaitInterval, MaxWaitInterval);
    }
    void MoveToNextRandomLocation()
    {
        while ((desiredPosition - realPosition).sqrMagnitude < MinMoveCircle * MinMoveCircle)
        {
            desiredPosition = Random.insideUnitCircle * MoveCircle;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(Vector3.zero, MoveCircle);
    }
}
