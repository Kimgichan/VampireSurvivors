using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;

public class EquipSlotBag : MonoBehaviour
{
    [SerializeField] private List<Transform> weaponSlots;

    public int WeaponCount
    {
        get
        {
            int count = 0;
            for(int i = 0, icount = weaponSlots.Count; i<icount; i++)
            {
                if (weaponSlots[i].childCount > 0)
                    count += 1;
            }

            return count;
        }
    }

    public WeaponSlotState AddWeapon(WeaponData weaponData)
    {
        for(int i = 0, icount = weaponSlots.Count; i<icount; i++)
        {
            var slot = weaponSlots[i];
            if(slot.childCount > 0)
            {
                var slotWeapon = slot.GetChild(0).GetComponent<Weapon>();
                if(slotWeapon.WeaponData == weaponData)
                {
                    if (slotWeapon.Level < slotWeapon.WeaponData.MaxLevel)
                    {
                        slotWeapon.Level += 1;
                        return WeaponSlotState.None;
                    }
                    else return WeaponSlotState.MaxLevel;
                }
            }
            else
            {
                //Instantiate(weapon, slot);

                if (i < icount / 2)
                {
                    return WeaponSlotState.Left;
                }
                else
                {
                    return WeaponSlotState.Right;
                }
            }
        }

        return WeaponSlotState.Full;
    }

    public WeaponSlotState AddWeapon(int weaponIndx)
    {
        return WeaponSlotState.None;
    }

    public int GetFindIndex(WeaponData weapon)
    {
        return 0;
    }

    public Weapon GetWeapon(int indx)
    {
        var slot = weaponSlots[indx];
        if(slot.childCount > 0)
        {
            return slot.GetChild(0).GetComponent<Weapon>();
        }
        return null;
    }

    public void RemoveWeapon(int weaponIndx)
    {
        var tr = weaponSlots[weaponIndx];
        if(tr.childCount > 0)
        {
            Destroy(tr.GetChild(0).gameObject);
        }
    }

    public void RemoveWeapon(WeaponData weaponData)
    {

    }
}
