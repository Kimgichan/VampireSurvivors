using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nodes;

public class LightingBolt : Bullet
{
    [SerializeField] private Transform target;
    [SerializeField] private List<Transform> bolts;
    [SerializeField] private List<TrList> boltPosList;
    [SerializeField] private List<TargetFocus> focusList;
    [SerializeField] private float runTime;
    [SerializeField] private float linkTimePercent;
    [SerializeField] private float linkRange;
    [SerializeField] private float force;
    [SerializeField] private Vector2 focusSize;

    protected override void Start()
    {
        collider2.enabled = false;
    }

    public override void Shot(Pistol weapon)
    {
        collider2.enabled = false;
        if (weapon == null) return;

        this.weapon = weapon;
        target = weapon.Focus.Target;

        if(target == null)
        {
            weapon = null;
            return;
        }

       
        gameObject.SetActive(true);
        transform.parent = null;
        transform.localScale = Vector3.one;
        gameObject.transform.position = weapon.AttackPoint;

        bolts[0].gameObject.SetActive(true);
        for(int i = 0, icount = boltPosList[0].trs.Count; i<icount; i++)
        {
            boltPosList[0].trs[i].transform.position = weapon.AttackPoint;
        }
        boltPosList[0].trs[1].parent = boltPosList[0].trs[0];
        //boltPosList[0].trs[2].parent = weapon.AttackPointTr;
        //boltPosList[0].trs[2].localPosition = Vector3.zero;

        for(int i = 1, icount = bolts.Count; i<icount; i++)
        {
            bolts[i].gameObject.SetActive(false);
        }

        StartCoroutine(RunCor());
    }

    private IEnumerator RunCor()
    {

        var originalTimer = runTime * linkTimePercent;
        var currentTimer = originalTimer;
        while(currentTimer > 0f)
        {
            yield return null;

            #region TSC
            var TSC = GameManager.GetTimeScaleController();
            if(TSC != null)
            {
                currentTimer -= TSC.GameTimeScaleUpdate;
            }
            else
            {
                currentTimer -= Time.deltaTime;
            }
            #endregion

            if(target == null || (!target.gameObject.activeSelf))
            {
                for(int i = 0; i<bolts.Count; i++)
                {
                    bolts[i].gameObject.SetActive(false);
                }

                TurnOff();
                yield break;
            }

            var vec2 = Vector2.Lerp(target.position, weapon.AttackPoint, currentTimer / originalTimer);
            boltPosList[0].trs[0].position = vec2;
            boltPosList[0].trs[2].position = weapon.AttackPoint;
        }
        boltPosList[0].trs[0].position = target.position;
        boltPosList[0].trs[2].position = weapon.AttackPoint;

        if (weapon.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {

        }
        else if(weapon.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            if(target.TryGetComponent(out Monster monster))
            {
                monster.OnHit(weapon.GetDamage(), Vector2.zero);
            }

            var linkAttack = LinkAttackCor_Monster();
            while (linkAttack.MoveNext())
            {
                yield return linkAttack;
            }
        }
    }

    private IEnumerator LinkAttackCor_Monster()
    {
        var GM = GameManager.Instance;
        var MC = GameManager.GetMonsterController();
        boltPosList[0].trs[2].parent = null;

        while (GM == null || MC == null)
        {
            yield return null;
            GM = GameManager.Instance;
            MC = GameManager.GetMonsterController();
        }

        Monster[] targets = new Monster[bolts.Count - 1];
        float[] distArray = new float[bolts.Count - 1];

        for (int i = 0; i < bolts.Count - 1; i++)
        {
            targets[i] = null;
            distArray[i] = -1f;
        }

        for (int i = 0; i < MC.FieldMonsterCount; i++)
        {
            if (i % GM.FrameSearchCount + 1 == GM.FrameSearchCount)
            {
                yield return null;
            }

            var monster = MC.GetFieldMonster(i);
            if (monster == null || (!monster.gameObject.activeSelf)) continue;

            var _rangeX2 = ((Vector2)(monster.transform.position - boltPosList[0].trs[0].position)).sqrMagnitude;

            for (int indx = 0; indx < bolts.Count - 1; indx++)
            {
                if (_rangeX2 > distArray[indx] * distArray[indx] && _rangeX2 <= linkRange * linkRange)
                {
                    var swapTarget = targets[indx];
                    var swapDist = distArray[indx];

                    targets[indx] = monster;
                    distArray[indx] = Mathf.Sqrt(_rangeX2);

                    indx += 1;
                    for (; indx < bolts.Count - 1; indx++)
                    {
                        var _swapTarget = targets[indx];
                        var _swapDist = distArray[indx];

                        targets[indx] = swapTarget;
                        distArray[indx] = swapDist;

                        swapTarget = _swapTarget;
                        swapDist = _swapDist;
                    }
                    break;
                }
            }
        }


        var originalTimer = runTime * (1f - linkTimePercent);
        var currentTimer = originalTimer;
        var openTime = originalTimer * 0.3f;

        for(int i = 1; i<bolts.Count; i++)
        {
            var target = targets[i - 1];
            if(target != null && target.gameObject.activeSelf)
            {
                bolts[i].gameObject.SetActive(true);
                var posList = boltPosList[i];

                posList.trs[0].position = boltPosList[0].trs[0].position;

                posList.trs[1].parent = posList.trs[0];
                posList.trs[1].localPosition = Vector3.zero;

                posList.trs[2].position = boltPosList[0].trs[0].position;
                focusList[i - 1].OnFocus(target.transform, focusSize);
            }
            else
            {
                bolts[i].gameObject.SetActive(false);
            }
        }

        while (currentTimer > 0f)
        {
            yield return null;

            #region TSC
            var TSC = GameManager.GetTimeScaleController();
            if (TSC != null)
            {
                currentTimer -= TSC.GameTimeScaleUpdate;
            }
            else
            {
                currentTimer -= Time.deltaTime;
            }
            #endregion

            for (int i = 1; i < bolts.Count; i++)
            {
                var target = targets[i - 1];
                if (target == null || (!target.gameObject.activeSelf))
                {
                    bolts[i].gameObject.SetActive(false);
                    continue;
                }
                else bolts[i].gameObject.SetActive(true);

                if(currentTimer <= openTime)
                {
                    boltPosList[i].trs[1].parent = null;
                }

                var trs = boltPosList[i].trs;
                trs[0].position = Vector2.Lerp(target.transform.position, boltPosList[0].trs[0].position, currentTimer);
            }
            boltPosList[0].trs[2].position = Vector2.Lerp(boltPosList[0].trs[0].position, weapon.AttackPoint, currentTimer);

        }

        for(int i = 0; i<targets.Length; i++)
        {
            if (targets[i] == null) continue;
            targets[i].OnHit(weapon.GetDamage(), ((Vector2)(boltPosList[i + 1].trs[2].position - boltPosList[i + 1].trs[0].position)).normalized * force);
        }
        TurnOff();
    }

    protected override void TurnOff()
    {
        for(int i = 0; i<boltPosList.Count; i++)
        {
            for(int j = 0; j<boltPosList[i].trs.Count; j++)
            {
                boltPosList[i].trs[j].parent = bolts[i];
            }
        }

        for(int i = 0; i<focusList.Count; i++)
        {
            focusList[i].OffFocus();
        }

        base.TurnOff();
    }
}

