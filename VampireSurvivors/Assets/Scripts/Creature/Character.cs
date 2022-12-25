using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


public class Character : Creature
{
    #region 공통
    [SerializeField] private int currentHP;
    [SerializeField] private int originalHP;
    public int OriginalHP
    {
        get
        {
            if(NetManager.Instance != null)
            {
                if(NetManager.Instance.Client != null)
                {
                    return originalHP;
                }
                if(NetManager.Instance.Server != null)
                {
                    return 50;
                }
            }
            return 0;
        }
        set
        {
            if(NetManager.Instance != null)
            {
                if(NetManager.Instance.Client != null)
                {
                    originalHP = value;
                }
            }
        }
    }
    public int CurrentHP
    {
        get => currentHP;
        set => currentHP = value;
    }

    private void OnEnable()
    {
        if(NetManager.Instance != null)
        {
            if(NetManager.Instance.Client != null)
            {
                StartCoroutine(ZOrderCor());
            }
        }
    }

    public void MoveInput(Vector2 input)
    {
        if(NetManager.Instance != null)
        {
            if(NetManager.Instance.Client != null)
            {
                NetManager.Instance.Client.SendData_PlayerMoveInput(new NetNodes.Client.PlayerMoveInput() { id = id, force = input });
            }
            else if(NetManager.Instance.Server != null)
            {
                rigid2D.velocity = input * moveSpeed;
            }
        }
    }
    #endregion

    #region 클라이언트
    [Header("클라이언트")]
    [SerializeField] private CharacterAnimator characterAnimator;
    
    private IEnumerator ZOrderCor()
    {
        while (true)
        {
            yield return null;

            var pos = transform.localPosition;
            pos.z = pos.y * 0.05f;
            transform.localPosition = pos;
        }
    }
    #endregion

    #region 서버
    [Header("서버")]
    [SerializeField] private NetPlayerControllerAgent playerControllerAgent;
    [SerializeField] private Rigidbody2D rigid2D;
    [SerializeField] private float moveSpeed;
    [ReadOnly] public int id;

    public void OnHit(int damage)
    {
        if (currentHP > 0)
        {
            currentHP -= damage;

            if (currentHP < 0)
            {
                currentHP = 0;
                OnDeath();
            }
        }
    }

    private void OnDeath()
    {

    }
    #endregion

}
