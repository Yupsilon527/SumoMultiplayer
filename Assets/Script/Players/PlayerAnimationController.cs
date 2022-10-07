using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : SimulationBehaviour
{
    PlayerController player;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
    }
    public void SetFacing(bool right)
    { animator.SetBool("facesright", right); }
    public void SetWalking(bool walking)
    { animator.SetBool("walking", walking); }
    public void SetCharging(bool charging)
    { animator.SetBool("charging", charging); }
    public void SetStaggered(bool staggered)
    {
        animator.SetBool("staggered", staggered);
    }
    public void SetHighlighted(bool highlight)
    {
        animator.SetBool("highlighted", highlight);
    }
    public void PlaySpecific(string animation)
    {
        animator.Play(animation);
    }
}
