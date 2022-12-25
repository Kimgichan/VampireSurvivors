using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonsterAnimator : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected AnimationClip hitAnim;
    [SerializeField] protected AnimationClip idleAnim;
    [SerializeField] protected AnimationClip deathAnim;
    [SerializeField] protected AnimationClip createAnim;


    public float AnimSpeed
    {
        set
        {
            if (value < 0f) value = 0f;
            animator.speed = value;
        }
    }

    public void OnHit()
    {
        if (hitAnim != null)
            animator.Play(hitAnim.name, -1, 0f);
    }

    public void OnIdle()
    {
        if (idleAnim != null)
            animator.Play(idleAnim.name, -1, 0f);
    }

    public void OnCreate()
    {
        if (createAnim != null)
            animator.Play(createAnim.name, -1, 0f);
    }

    public void OnDeath()
    {
        if (deathAnim != null)
            animator.Play(deathAnim.name, -1, 0f);
    }

    #region Æó±â Ã³¸®

    public void AddCreateCall(UnityAction createCall)
    {

    }

    public void ClearCreateCall()
    {

    }
    public void AddDeathCall(UnityAction deathCall)
    {

    }

    public void ClearDeathCall()
    {

    }
    #endregion
}
