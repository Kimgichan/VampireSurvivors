using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enums;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Object/WeaponData", order = int.MaxValue)]
public class WeaponData : ScriptableObject
{
    [SerializeField] private string title;
    [SerializeField] private WeaponRating rating;

    [SerializeField] private int requireCoin;

    [SerializeField] private float cooltime;
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;
    [SerializeField] private int maxLevel;
    [SerializeField] private float range;
    [SerializeField] private float searchCooltime;

    [SerializeField] private float decreaseCooltime;
    [SerializeField] private int increaseMinDamage;
    [SerializeField] private int increaseMaxDamage;
    [SerializeField] private int increaseRequireCoin;
    [SerializeField] private float increaseRange;
    [SerializeField] private float decreaseSearchCooltime;


    public virtual string Title => title;

    public WeaponRating Rating => rating;

    public int MaxLevel => maxLevel;

    public virtual string GetContent(int level)
    {
        if(level >= maxLevel)
        {
            return $"「Lv. {level}(MAX)」\r\n「쿨타임」 : {GetCooltime(level)}s\r\n「데미지」 : {GetMinDamage(level)}~{GetMaxDamage(level)}\r\n";
        }
        else
        {
            return $"「Lv. {level}」\r\n「쿨타임」 : {GetCooltime(level)}(=><color=orenge>{GetCooltime(level + 1)}</color>)s\r\n「데미지」 : {GetMinDamage(level)}(=><color=orenge>{GetMinDamage(level+1)}</color>)~{GetMaxDamage(level)}(=><color=orenge>{GetMaxDamage(level+1)}</color>)\r\n";
        }
    }

    public float GetCooltime(int level)
    {
        var _cooltime = cooltime - decreaseCooltime * (float)(level - 1);
        if (cooltime < 0f) return 0f;
        return cooltime;
    }

    public int GetMinDamage(int level)
    {
        var _minDamage = minDamage + increaseMinDamage * (level - 1);
        if (_minDamage < 0) return 0;

        var _maxDamage = GetMaxDamage(level);
        if (_minDamage > _maxDamage) return _maxDamage;

        return _minDamage;
    }

    public int GetMaxDamage(int level)
    {
        var _maxDamage = maxDamage + increaseMaxDamage * (level - 1);
        if (_maxDamage < 0) return 0;
        return _maxDamage;
    }

    public int GetRequireCoin(int level)
    {
        var _requireCoin = requireCoin + increaseRequireCoin * (level - 1);
        if (_requireCoin < 0) return 0;
        return _requireCoin;
    }

    public float GetRange(int level)
    {
        var _range = range + increaseRange * (level - 1);
        if (_range < 0f) return 0f;
        return _range;
    }

    public float GetSearchCooltime(int level)
    {
        var _searchCooltime = searchCooltime - decreaseSearchCooltime * (level - 1);
        if (_searchCooltime < 0f) return 0f;
        return _searchCooltime;
    }
}
