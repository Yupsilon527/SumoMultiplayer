using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    PlayerController player;
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
    }
    public void PlaySpecific(string animation)
    {
        animator.Play(animation);
    }
    private void Update()
    {
        animator.SetBool("facesright", player.mover.facesRight);
        animator.SetBool("walking", player.mover.moveDir.sqrMagnitude>0);
        animator.SetBool("charging", player.actionman.currentAction == ToonActionController.PlayerAction.charging);
        animator.SetBool("staggered", player.actionman.currentAction == ToonActionController.PlayerAction.stagger);
    }
}
