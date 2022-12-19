using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class MonsterController : MonoBehaviour 
{
    [SerializeField] private List<Monster> monsters;
    [SerializeField] private Transform map;
    [SerializeField] private float lerpTime;
    private float lerpTimer;
    [ReadOnly] [SerializeField] private NetNodes.Server.MonstersInfo currentInfo;

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

    private void Update()
    {
        if(lerpTimer < lerpTime)
        {
            lerpTimer += Time.deltaTime;
            var percent = 1f - lerpTimer / lerpTime;
            for(int i = 0, icount = currentInfo.ID_List.Length; i<icount; i++)
            {
                var id = currentInfo.ID_List[i];
                if(id > -1)
                {
                    Vector3 pos = Vector2.Lerp(monsters[id].transform.localPosition, currentInfo.posList[i], percent);

                    pos.z = pos.y * 0.05f;
                    monsters[id].transform.localPosition = pos;
                    monsters[id].CurrentHP = currentInfo.currentHPList[i];
                }
            }
        }
    }

    public Monster GetFieldMonster(int indx)
    {
        return null;
    }
    
    public void Push(Monster monster)
    {
        
    }

    public void Update(in NetNodes.Server.MonstersInfo monstersInfo)
    {
        currentInfo = monstersInfo;
        lerpTimer = 0f;

        for(int i = 0, icount = monsters.Count; i<icount; i++)
        {
            monsters[i].gameObject.SetActive(false);
        }

        for(int i = 0, icount = monstersInfo.ID_List.Length; i<icount; i++)
        {
            var id = monstersInfo.ID_List[i];
            if(id > -1)
            {
                monsters[id].gameObject.SetActive(true);
            }
        }
    }
}

