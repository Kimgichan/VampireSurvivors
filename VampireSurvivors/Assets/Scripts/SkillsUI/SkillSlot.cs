using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Nodes;

public class SkillSlot : MonoBehaviour
{
    [SerializeField] protected Image icon;
    [SerializeField] protected TextMeshProUGUI levelTxt;

    [SerializeField] protected NSkill skillInfo;
    
    public NSkill SkillInfo
    {
        get
        {
            return skillInfo;
        }
        set
        {
            skillInfo = value;
            if(skillInfo == null)
            {
                if (icon != null) icon.gameObject.SetActive(false);
                if (levelTxt != null) levelTxt.gameObject.SetActive(false);
                return;
            }

            if (icon != null)
            {
                icon.gameObject.SetActive(true);
                icon.sprite = skillInfo.skillData.Icon;
            }
            if (levelTxt != null)
            {
                levelTxt.gameObject.SetActive(true);
                levelTxt.text = $"LV{skillInfo.level}";
            }
        }
    }

    public void SetSkillInfo(SkillData data, int level)
    {
        if (skillInfo == null)
        {
            var skill = new NSkill() { skillData = data, level = level };
            SkillInfo = skill;
        }
        else
        {
            skillInfo.skillData = data;
            skillInfo.level = level;

            if (icon != null)
            {
                icon.gameObject.SetActive(true);
                icon.sprite = skillInfo.skillData.Icon;
            }
            if (levelTxt != null)
            {
                levelTxt.gameObject.SetActive(true);
                levelTxt.text = $"LV{skillInfo.level}";
            }
        }
    }
}
