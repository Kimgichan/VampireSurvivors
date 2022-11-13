using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{
    [SerializeField] protected EquipSlotBag equipSlotBag;
    [SerializeField] protected Transform weaponModel;
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected WeaponAnimator weaponAnim;
    [SerializeField] protected BowFocus focus;
    [SerializeField] protected float focusScale;
    [SerializeField] protected float attackRange;
    [SerializeField] protected LineRenderer arrow;
    [SerializeField] protected float arrowTrailTime;
    [SerializeField] protected bool defaultRight;
    [SerializeField] protected Vector2 shotDir;
    [SerializeField] protected float pushForce;
    [SerializeField] protected bool isShot;

    public BowFocus Focus => focus;
    

    protected void OnEnable()
    {
        isShot = false;
        arrow.enabled = false;
        StartCoroutine(ShotCor());
        StartCoroutine(SearchCor());
        StartCoroutine(RotateCor());
    }

    protected void FixedUpdate()
    {
        if(weaponAnim != null)
        {
            var TSC = GameManager.GetTimeScaleController();
            if(TSC != null)
            {
                weaponAnim.AnimSpeed = TSC.gameTimeScale;
            }
        }
    }

    protected IEnumerator ShotCor()
    {
        while (true)
        {
            yield return null;

            if (Focus.Target == null) continue;

            shotDir = Focus.Target.position - transform.position;
            shotDir.Normalize();
            isShot = true;
            weaponAnim.OnAttack();

            var cooltime = WeaponData.GetCooltime(Level);
            while(cooltime > 0f)
            {
                yield return null;

                var TSC = GameManager.GetTimeScaleController();
                if (TSC != null)
                {
                    cooltime -= TSC.GameTimeScaleUpdate;
                }
                else cooltime -= Time.deltaTime;
            }
        }
    }

    protected IEnumerator SearchCor()
    {
        while (true)
        {
            yield return null;

            var cooltime = WeaponData.GetSearchCooltime(Level);
            while(cooltime > 0f)
            {
                yield return null;

                var TSC = GameManager.GetTimeScaleController();
                if (TSC != null)
                {
                    cooltime -= TSC.GameTimeScaleUpdate;
                }
                else cooltime -= Time.deltaTime;
            }

            Transform target = null;
            var minRange = WeaponData.GetRange(Level);

            if(Focus.Target != null)
            {
                var _minRangeX2 = ((Vector2)(Focus.Target.position - transform.position)).sqrMagnitude;
                if(minRange * minRange < _minRangeX2)
                {
                    Focus.OffFocus();
                }
                else
                {
                    target = Focus.Target;
                    minRange = Mathf.Sqrt(_minRangeX2);
                }
            }

            if(gameObject.layer == LayerMask.NameToLayer("Character"))
            {
                var GM = GameManager.Instance;
                if (GM == null) continue;
                var CM = GameManager.GetMonsterController();
                if (CM == null) continue;
                for(int i = 0, icount = CM.FieldMonsterCount; i<icount; i++)
                {
                    if (i % GM.FrameSearchCount + 1 == GM.FrameSearchCount)
                        yield return null;

                    var monster = CM.GetFieldMonster(i);
                    if (monster == null) continue;

                    var _rangeX2 = ((Vector2)(monster.transform.position - transform.position)).sqrMagnitude;

                    if(minRange * minRange > _rangeX2)
                    {
                        target = monster.transform;
                        minRange = Mathf.Sqrt(_rangeX2);
                    }
                }
            }
            else if(gameObject.layer == LayerMask.NameToLayer("Monster"))
            {

            }

            if(target != null)
            {
                Focus.OnFocus(target, focusScale);
            }
        }
    }

    protected IEnumerator RotateCor()
    {
        while (true)
        {
            yield return null;

            if (isShot) continue;

            if(Focus.Target == null)
            {
                DefaultRot();
                continue;
            }

            Vector2 dir = Focus.Target.position - transform.position;
            dir.Normalize();

            transform.parent.rotation = Quaternion.FromToRotation(Vector3.right, dir);

            if(transform.parent.eulerAngles.z != 90f && 
                transform.parent.eulerAngles.z != 270f)
            {
                if(transform.parent.eulerAngles.z > 90f &&
                    transform.parent.eulerAngles.z < 270f)
                {
                    weaponModel.localScale = new Vector3(1f, -1f, 1f);
                }
                else
                {
                    weaponModel.localScale = new Vector3(1f, 1f, 1f);
                }
            }
            else
            {
                DefaultRot();
            }
        }
    }
    protected void DefaultRot()
    {
        transform.parent.rotation = Quaternion.identity;
        if (defaultRight)
        {
            weaponModel.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            weaponModel.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    public void OnShot()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Character")) 
        {
            var hitList = Physics2D.RaycastAll(attackPoint.transform.position, shotDir, attackRange, 1<<LayerMask.NameToLayer("Monster"));

            for(int i = 0, icount = hitList.Length; i<icount; i++)
            {
                var hit = hitList[i];
                var monster = hit.transform.GetComponent<Monster>();
                if(monster != null)
                {
                    monster.OnHit(GetDamage(), shotDir * pushForce);
                }
            }
        }
        else if(gameObject.layer == LayerMask.NameToLayer("Monster"))
        {

        }

        arrow.enabled = true;
        arrow.positionCount = 2;
        arrow.SetPosition(0, (Vector2)attackPoint.transform.position);
        arrow.SetPosition(1, (Vector2)attackPoint.transform.position + shotDir * attackRange);
        //arrow.AddPosition((Vector2)(attackPoint.transform.position));
        //arrow.AddPosition((Vector2)attackPoint.transform.position + shootDir * attackRange);


        StartCoroutine(ArrowTrailCor());
        isShot = false;
    }

    protected IEnumerator ArrowTrailCor()
    {
        var time = arrowTrailTime;
        while(time > 0f)
        {
            yield return null;

            var TSC = GameManager.GetTimeScaleController();
            if(TSC != null)
            {
                time -= TSC.GameTimeScaleUpdate;
            }
            else
            {
                time -= Time.deltaTime;
            }
        }

        arrow.enabled = false;
        arrow.positionCount = 0;
    }
}
