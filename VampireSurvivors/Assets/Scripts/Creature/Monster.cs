using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Monster : Creature
{
    [ReadOnly] public int id;
    [SerializeField] private MonsterAnimator monsterAnimator;
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private Rigidbody2D rigid2D;
    [SerializeField] private int currentHP;
    [SerializeField] private float mass;

    private IEnumerator groggyCor;
    private IEnumerator attackCooltimeCor;
    private IEnumerator activeCor;

    public MonsterData MonsterData => monsterData;

    public int OriginalHP
    {
        get
        {
            var MC = GameManager.GetMonsterController();
            if (MC == null) return 0;
            return MonsterData.GetHP(MC.MonsterLevel);
        }
    }
    public int CurrentHP
    {
        get => currentHP;
        set => currentHP = value;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!gameObject.activeSelf) return;
        if (attackCooltimeCor != null)
        {
            return;
        }

        if (attackCooltimeCor == null)
        {
            var character = collision.transform.GetComponent<Character>();
            if (character != null)
            {
                character.OnHit(GetDamage());

                attackCooltimeCor = AttackCooltimeCor();
                StartCoroutine(attackCooltimeCor);

                TurnOffGroggy();
                groggyCor = GroggyCor(Vector2.zero);
                StartCoroutine(groggyCor);
            }
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

        int level = 1;
        var MC = GameManager.GetMonsterController();
        if (MC != null) level = MC.MonsterLevel;

        if(equipSlotBag != null)
        {
            for(int i = 0, icount = equipSlotBag.SlotCount; i<icount; i++)
            {
                equipSlotBag.SetWeaponLevel(i, level);
            }
        }

        monsterAnimator.ClearCreateCall();
        monsterAnimator.AddCreateCall(TurnOnActive);

        monsterAnimator.ClearDeathCall();
        monsterAnimator.AddDeathCall(Death);

        monsterAnimator.OnCreate();
    }

    private IEnumerator AttackCooltimeCor()
    {
        var cooltime = MonsterData.AttackCooltime;

        while(cooltime > 0f)
        {
            yield return null;

            if(GameManager.Instance == null || GameManager.Instance.timeScaleController == null)
            {
                attackCooltimeCor = null;
                yield break;
            }

            cooltime -= GameManager.Instance.timeScaleController.GameTimeScaleUpdate;
        }

        attackCooltimeCor = null;
    }

    public int GetDamage()
    {
        var MC = GameManager.GetMonsterController();
        if (MC == null) return 0;

        return Random.Range(MonsterData.GetMinDamage(MC.MonsterLevel), MonsterData.GetMaxDamage(MC.MonsterLevel) + 1);
    }

    public void OnHit(int damage, Vector2 force)
    {
        var DTC = GameManager.GetDamageTextController();
        if(DTC != null)
        {
            DTC.TurnOnPopup(damage, transform.position);
        }

        currentHP -= damage;
        if(currentHP < 0)
        {
            currentHP = 0;
        }

        if (currentHP > 0)
        {
            monsterAnimator.OnHit();
            TurnOffGroggy();
            groggyCor = GroggyCor(force);
            StartCoroutine(groggyCor);
        }
        else // »ç¸Á
        {
            monsterAnimator.OnDeath();
            TurnOffActive();
        }
    }

    public void Deactive()
    {
        //monsterAnimator.OnDeath();
        TurnOffActive();
        monsterAnimator.ClearCreateCall();
        monsterAnimator.ClearDeathCall();
    }

    private void Death()
    {
        if (GameManager.Instance == null || GameManager.Instance.monsterController == null) return;

        monsterAnimator.ClearCreateCall();
        monsterAnimator.ClearDeathCall();

        var MC = GameManager.Instance.monsterController;
        MC.Push(this);

        var GC = GameManager.GetGameController();
        if(GC != null)
        {
            GC.CurrentEXP += 1;
        }
    }

    private IEnumerator GroggyCor(Vector2 force)
    {
        var cooltime = MonsterData.GroggyCooltime;
        rigid2D.mass = mass;
        rigid2D.drag = 10f;
        rigid2D.velocity = Vector2.zero;

        rigid2D.AddForce(force*10f);

        while (cooltime > 0f)
        {
            yield return null;

            if(GameManager.Instance == null ||
                GameManager.Instance.timeScaleController == null)
            {
                groggyCor = null;
                yield break;
            }
            cooltime -= GameManager.Instance.timeScaleController.GameTimeScaleUpdate;
        }

        monsterAnimator.OnIdle();
        rigid2D.mass = 0.0001f;
        rigid2D.drag = 0f;
        rigid2D.velocity = Vector2.zero;
        groggyCor = null;
    }

    public void TurnOffGroggy()
    {
        if (groggyCor != null)
        {
            StopCoroutine(groggyCor);
        }
        groggyCor = null;
    }

    private void TurnOnActive()
    {
        if (activeCor != null) return;

        rigid2D.simulated = true;
        monsterAnimator.OnIdle();
        activeCor = ActiveCor();
        StartCoroutine(activeCor);
    }
    private void TurnOffActive()
    {
        rigid2D.simulated = false;
        if (attackCooltimeCor != null)
        {
            StopCoroutine(attackCooltimeCor);
            attackCooltimeCor = null;
        }
        if (groggyCor != null)
        {
            StopCoroutine(groggyCor);
            groggyCor = null;
        }

        if (activeCor != null)
        {
            StopCoroutine(activeCor);
            activeCor = null;
        }
    }

    private IEnumerator ActiveCor()
    {
        while (true)
        {
            yield return null;
            var TSC = GameManager.GetTimeScaleController();
            if(TSC != null)
            {
                monsterAnimator.AnimSpeed = TSC.gameTimeScale;
            }

            var pos = transform.position;
            pos.z = pos.y * 0.001f;
            transform.position = pos;

            if (GameManager.Instance == null || GameManager.Instance.gameController == null) continue;

            var GC = GameManager.Instance.gameController;

            if (GC.Player == null)
            {
                rigid2D.velocity = Vector2.zero;
                continue;
            }


            if (groggyCor == null)
            {
                if (GameManager.Instance.timeScaleController == null) continue;
                var TS = GameManager.Instance.timeScaleController;
                var dir = ((Vector2)(GC.Player.transform.position - transform.position)).normalized;
                rigid2D.velocity = dir * TS.gameTimeScale * MonsterData.MoveSpeed;
            }
        }
    }
}
