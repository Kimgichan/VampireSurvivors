using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

/// <summary>
/// MonsterController 역할을 대체
/// </summary>
public class NetMonsterControllerAgent : MonoBehaviour
{
    [SerializeField] private NetGameRuler gameRuler;

    [SerializeField] private Vector2 spawnRange;
    [SerializeField] private float spawnCooltime;
    /// <summary>
    /// 필드에 활성화될 수 있는 몬스터의 수
    /// </summary>
    [SerializeField] private int capacity;
    [SerializeField] private Transform map;

    [Space]
    [ReadOnly] [SerializeField] private int remainCount;
    [SerializeField] List<Monster> remainMonsters;
    [ReadOnly] [SerializeField] private int fieldCount = 0;
    [ReadOnly] [SerializeField] List<Monster> fieldMonsters;


    private void Start()
    {
        remainCount = remainMonsters.Count;
        for(int i = 0; i<remainCount; i++)
        {
            remainMonsters[i].id = i;
        }

        fieldMonsters = new List<Monster>(capacity);
        fieldCount = 0;

        for(int i = 0, icount = fieldCount; i<icount; i++)
        {
            fieldMonsters.Add(null);
        }
    }
    private void OnEnable()
    {
        MonsterReset();
        StartCoroutine(SpawnTickCor());
    }

    private void MonsterReset()
    {
        for(int i = 0; i < fieldCount; i++)
        {
            var monster = fieldMonsters[i];
            monster.transform.parent = transform;
            monster.Deactive();
            monster.gameObject.SetActive(false);

            remainMonsters[remainCount++] = monster;
            fieldMonsters[i] = null;
        }

        fieldCount = 0;
    }

    public void GetMonstersInfo(out NetNodes.Server.MonstersInfo info)
    {
        info = new NetNodes.Server.MonstersInfo();

        info.ID_List = new int[fieldCount];
        info.posList = new Vector2[fieldCount];
        info.currentHPList = new int[fieldCount];

        for (int i = 0; i < fieldCount; i++)
        {
            var monster = fieldMonsters[i];
            if (monster == null)
            {
                info.ID_List[i] = -1;
                continue;
            }

            info.ID_List[i] = monster.id;
            info.posList[i] = monster.transform.localPosition;
            info.currentHPList[i] = monster.CurrentHP;
        }


    }

    private IEnumerator SpawnTickCor()
    {
        var wait = new WaitForSeconds(spawnCooltime);
        while (true)
        {
            yield return wait;

            if (gameRuler == null || gameRuler.Room == null)
            {
                gameObject.SetActive(false);
                yield break;
            }

            SpawnTick();
        }
    }
    private void SpawnTick()
    {
        var newMonster = Pop();
        if (newMonster == null) return;

        Vector3 pos = Vector3.zero;
        pos.x = Random.Range(-spawnRange.x, spawnRange.x);
        pos.y = Random.Range(-spawnRange.y, spawnRange.y);

        newMonster.transform.localPosition = pos;
        newMonster.Active();
    }
    private void Push(Monster monster)
    {
        if (monster == null) return;

        var indx = fieldMonsters.FindIndex((_monster) => monster == _monster);

        if(indx > -1)
        {
            fieldMonsters[indx] = fieldMonsters[fieldCount - 1];
            fieldMonsters[fieldCount - 1] = null;
            fieldCount -= 1;

            remainMonsters[remainCount] = monster;
            remainCount += 1;

            monster.transform.parent = transform;
            monster.Deactive();
            monster.gameObject.SetActive(false);
            return;
        }
    }
    private Monster Pop()
    {
        if (remainCount <= 0) return null;

        var choice = Random.Range(0, remainCount);
        var monster = remainMonsters[choice];

        fieldMonsters[fieldCount] = monster;
        fieldCount += 1;

        remainCount -= 1;
        remainMonsters[choice] = remainMonsters[remainCount];
        remainMonsters[remainCount] = null;

        monster.transform.parent = map;
        return monster;
    }
}
