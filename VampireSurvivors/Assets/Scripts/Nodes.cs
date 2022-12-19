using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System;
using UnityEngine;
using NaughtyAttributes;

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
        [ReadOnly] public bool isRun;
        [ReadOnly] [SerializeField] private string stage;
        [ReadOnly] [SerializeField] private List<string> players;
        [ReadOnly] [SerializeField] private List<float> percents;


        public string Stage => stage;
        public int PlayerCount
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
        public int RoomSize
        {
            get
            {
                return players.Count;
            }
        }

        public Room(int capacity, string stage)
        {
            this.stage = stage;
            players = new List<string>(capacity);
            percents = new List<float>(capacity);
            for(int i = 0; i<capacity; i++)
            {
                players.Add("");
                percents.Add(0f); 
            }

            isRun = false;
        }

        public NetEnums.RoomEnterResult AddPlayer(string player)
        {
            if (PlayerCount == RoomSize)
                return NetEnums.RoomEnterResult.Full;
            else
            {
                if (players.Contains(player))
                {
                    return NetEnums.RoomEnterResult.Multiple;
                }
                else
                {
                    for(int i = 0, icount = RoomSize; i<icount; i++)
                    {
                        if(players[i] == "")
                        {
                            players[i] = player;
                            break;
                        }
                    }
                    return NetEnums.RoomEnterResult.Success;
                }
            }
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

        public string GetPlayer(int indx)
        {
            if (players == null) return "";
            return players[indx];
        }
        public float GetPercent(int indx)
        {
            return percents[indx];
        }
        public void SetPercent(int indx, float val)
        {
            percents[indx] = val;
        }

        public NetNodes.Server.EnterRoom GetEnterRoom()
        {
            var _return = new NetNodes.Server.EnterRoom();
            _return.players = new List<string>(players);
            return _return;
        }

        public NetNodes.Server.Ready GetReady()
        {
            var _return = new NetNodes.Server.Ready();
            _return.stage = stage;
            _return.players = new List<string>(players);
            _return.percents = new List<float>(percents);
            return _return;
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
            public int size;
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

            public int RoomSize
            {
                get
                {
                    return players.Count;
                }
            }
            public int PlayerCount
            {
                get
                {
                    int result = RoomSize;
                    for(int i = 0, icount = RoomSize; i<icount; i++)
                    {
                        if(players[i] == "")
                        {
                            result -= 1;
                        }
                    }

                    return result;
                }
            }
        }
        // CancelRoom은 파라미터 없음
        //여기까지
        [Serializable]
        public struct Ready
        {
            public string stage;
            public List<string> players;
            public List<float> percents;
        }

        [Serializable]
        public struct GameTick
        {
            public MonstersInfo monstersInfo;
        }
        [Serializable]
        public struct MonstersInfo
        {
            public int[] ID_List;
            public Vector2[] posList;
            public int[] currentHPList;

            public static MonstersInfo zero => new MonstersInfo()
            {
                ID_List = new int[0],
                posList = new Vector2[0],
                currentHPList = new int[0]
            };
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