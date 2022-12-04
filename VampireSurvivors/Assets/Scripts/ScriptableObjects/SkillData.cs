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
            "ON_����_1",
            () =>
            {

            }
        },

        {
            "ON_����_1",
            () =>
            {

            }
        },

        {
            "ON_�̼�_1",
            () =>
            {

            }
        },

        {
            "ON_����_1",
            () =>
            {

            }
        },

        {
            "ON_�Ŀ�_1",
            () =>
            {

            }
        },
        #endregion
#region OFF
        {
            "OFF_����_1",
            () =>
            {

            }
        },

        {
            "OFF_����_1",
            () =>
            {

            }
        },

        {
            "OFF_�̼�_1",
            () =>
            {

            }
        },

        {
            "OFF_����_1",
            () =>
            {

            }
        },

        {
            "OFF_�Ŀ�_1",
            () =>
            {

            }
        },
#endregion
    };

    private static Dictionary<string, UnityAction<int>> completeAgentTable = new Dictionary<string, UnityAction<int>>()
    {
        {
            "��������",
            (amount) =>
            {
                if(GameManager.Instance != null && GameManager.Instance.Inventory != null)
                {
                    GameManager.Instance.Inventory.Gold += amount;
                }
            }
        },
        {
            "���",
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
            "����",
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
            "����_1",
            ""
        },

        {
            "����_1",
            ""
        },

        {
            "�̼�_1",
            ""
        },

        {
            "����_1",
            ""
        },

        {
            "�Ŀ�_1",
            ""
        },
    };

    private static Dictionary<string, Func<int, string>> contentCompleteTable = new Dictionary<string, Func<int, string>>()
    {
        {
            "��������",
            (amount) => $"<color=yellow>{amount}</color> Gold ȹ��"
        },
        {
            "���",
            (amount) => $"<color=green>{amount}</color> EXP ȹ��"
        },
        {
            "����",
            (amount) => $"<color=red>{amount}</color> HP ȸ��"
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

