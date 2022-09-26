using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToonAttacker : NetworkBehaviour
{
    private PlayerController controller = null;

    public override void Spawned()
    {
        controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()

    float staggerStart;
    TickTimer staggerTimer;

    void Stagger(float dur)
    {
        staggerStart = GameController.main.gameTimer.RemainingTime(Runner) ?? 0;
        staggerTimer = TickTimer.CreateFromSeconds(Runner, dur);
    }
    public bool IsStaggered()
    {
        return !staggerTimer.ExpiredOrNotRunning(Runner);
    }
}
