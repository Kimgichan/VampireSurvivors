using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip attackAnim;

    public float AnimSpeed
    {
        set
        {
            if (value < 0f) value = 0f;
            animator.speed = value;
        }
    }

    public void OnAttack()
    {
        if (attackAnim != null)
            animator.Play(attackAnim.name, -1, 0f);
    }
}
