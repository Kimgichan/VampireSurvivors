using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;
using Nodes;

public class EquipSlotBag : MonoBehaviour
{
    [SerializeField] private List<Transform> weaponSlots;
    [SerializeField] private List<Weapon> weapons;

    public int WeaponCount
    {
        get
        {
            int count = 0;
            for(int i = 0, icount = weapons.Count; i<icount; i++)
            {
                if(weapons[i] != null)
                {
                    count += 1;
                }
            }

            return count;
        }
    }

    public int SlotCount => weaponSlots.Count;

    private void Awake()
    {
        weapons = new List<Weapon>(weaponSlots.Count);
        for(int i = 0, icount = weaponSlots.Count; i<icount; i++)
        {
            if(weaponSlots[i].childCount > 0)
            {
                weapons.Add(weaponSlots[i].GetChild(0).GetComponent<Weapon>());
            }
            else weapons.Add(null);
        }
    }

    public int GetWeaponLevel(int indx) 
    {
        if(weapons[indx] != null)
        {
            return weapons[indx].Level;
        }
        return -1;
    }
    public bool SetWeaponLevel(int indx, int level)
    {
        if (weapons[indx] != null)
        {
            weapons[indx].Level = level;
            return true;
        }
        return false;
    }

    public void EquipWeapon(int indx, in NWeapon weapon)
    {
        var _weapon = weapons[indx];
        if(_weapon != null)
        {
            RemoveWeapon(indx);
        }

        EquipWeaponProcess(indx, weapon);
    }
    public void RemoveWeapon(int indx)
    {
        if(weapons[indx] != null)
        {
            Destroy(weapons[indx].gameObject);
            weapons[indx] = null;
        }
    }

    private void EquipWeaponProcess(int indx, in NWeapon weapon)
    {
        if (GameManager.Instance == null || GameManager.Instance.WeaponDatabase == null) return;

        var _weapon = GameManager.Instance.WeaponDatabase.CreateWeapon(weapon);
        _weapon.transform.parent = weaponSlots[indx];
        _weapon.transform.localPosition = Vector3.zero;

        _weapon.gameObject.SetActive(true);

        weapons[indx] = _weapon;
    }

    public NWeapon GetWeaponInfo(int indx)
    {
        NWeapon weapon = new NWeapon();
        weapon.InfoRemove();

        if(weapons[indx] != null)
        {
            weapon.weaponData = weapons[indx].WeaponData;
            weapon.level = weapons[indx].Level;
        }

        return weapon;
    }
}
