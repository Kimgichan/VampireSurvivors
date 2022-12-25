using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class MonsterController : MonoBehaviour 
{
    [SerializeField] private List<Monster> monsters;
    private HashSet<int> checkList;
    [SerializeField] private Transform map;
    [ReadOnly] [SerializeField] private NetNodes.Server.MonstersInfo currentInfo;
    [SerializeField] private float lerpForce;
    private bool init = true;
    /// <summary>
    /// 필드에서 활성화된 몬스터 카운트
    /// 제거될 예정
    /// </summary>
    public int FieldMonsterCount => 0;

    public int MonsterLevel
    {
        get
        {
            var GC = GameManager.GetGameController();
            if(GC != null)
            {
                return GC.StageLevel;
            }
            return 1;
        }
    }



    private IEnumerator Start()
    {
        while (GameManager.Instance == null)
            yield return null;

        if (GameManager.Instance.monsterController != null)
        {
            Destroy(gameObject);
            yield break;
        }

        GameManager.Instance.monsterController = this;
    }

    private void FixedUpdate()
    {
        for(int i = 0, icount = currentInfo.monsters.Length; i<icount; i++)
        {
            Vector3 pos = Vector2.Lerp(monsters[currentInfo.monsters[i].id].transform.localPosition, currentInfo.monsters[i].pos, lerpForce);
            pos.z = pos.y * 0.05f;
            monsters[currentInfo.monsters[i].id].transform.localPosition = pos;
        }
    }

    public Monster GetFieldMonster(int indx)
    {
        return null;
    }
    

    public void SetMonstersInfo(in NetNodes.Server.MonstersInfo monstersInfo)
    {
        currentInfo = monstersInfo;

        if (checkList == null) checkList = new HashSet<int>();
        else checkList.Clear();

        for(int i = 0, icount = monstersInfo.monsters.Length; i<icount; i++)
        {
            checkList.Add(monstersInfo.monsters[i].id);
            monsters[monstersInfo.monsters[i].id].gameObject.SetActive(true);
            monsters[monstersInfo.monsters[i].id].CurrentHP = currentInfo.monsters[i].currentHP;
            monsters[monstersInfo.monsters[i].id].OriginalHP = currentInfo.monsters[i].originalHP;
        }

        for(int i =0, icount = monsters.Count; i<icount; i++)
        {
            if (!checkList.Contains(i))
            {
                monsters[i].gameObject.SetActive(false);
            }
        }

        Init();
    }

    private void Init()
    {
        if (init)
        {
            for(int i = 0, icount = currentInfo.monsters.Length; i<icount; i++)
            {
                monsters[currentInfo.monsters[i].id].transform.localPosition = currentInfo.monsters[i].pos;
            }

            init = false;
        }
    }
}

