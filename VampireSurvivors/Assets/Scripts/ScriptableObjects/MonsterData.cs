using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Object/MonsterData", order = int.MaxValue)]
public class MonsterData : ScriptableObject
{
    [SerializeField] private int hp;
    [SerializeField] private int minRewardCoin;
    [SerializeField] private int maxRewardCoin;
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;
    [SerializeField] private float attackCooltime;
    [SerializeField] private float groggyCooltime;
    [SerializeField] private float moveSpeed;

    [SerializeField] private int increaseHP;
    [SerializeField] private int increaseMinRewardCoin;
    [SerializeField] private int increaseMaxRewardCoin;
    [SerializeField] private int increaseMinDamage;
    [SerializeField] private int increaseMaxDamage;

    public float AttackCooltime => attackCooltime;
    public float GroggyCooltime => groggyCooltime;
    public float MoveSpeed => moveSpeed;

    public int GetHP(int level)
    {
        var _hp = hp + increaseHP * (level - 1);
        if (_hp < 0) return 0;
        return _hp;
    }
    public int GetMinRewardCoin(int level)
    {
        var _minRewardCoin = minRewardCoin + increaseMinRewardCoin * (level - 1);
        var maxRewardCoin = GetMaxRewardCoin(level);

        if (_minRewardCoin > maxRewardCoin) return maxRewardCoin;
        if (_minRewardCoin < 0) return 0;
        return _minRewardCoin;
    }

    public int GetMaxRewardCoin(int level)
    {
        var _maxRewardCoin = maxRewardCoin + increaseMaxRewardCoin * (level - 1);
        if (_maxRewardCoin < 0) return 0;
        return _maxRewardCoin;
    }

    public int GetMinDamage(int level)
    {
        var _minDamage = minDamage + increaseMinDamage * (level - 1);
        var maxDamage = GetMaxDamage(level);

        if (_minDamage > maxDamage) return maxDamage;
        if (_minDamage < 0) return 0;
        return _minDamage;
    }
    public int GetMaxDamage(int level)
    {
        var _maxDamage = maxDamage + increaseMaxDamage * (level - 1);
        if (_maxDamage < 0) return 0;
        return _maxDamage;
    }
}
