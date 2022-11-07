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

    protected UnityAction deathCall;
    protected UnityAction createCall;

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
        else CreateCall();
    }

    public void OnDeath()
    {
        if (deathAnim != null)
            animator.Play(deathAnim.name, -1, 0f);
        else DeathCall();
    }

    public void DeathCall()
    {
        if (deathCall != null)
            deathCall();
    }
    public void CreateCall()
    {
        if (createCall != null)
            createCall();
    }

    public void AddCreateCall(UnityAction createCall)
    {
        if (createCall == null) return;
        if (this.createCall == null)
            this.createCall = createCall;
        else
            this.createCall += createCall;
    }
    public void RemoveCreateCall(UnityAction createCall)
    {
        if (createCall == null) return;
        if (this.createCall != null)
            this.createCall -= createCall;
    }

    public void ClearCreateCall()
    {
        createCall = null;
    }
    public void AddDeathCall(UnityAction deathCall)
    {
        if (deathCall == null) return;
        if (this.deathCall == null)
            this.deathCall = deathCall;
        else
            this.deathCall += deathCall;
    }
    public void RemoveDeathCall(UnityAction deathCall)
    {
        if (deathCall == null) return;
        if (this.deathCall != null)
            this.deathCall -= deathCall;
    }
    public void ClearDeathCall()
    {
        deathCall = null;
    }
}
