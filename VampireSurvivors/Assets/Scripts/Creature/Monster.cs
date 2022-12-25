using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Monster : Creature
{
    [SerializeField] private int currentHP;
    [SerializeField] private int originalHP;

    private void OnEnable()
    {
        if(NetManager.Instance != null)
        {
            if(NetManager.Instance.Server != null)
            {
                StartCoroutine(AICor());
            }
        }
    }

    #region 클라이언트에서 사용
    [Header("클라이언트")]
    [SerializeField] private MonsterAnimator monsterAnimator;
    public void OnDeath()
    {
        if(monsterAnimator != null)
        {
            monsterAnimator.OnDeath();
        }
    }

    public void OnHit()
    {
        if(monsterAnimator != null)
        {
            monsterAnimator.OnHit();
        }
    }

    public void OnCreate()
    {
        if (monsterAnimator != null)
        {
            monsterAnimator.OnCreate();
        }
    }
    #endregion

    #region 서버에서 사용 
    [Header("서버")]
    [SerializeField] private NetMonsterControllerAgent monsterControllerAgent;
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private Rigidbody2D rigid2D;
    [SerializeField] private float mass;
    [SerializeField] private float findCooltime;
    [SerializeField] private Transform target;
    [ReadOnly] public int id;
 
    public int OriginalHP
    {
        get
        {
            if (NetManager.Instance != null)
            {
                if (NetManager.Instance.Server != null)
                {
                    var MC = GameManager.GetMonsterController();
                    if (MC == null) return 0;
                    return MonsterData.GetHP(MC.MonsterLevel);
                }
                if(NetManager.Instance.Client != null)
                {
                    return originalHP;
                }
            }
            return -1;
        }
        set
        {
            if(NetManager.Instance != null && NetManager.Instance.Client != null)
            {
                originalHP = value;
            }
        }
    }
    public int CurrentHP
    {
        get => currentHP;
        set => currentHP = value;
    }

    public MonsterData MonsterData => monsterData;

    private IEnumerator AICor()
    {
        var wait = new WaitForSeconds(findCooltime);
        StartCoroutine(MoveCor());
        while (true)
        {
            yield return wait;

            if (monsterControllerAgent != null)
            {
                if(monsterControllerAgent.GameRuler != null)
                {
                    var playerController = monsterControllerAgent.GameRuler.PlayerControllerAgent;

                    if(playerController != null)
                    {
                        var choice = Random.Range(0, playerController.PlayerCount);

                        target = playerController.GetPlayer(choice).transform;
                    }
                }
            }
        }
    }

    private IEnumerator MoveCor()
    {
        while (true)
        {
            yield return null;

            if(target != null)
            {
                rigid2D.velocity = ((Vector2)(target.position - transform.position)).normalized * MonsterData.MoveSpeed;
            }
            else
            {
                rigid2D.velocity = Vector2.zero;
            }
        }
    }
    #endregion

    #region 폐기

    public void OnHit(int damage, Vector2 force)
    {
    }

    public void Deactive()
    {
    }

    #endregion


}
