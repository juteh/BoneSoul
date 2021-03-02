using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{

    private string currentState;
    public void ChangeAnimationState(string newState, Animator animator)
    {
        if (currentState == newState) return;

        animator.Play(newState, -1);

        currentState = newState;
    }
}
