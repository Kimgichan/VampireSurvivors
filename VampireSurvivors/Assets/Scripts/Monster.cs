using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private Rigidbody2D rigid2D;
    [SerializeField] private int currentHP;

    private IEnumerator groggyCor;
    private IEnumerator attackCooltimeCor;

    public MonsterData MonsterData => monsterData;

    public int OriginalHP
    {
        get
        {
            if (GameManager.Instance == null) return 0;
            return MonsterData.GetHP(GameManager.Instance.GetCurrengStageLevel());
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        var pos = transform.position;
        pos.z = pos.y * 0.001f;
        transform.position = pos;

        if (GameManager.Instance == null || GameManager.Instance.gameController == null) return;

        var GC = GameManager.Instance.gameController;

        if (GC.Player == null)
        {
            rigid2D.velocity = Vector2.zero;
            return;
        }


        if (groggyCor == null)
        {
            if (GameManager.Instance.timeScaleController == null) return;
            var TS = GameManager.Instance.timeScaleController;
            var dir = ((Vector2)(GC.Player.transform.position - transform.position)).normalized;
            rigid2D.velocity = dir * TS.gameTimeScale * MonsterData.MoveSpeed;
        }
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
                groggyCor = GroggyCor();
                StartCoroutine(groggyCor);
            }
        }
    }

    private void OnDisable()
    {
        if(attackCooltimeCor != null)
        {
            StopCoroutine(attackCooltimeCor);
            attackCooltimeCor = null;
        }
        if(groggyCor != null)
        {
            StopCoroutine(groggyCor);
            groggyCor = null;
        }
    }

    public void Active()
    {
        gameObject.SetActive(true);
        currentHP = OriginalHP;
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
        if (GameManager.Instance == null) return 0;

        var GM = GameManager.Instance;
        return Random.Range(MonsterData.GetMinDamage(GM.GetCurrengStageLevel()), MonsterData.GetMaxDamage(GM.GetCurrengStageLevel()) + 1);
    }

    public void OnHit(int damage)
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
            TurnOffGroggy();
            groggyCor = GroggyCor();
            StartCoroutine(groggyCor);
        }
        else // »ç¸Á
        {
            if (GameManager.Instance == null || GameManager.Instance.monsterController == null) return;

            var MC = GameManager.Instance.monsterController;
            MC.Push(this);
        }
    }

    private IEnumerator GroggyCor()
    {
        var cooltime = MonsterData.GroggyCooltime;
        rigid2D.mass = 0.5f;
        rigid2D.velocity = Vector2.zero;

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

        groggyCor = null;
    }

    public void TurnOffGroggy()
    {
        if (groggyCor != null)
        {
            StopCoroutine(groggyCor);
        }
        groggyCor = null;
        rigid2D.mass = 0.0001f;
    }
}
