using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NaughtyAttributes;

using Nodes;


[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Object/SkillData", order = int.MaxValue)]
public class SkillData : ScriptableObject
{
    [ShowAssetPreview][SerializeField] private Sprite icon;
    public Sprite Icon => icon;
}

public class SkillAgent 
{
    private static Dictionary<string, UnityAction> agentTable = new Dictionary<string, UnityAction>()
    {
#region ON
        {
            "ON_공속_1",
            () =>
            {

            }
        },

        {
            "ON_빙결_1",
            () =>
            {

            }
        },

        {
            "ON_이속_1",
            () =>
            {

            }
        },

        {
            "ON_출혈_1",
            () =>
            {

            }
        },

        {
            "ON_파워_1",
            () =>
            {

            }
        },
        #endregion
#region OFF
        {
            "OFF_공속_1",
            () =>
            {

            }
        },

        {
            "OFF_빙결_1",
            () =>
            {

            }
        },

        {
            "OFF_이속_1",
            () =>
            {

            }
        },

        {
            "OFF_출혈_1",
            () =>
            {

            }
        },

        {
            "OFF_파워_1",
            () =>
            {

            }
        },
#endregion
    };

    private static Dictionary<string, UnityAction<int>> completeAgentTable = new Dictionary<string, UnityAction<int>>()
    {
        {
            "보물상자",
            (amount) =>
            {
                if(GameManager.Instance != null && GameManager.Instance.Inventory != null)
                {
                    GameManager.Instance.Inventory.Gold += amount;
                }
            }
        },
        {
            "편법",
            (amount) =>
            {
                var GC = GameManager.GetGameController();
                if(GC != null)
                {
                    GC.CurrentEXP += amount;
                }
            }
        },
        {
            "포션",
            (amount) =>
            {
                var GC = GameManager.GetGameController();
                if(GC != null && GC.Player != null)
                {
                    GC.Player.CurrentHP += amount;
                }
            }
        },
    };

    private static Dictionary<string, string> contentTable = new Dictionary<string, string>()
    {
        {
            "공속_1",
            ""
        },

        {
            "빙결_1",
            ""
        },

        {
            "이속_1",
            ""
        },

        {
            "출혈_1",
            ""
        },

        {
            "파워_1",
            ""
        },
    };

    private static Dictionary<string, Func<int, string>> contentCompleteTable = new Dictionary<string, Func<int, string>>()
    {
        {
            "보물상자",
            (amount) => $"<color=yellow>{amount}</color> Gold 획득"
        },
        {
            "편법",
            (amount) => $"<color=green>{amount}</color> EXP 획득"
        },
        {
            "포션",
            (amount) => $"<color=red>{amount}</color> HP 회복"
        },
    };


    public static bool ContainsAction(string skill, int level, bool off = false)
    {
        if (agentTable != null)
        {
            string key;
            if (off)
            {
                key = $"OFF_{skill}_{level}";
            }
            else
            {
                key = $"ON_{skill}_{level}";
            }

            if (agentTable.ContainsKey(key))
            {
                return true;
            }
        }
        return false;
    }

    public static bool Action(string skill, int level, bool off = false)
    {
        if(agentTable != null)
        {
            string key;
            if (off)
            {
                key = $"OFF_{skill}_{level}";
            }
            else
            {
                key = $"ON_{skill}_{level}";
            }

            if(agentTable.TryGetValue(key, out UnityAction action))
            {
                action();
                return true;
            }
        }
        return false;
    }

    public static bool ContainsCompleteAction(string skill)
    {
        if(completeAgentTable != null)
        {
            return completeAgentTable.ContainsKey(skill);
        }
        return false;
    }

    public static bool CompleteAction(string skill, int amount)
    {
        if(completeAgentTable != null)
        {
            if(completeAgentTable.TryGetValue(skill, out UnityAction<int> action))
            {
                action(amount);
                return true;
            }
        }
        return false;
    }

    public static string GetContent(string skill, int level)
    {
        if(contentTable != null)
        {
            if(contentTable.TryGetValue($"{skill}_{level}", out string content))
            {
                return content;
            }
        }
        return "";
    }

    public static string GetContent_Complete(string skill, int amount)
    {
        if(contentCompleteTable != null)
        {
            if(contentCompleteTable.TryGetValue(skill, out Func<int, string> content))
            {
                return content(amount);
            }
        }
        return "";
    }
}

