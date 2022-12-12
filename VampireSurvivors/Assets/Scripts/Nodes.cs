using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System;
using UnityEngine;

using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

namespace Nodes
{
    [System.Serializable]
    public class NAudioClip
    {
        public float scale;
        public AudioClip audioClip;
    }

    [System.Serializable]
    public class NAudioSource
    {
        public NAudioClip soundClip;
        public AudioSource audioSource;
    }

    [System.Serializable]
    public class TrList
    {
        public List<Transform> trs;
    }

    [System.Serializable]
    public struct NWeaponJson
    {
        public string weaponData;
        public int level;

        public static NWeapon ToNWeapon(NWeaponJson weaponJson)
        {
            var result = new NWeapon();

            result.weaponData = GameManager.GetWeaponData(weaponJson.weaponData);
            result.level = weaponJson.level;

            return result;
        }
    }

    [System.Serializable]
    public struct NWeapon
    {
        public WeaponData weaponData;
        public int level;

        public void InfoRemove()
        {
            weaponData = null;
            level = -1;
        }

        public static NWeaponJson ToNWeaponJson(NWeapon weapon)
        {
            var result = new NWeaponJson();

            result.weaponData = weapon.weaponData.name;
            result.level = weapon.level;

            return result;
        }
    }

    [System.Serializable]
    public class NInventory
    {
#if UNITY_EDITOR
        [SerializeField] private string goldView;
        [SerializeField] private string crystalView;
        [SerializeField] private string dustView;
#endif

        private BigInteger gold;
        private BigInteger crystal;
        private BigInteger dust;

        [SerializeField] private List<NWeapon> weapons;
        [SerializeField] private int weaponSlotCapacity;

        public BigInteger Gold
        {
            get => gold;
            set
            {
                if (value < 0)
                {
                    gold = 0;
                }
                else
                {
                    gold = value;
                }

                var UC = GameManager.GetUIController();
                if(UC != null)
                {
                    UC.Gold = gold;
                }

#if UNITY_EDITOR
                goldView = gold.ToString();
#endif
            }
        }
        public BigInteger Crystal
        {
            get => crystal;
            set
            {
                if (value < 0)
                {
                    crystal = 0;
                }
                else
                    crystal = value;

#if UNITY_EDITOR
                crystalView = crystal.ToString();
#endif
            }
        }
        public BigInteger Dust
        {
            get => dust;
            set
            {
                if (value < 0)
                {
                    dust = 0;

                }
                else
                    dust = value;

#if UNITY_EDITOR
                dustView = dust.ToString();
#endif
            }
        }


        public void Init()
        {
            weapons = new List<NWeapon>(weaponSlotCapacity);
            for(int i = 0; i<weaponSlotCapacity; i++)
            {
                weapons.Add(new NWeapon() { weaponData = null, level = -1 });
            }
        }
    }


    [System.Serializable]
    public class NSkillTree
    {
        [SerializeField] private List<NSkill> skills;

        public int Count => skills.Count;

        public bool AddSkill(SkillData skillData)
        {
            if (SkillAgent.ContainsCompleteAction(skillData.name))
            {
                return false;
            }

            for(int i = 0, icount = Count; i<icount; i++)
            {
                if(skills[i].skillData == skillData)
                {
                    skills[i].level += 1;
                    if (skills[i].Active())
                    {
                        SkillAgent.Action(skills[i].skillData.name, skills[i].level - 1, true);
                        return true;
                    }
                    else
                    {
                        skills[i].level -= 1;
                        return false;
                    }
                }
            }

            var newSkill = new NSkill() { skillData = skillData, level = 1 };
            newSkill.Active();
            skills.Add(newSkill);

            return true;
        }
        public int GetSkillLevel(int indx)
        {
            return skills[indx].level;
        }
        public int GetSkillLevel(SkillData skillData)
        {
            var skill = skills.Find(f => f.skillData == skillData);
            if (skill != null)
            {
                return skill.level;
            }
            else return 0;
        }
        public SkillData GetSkillData(int indx)
        {
            return skills[indx].skillData;
        }
    }

    [System.Serializable]
    public class NSkill
    {
        public SkillData skillData;
        public int level;

        public bool Active()
        {
            return SkillAgent.Action(skillData.name, level);
        }

        public bool Deactive()
        {
            return SkillAgent.Action(skillData.name, level, true);
        }

        public bool CompleteActive()
        {
            return SkillAgent.CompleteAction(skillData.name, level);
        }

        public bool ContainsCompleteSkill()
        {
            return SkillAgent.ContainsCompleteAction(skillData.name);
        }

        public string GetContent()
        {
            return SkillAgent.GetContent(skillData.name, level);
        }

        public string GetCompleteContent()
        {
            return SkillAgent.GetContent_Complete(skillData.name, level);
        }
    }

