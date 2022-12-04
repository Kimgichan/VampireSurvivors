using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nodes;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected NWeapon weaponInfo;

    public WeaponData WeaponData => weaponInfo.weaponData;
    public int Level
    {
        get
        {
            return weaponInfo.level;
        }
        set
        {
            if (value < 0) value = 0;
            else if (value > WeaponData.MaxLevel) value = WeaponData.MaxLevel;

            weaponInfo.level = value;
        }
    }

    public virtual int GetDamage()
    {
        return Random.Range(WeaponData.GetMinDamage(Level), WeaponData.GetMaxDamage(Level) + 1);
    }
}
