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

    public override void FixedUpdateNetwork()
    {
        if (controller.CanAct() && Runner.TryGetInputForPlayer(Object.InputAuthority, out ToonInput input))
        {
            HandlePlayerInput(input);
        }
    }
    [Networked] private NetworkButtons _buttonsPrevious { get; set; }
    void HandlePlayerInput(ToonInput input)
    {
        if (input.Buttons.WasPressed(_buttonsPrevious, ToonInput.Button.Stab))
        {
            DoFastAttack();
        }
        if (input.Buttons.WasPressed(_buttonsPrevious, ToonInput.Button.Strong))
        {
            DoStrongAttack();
        }
        if (input.Buttons.WasPressed(_buttonsPrevious, ToonInput.Button.Parry))
        {
            DoParry();
        }
        if (input.Buttons.WasPressed(_buttonsPrevious, ToonInput.Button.Dash))
        {
            DoDash();
        }
    }
    public void DoFastAttack()
    {

    }
    public void DoStrongAttack()
    {

    }
    public void DoParry()
    {

    }
    public void DoDash()
    {

    }

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
