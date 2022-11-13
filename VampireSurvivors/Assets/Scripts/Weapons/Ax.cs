using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ax : Weapon
{
    [SerializeField] protected EquipSlotBag equipSlotBag;
    [SerializeField] protected DestinationFocus focus;
    [SerializeField] protected AxProjectile axProjectile;
    [SerializeField] protected float defaultAngle;
    [SerializeField] protected float focusScale;
    [SerializeField] protected bool isAttack;
    protected IEnumerator searchFocusCor;


    public DestinationFocus Focus => focus;


    private void OnEnable()
    {
        if(searchFocusCor != null)
        {
            StopCoroutine(searchFocusCor);
        }

        searchFocusCor = SearchFocusCor();
        StartCoroutine(searchFocusCor);
    }

    public void Enqueue(AxProjectile axProjectile)
    {
        isAttack = false;
        axProjectile.transform.parent = transform;
        axProjectile.transform.localEulerAngles = new Vector3(0f, 0f, defaultAngle);
    }

    protected IEnumerator SearchFocusCor()
    {
        while (true)
        {
            yield return null;

            if (isAttack) continue;

            var GM = GameManager.Instance;
            if (GM == null) continue;

            var MC = GameManager.GetMonsterController();
            if (MC == null) continue;

            Transform target = null;
            var maxRange = WeaponData.GetRange(Level);
            var currentRange = maxRange / 2f;
            
            if(gameObject.layer == LayerMask.NameToLayer("Character"))
            {
                for(int i = 0, icount = MC.FieldMonsterCount; i<icount; i++)
                {
                    if(i % GM.FrameSearchCount + 1 == GM.FrameSearchCount)
                    {
                        yield return null;
                    }

                    var monster = MC.GetFieldMonster(i);
                    if (monster == null) continue;

                    var _rangeX2 = ((Vector2)(monster.transform.position - transform.position)).sqrMagnitude;

                    if(maxRange * maxRange >= _rangeX2 && currentRange * currentRange < _rangeX2)
                    {
                        target = monster.transform;
                        currentRange = Mathf.Sqrt(_rangeX2);
                    }
                }
            }
            else if(gameObject.layer == LayerMask.NameToLayer("Monster"))
            {
                var GC = GameManager.GetGameController();
                if(GC != null)
                {
                    target = GC.Player.transform;
                    currentRange = ((Vector2)(target.transform.position - transform.position)).magnitude;
                }
            }

            if(target != null && currentRange > maxRange / 2f && currentRange <= maxRange)
            {
                Focus.OnFocus(target.position, focusScale);
                axProjectile.Shot(this);
                isAttack = true;
            }
        }
    }
}