    [System.Serializable]
    public class NSkillBookInfo
    {
        [SerializeField] private int minResetPrice;
        [SerializeField] private int maxResetPrice;

        [SerializeField] private int minPrice;
        [SerializeField] private int maxPrice;

        [SerializeField] private int minRewardGold;
        [SerializeField] private int maxRewardGold;

        [SerializeField] private int minPortion;
        [SerializeField] private int maxPortion;

        [SerializeField] private int minEXP;
        [SerializeField] private int maxEXP;

        public int GetRandomResetPrice()
        {
            return Random.Range(minResetPrice, maxResetPrice + 1);
        }

        public int GetRandomPrice()
        {
            return Random.Range(minPrice, maxPrice + 1);
        }

        public int GetRandomRewardGold()
        {
            return Random.Range(minRewardGold, maxRewardGold + 1);
        }
        public int GetRandomPortionAmount()
        {
            return Random.Range(minPortion, maxPortion + 1);
        }
        public int GetRandomEXP()
        {
            return Random.Range(minEXP, maxEXP + 1);
        }

        public int GetCompleteLevel(SkillData skillData)
        {
            switch (skillData.name)
            {
                case "보물상자":
                    return GetRandomRewardGold();
                case "편법":
                    return GetRandomEXP();
                case "포션":
                    return GetRandomPortionAmount();
            }
            return 0;
        }
    }

    [Serializable]
    public class VampireSurvivorsClientInfo
    {
        public ServerClient_VampireSurvivors client;
        public Room room;
    }

    [Serializable]
    public class Room
    {
        public string stage;
        private List<string> players;

        public int Count
        {
            get
            {
                int result = players.Count;
                for(int i = 0, icount = players.Count; i<icount; i++)
                {
                    if(players[i] == "")
                    {
                        result -= 1;
                    }
                }

                return result;
            }
        }

        public Room(int capacity)
        {
            players = new List<string>(capacity);
            for(int i = 0; i<capacity; i++)
            {
                players[i] = "";
            }
        }

        public NetEnums.RoomEnterResult AddPlayer(string player)
        {
            return NetEnums.RoomEnterResult.Full;
        }
        public void RemovePlayer(string player)
        {
            for (int i = 0, icount = players.Count; i < icount; i++)
            {
                if (players[i] == player)
                {
                    players[i] = "";
                    break;
                }
            }
        }
    }
}

namespace NetNodes
{
    namespace Client
    {
        [Serializable]
        public struct Login
        {
            public string player;
        }
        // Logout은 파라미터 없음
        [Serializable]
        public struct EnterRoom
        {
            public string player;
            public string stage;
        }

        [Serializable]
        public struct CancelRoom
        {
            public string player;
        }
        //여기까지
        [Serializable]
        public struct Ready
        {
            public string player;
            public float percent;
        }
        [Serializable]
        public struct Chat
        {
            [SerializeField] private string player;
            [SerializeField] private string msg;

            public string Player => player;
            public string MSG => msg;

            public Chat(string player, string msg)
            {
                this.player = string.Format(@"{0}", player);
                this.msg = string.Format(@"{0}", msg);
            }
        }
    }

    namespace Server
    {
        [Serializable]
        public struct Login
        {
            public bool ok;
            public string msg;
        }
        // Logout은 파라미터 없음
        [Serializable]
        public struct EnterRoom
        {
            public List<string> players;
        }
        [Serializable]
        public struct CancelRoom
        {
            public bool ok;
        }
        //여기까지
        [Serializable]
        public struct Ready
        {
            public string starge;
            public List<string> players;
            public List<float> percents;
        }

        [Serializable]
        public struct PlayUpdate
        {
            public int level;
            public float exp;

            public List<int> monsterKeys;
            public List<Vector2> monsterPosList;
            public List<int> monsterHPList;
            public List<Vector2> playerPosList;
            public List<int> playerHPList;
        }

        [Serializable]
        public struct WeaponAction
        {
            public int team;
            public int creaturekey;
            public int slotIndx;
            public Vector2 targetPos;
            public bool isAttack;
        }

        [Serializable]
        public struct CreatureHitAction
        {
            public int team;
            public int creatureKey;
        }

        [Serializable]
        public struct MonsterCreateAction 
        {
            public int monsterKey;
        }
        [Serializable]
        public struct MonsterDeathAction
        {
            public int monsterKey;
        }
        [Serializable]
        public struct Chat 
        {
            [SerializeField] private string player;
            [SerializeField] private string msg;

            public string Player => player;
            public string MSG => msg;

            public Chat(string player, string msg)
            {
                this.player = string.Format(@"{0}", player);
                this.msg = string.Format(@"{0}", msg);
            }
        }
    }
}