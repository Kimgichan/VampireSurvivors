using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour 
{
    /// <summary>
    /// ������ ���͵�
    /// </summary>
    [SerializeField] private List<Monster> monsters;
    [SerializeField] private int remainCount; 

    /// <summary>
    /// �ʵ忡 Ȱ��ȭ�� ���͵�
    /// </summary>
    [SerializeField] private List<Monster> fieldMonsters;
    [SerializeField] private int fieldCount;

    /// <summary>
    /// �ʵ忡 Ȱ��ȭ�� �� �ִ� ������ �ִ� ī��Ʈ
    /// </summary>
    [SerializeField] private int capacity;
    [SerializeField] private Vector2 spawnRange;
    [SerializeField] private float spawnCooltime;
    [SerializeField] private Transform map;

    /// <summary>
    /// �ʵ忡�� Ȱ��ȭ���� ���� ���� ī��Ʈ
    /// </summary>
    public int RemainMonsterCount => remainCount;
    /// <summary>
    /// �ʵ忡�� Ȱ��ȭ�� ���� ī��Ʈ
    /// </summary>
    public int FieldMonsterCount => fieldCount;


    private IEnumerator Start()
    {
        while (GameManager.Instance == null)
            yield return null;

        if(GameManager.Instance.monsterController != null)
        {
            Destroy(gameObject);
            yield break;
        }

        GameManager.Instance.monsterController = this;

        fieldMonsters.Clear();
        for(int i = 0, icount = monsters.Count; i<icount; i++)
        {
            monsters[i].transform.parent = transform;
            fieldMonsters.Add(null);
        }
        remainCount = monsters.Count;
        fieldCount = 0;
        StartCoroutine(SpawnCor());
    }

    public Monster GetFieldMonster(int indx)
    {
        return fieldMonsters[indx];
    }
    
    public void Push(Monster monster)
    {
        if (monster == null) return;

        for(int i = 0; i<fieldCount; i++)
        {
            var _monster = fieldMonsters[i];
            if((object)monster == _monster)
            {
                fieldMonsters[i] = fieldMonsters[fieldCount - 1];
                fieldMonsters[fieldCount - 1] = null;
                fieldCount -= 1;

                monsters[remainCount] = monster;
                remainCount += 1;

                monster.transform.parent = transform;
                monster.gameObject.SetActive(false);
                return;
            }
        }

        if (!monsters.Contains(monster))
            Destroy(monster.gameObject);
    }

    public Monster Pop()
    {
        if (remainCount <= 0) return null; 

        var choice = Random.Range(0, remainCount);
        var monster = monsters[choice];

        fieldMonsters[fieldCount] = monster;
        fieldCount += 1;

        remainCount -= 1;
        monsters[choice] = monsters[remainCount];
        monsters[remainCount] = null;

        monster.transform.parent = null;
        return monster;
    }

    private IEnumerator SpawnCor()
    {
        while (true)
        {
            if (FieldMonsterCount < capacity)
            {
                var cooltime = spawnCooltime;
                while (cooltime > 0f)
                {
                    if (GameManager.Instance != null && GameManager.Instance.timeScaleController != null)
                    {
                        cooltime -= GameManager.Instance.timeScaleController.GameTimeScaleUpdate;
                    }
                    yield return null;
                }

                var newMonster = Pop();
                if (newMonster != null)
                {
                    newMonster.transform.parent = null;
                    //var pos = newMonster.transform.position;
                    Vector3 pos = Vector3.zero;
                    pos.x = Random.Range(-spawnRange.x, spawnRange.x);
                    pos.y = Random.Range(-spawnRange.y, spawnRange.y);

                    var wPos = map.TransformPoint(pos);

                    wPos.z = newMonster.transform.position.z;

                    newMonster.transform.position = wPos;
                    newMonster.Active();
                }
            }
            yield return null;
        }
    }
}
