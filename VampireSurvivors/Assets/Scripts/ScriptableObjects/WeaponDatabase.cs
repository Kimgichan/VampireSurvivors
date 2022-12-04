using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Nodes;

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Scriptable Object/WeaponDatabase", order = int.MaxValue)]
public class WeaponDatabase : ScriptableObject
{
    [SerializeField] private List<Weapon> weapons;
    [SerializeField] private List<string> keys;
    private Dictionary<string, Weapon> table;

    private void OnEnable()
    {
        if(weapons != null)
        {
            keys = new List<string>(weapons.Count);
            table = new Dictionary<string, Weapon>(weapons.Count);
            for(int i = 0, icount = weapons.Count; i<icount; i++)
            {
                keys.Add(weapons[i].WeaponData.name);
                table.Add(keys[i], weapons[i]);
            }
        }
    }

    public Weapon CreateWeapon(NWeapon weapon)
    {
        if(table.TryGetValue(name, out Weapon val))
        {
            var _weapon = Instantiate(val);
            _weapon.Level = weapon.level;
            return _weapon;
        }
        return null;
    }

    public WeaponData GetWeaponData(string name)
    {
        if (table.TryGetValue(name, out Weapon val))
        {
            return val.WeaponData;
        }
        return null;
    }
}
