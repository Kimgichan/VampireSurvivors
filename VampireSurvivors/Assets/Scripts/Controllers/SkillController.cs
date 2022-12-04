using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    [SerializeField] private List<SkillData> skills;

    /// <summary>
    /// 플레이어에게 물먹일 경우가 없는 스킬(보상) 목록
    /// </summary>
    [SerializeField] private List<SkillData> completeSkills;

    public int SkillCount => skills.Count;
    public int CompleteSkillCount => completeSkills.Count;


    private IEnumerator Start()
    {
        while (GameManager.Instance == null)
            yield return null;

        if(GameManager.Instance.skillController != null)
        {
            Destroy(gameObject);
            yield break;
        }

        GameManager.Instance.skillController = this;

        ShuffleSkills(skills.Count, 0, skills.Count);
        ShuffleCompleteSkills(completeSkills.Count, 0, completeSkills.Count);
    } 

    public SkillData[] GetRandomRewardSkills(int count)
    {
        if (count > skills.Count) return null;

        ShuffleSkills(count, count, skills.Count);

        var _skills = new SkillData[count];
        for(int i = 0; i<count; i++)
        {
            _skills[i] = skills[i];
        }
        return _skills;
    }

    /// <summary>
    /// 스킬의 레벨 등등의 제한(MAX) 요소가 없는 반드시 보상을(=플레이어를 물먹일 경우가 없는) 제공하는 스킬
    /// </summary>
    /// <returns></returns>
    public SkillData[] GetCompleteRewardSkills(int count)
    {
        if (count > completeSkills.Count) return null;

        ShuffleCompleteSkills(count, count, completeSkills.Count);

        var _skills = new SkillData[count];
        for(int i = 0; i<count; i++)
        {
            _skills[i] = completeSkills[i];
        }
        return _skills;
    }

    public void ShuffleSkills(int shuffleCount, int rangeMin, int rangeMax)
    {
        if (skills.Count < shuffleCount)
        {
            return;
        }
        if(rangeMin < 0 || rangeMax < 0 || rangeMin > skills.Count || rangeMax > skills.Count || rangeMin > rangeMax)
        {
            return;
        }


        for(int i = 0; i<shuffleCount; i++)
        {
            var swap = skills[i];
            var pivot = Random.Range(rangeMin, rangeMax);
            skills[i] = skills[pivot];
            skills[pivot] = swap;
        }
    }

    public void ShuffleCompleteSkills(int shuffleCount, int rangeMin, int rangeMax)
    {
        if (completeSkills.Count < shuffleCount) return;
        if (rangeMin < 0 || rangeMax < 0 || rangeMin > completeSkills.Count || rangeMax > completeSkills.Count || rangeMin > rangeMax)
        {
            return;
        }


        for (int i = 0; i < shuffleCount; i++)
        {
            var swap = completeSkills[i];
            var pivot = Random.Range(rangeMin, rangeMax);
            completeSkills[i] = completeSkills[pivot];
            completeSkills[pivot] = swap;
        }
    }

    public SkillData GetSkill(int indx)
    {
        return skills[indx];
    }

    public SkillData GetCompleteSkill(int indx)
    {
        return completeSkills[indx];
    }
}
