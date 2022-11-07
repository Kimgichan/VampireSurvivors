using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character : Creature
{
    [SerializeField] private CharacterAnimator characterAnimator;
    [SerializeField] private Rigidbody2D rigid2D;
    [SerializeField] private int currentHP;
    [SerializeField] private float moveSpeed;
    
    private IEnumerator activeCor;

    public int OriginalHP
    {
        get
        {
            return 50;
        }
    }

    public int CurrentHP
    {
        set
        {
            var UC = GameManager.GetUIController();
            if (UC != null)
            {
                UC.HP_Percent = value / (float)OriginalHP;
            }

            currentHP = value;
        }
        get => currentHP;
    }

    private Vector2 Velocity
    {
        get
        {
            return rigid2D.velocity;
        }
        set
        {
            if(GameManager.Instance == null || GameManager.Instance.timeScaleController == null)
            {
                rigid2D.velocity = Vector2.zero;
                return;
            }
            rigid2D.velocity = value * GameManager.Instance.timeScaleController.gameTimeScale * moveSpeed;
        }
    }

    private void OnDisable()
    {
        TurnOffActive();
    }

    public void Active()
    {
        gameObject.SetActive(true);
        currentHP = OriginalHP;

        var UC = GameManager.GetUIController();
        if(UC != null)
        {
            UC.SetHP_Percent(currentHP / (float)OriginalHP);
        }


        characterAnimator.ClearCreateCall();
        characterAnimator.AddCreateCall(TurnOnActive);

        characterAnimator.ClearDeathCall();
        characterAnimator.AddDeathCall(Death);

        characterAnimator.OnCreate();
    }

    private void TurnOnActive()
    {
        if (activeCor != null) return;

        rigid2D.simulated = true;
        characterAnimator.OnIdle();
        activeCor = ActiveCor();
        StartCoroutine(activeCor);
    }

    private void TurnOffActive()
    {
        rigid2D.simulated = false;
        if (activeCor != null)
        {
            StopCoroutine(activeCor);
            activeCor = null;
        }
    }

    private void Death()
    {
        var gc = GameManager.GetGameController();
        if(gc != null)
        {
            gc.GameEnd();
        }
    }

    private IEnumerator ActiveCor()
    {
        while (true)
        {
            yield return null;

            var pos = transform.position;
            pos.z = pos.y * 0.001f;
            transform.position = pos;

            var TSC = GameManager.GetTimeScaleController();
            if(TSC != null)
            {
                characterAnimator.AnimSpeed = TSC.gameTimeScale;
            }
        }
    }

    public void Move(Vector2 force)
    {
        if (activeCor == null) return;

        Velocity = force;
    }

    public void OnHit(int damage)
    {
        if (activeCor == null) return;

        var hp = CurrentHP - damage;
        if (hp <= 0)
        {
            CurrentHP = 0;
            characterAnimator.OnDeath();
        }
        else
        {
            CurrentHP = hp;
            characterAnimator.OnHit();

            var UC = GameManager.GetUIController();
            if(UC != null)
            {
                UC.HP_Percent = CurrentHP / (float)OriginalHP;
            }
        }

        var DTC = GameManager.GetDamageTextController();
        if(DTC != null)
        {
            DTC.TurnOnPopup(damage, transform.position, Enums.Creature.Character);
        }
    }
}
