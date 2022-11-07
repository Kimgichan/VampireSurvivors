using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected WeaponData weaponData;
    [SerializeField] protected int level;

    public WeaponData WeaponData => weaponData;
    public int Level
    {
        get
        {
            return level;
        }
        set
        {
            if (value < 0) value = 0;
            else if (value > weaponData.MaxLevel) value = weaponData.MaxLevel;

            level = value;
        }
    }

    public virtual int GetDamage()
    {
        return Random.Range(WeaponData.GetMinDamage(level), WeaponData.GetMaxDamage(level) + 1);
    }
}
