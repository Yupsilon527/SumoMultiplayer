using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkAnimationResponder : SimulationBehaviour
{
    public Animator animator;

    private void Awake()
    {
        if (animator == null)
        animator = GetComponent<Animator>();
    }
    public void PlaySpecific(string animation)
    {
        animator.Play(animation);
    }
}
