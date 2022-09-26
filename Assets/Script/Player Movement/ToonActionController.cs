using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToonActionController : NetworkBehaviour
{
    private PlayerController controller = null;
    public enum PlayerAction
    {
        free,
        stagger,
        attack,
        charge,
        parry,
        dash
    }

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
            BeginAction(PlayerAction.attack,1);
        }
        if (input.Buttons.WasPressed(_buttonsPrevious, ToonInput.Button.Strong))
        {
            BeginAction(PlayerAction.charge,1);
        }
        if (input.Buttons.WasPressed(_buttonsPrevious, ToonInput.Button.Parry))
        {
            BeginAction(PlayerAction.parry,1);
        }
        if (input.Buttons.WasPressed(_buttonsPrevious, ToonInput.Button.Dash))
        {
            BeginAction(PlayerAction.dash,1);
        }
    }

    public PlayerAction currentAction = PlayerAction.free;
    float actionStart;
    TickTimer actionTime;
    public void BeginAction(PlayerAction nState, float dur)
    {
        actionStart = GameController.main.gameTimer.RemainingTime(Runner) ?? 0;
        actionTime = TickTimer.CreateFromSeconds(Runner, dur);
        currentAction = nState;
    }
    void HandleAction()
    {
        if (!IsStaggered())
        {

        }
    }

    public bool IsStaggered()
    {
        return !actionTime.ExpiredOrNotRunning(Runner);
    }
}
