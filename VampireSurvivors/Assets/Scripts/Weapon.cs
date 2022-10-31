using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform weaponModel;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private WeaponAnimator weaponAnim;
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Focus focus;
    [SerializeField] private int level;
    [SerializeField] private Vector2 focusSize;
    [SerializeField] private Bullet prefab;
    [SerializeField] private int bulletCapacity;
    [SerializeField] private bool defaultRight;

    private IEnumerator shootCor;
    private IEnumerator searchFocusCor;
    private Queue<Bullet> bullets;


    public WeaponData WeaponData => weaponData;
    public int Level
    {
        get
        {
            return level;
        }
        set
        {
            if (value < 0) value = 0;
            else if (value > weaponData.MaxLevel) value = weaponData.MaxLevel;

            level = value;
        }
    }

    public Focus Focus => focus;

    public Vector3 AttackPoint => attackPoint.position;

    private void Start()
    {
        bullets = new Queue<Bullet>(bulletCapacity);
        for (int i = 0; i < bulletCapacity; i++)
        {
            var bullet = Instantiate(prefab, transform);
            bullet.transform.localPosition = Vector3.zero;
            bullets.Enqueue(bullet);
        }
    }


    private void FixedUpdate()
    {
        Rotate();

        SearchFocus();

        if (shootCor == null)
        {
            shootCor = ShootCor();
            StartCoroutine(shootCor);
        }

        if(weaponAnim != null)
        {
            var TSC = GameManager.GetTimeScaleController();
            if(TSC != null)
            {
                weaponAnim.AnimSpeed = TSC.gameTimeScale;
            }
        }
    }

    private void OnDisable()
    {
        shootCor = null;
        searchFocusCor = null;
    }

    public void Enqueue(Bullet bullet)
    {
        bullets.Enqueue(bullet);
    }
    public int GetDamage()
    {
        return Random.Range(WeaponData.GetMinDamage(level), WeaponData.GetMaxDamage(level) + 1);
    }
    /// <summary>
    /// importance = true => 탐색 초기화(=무기 레벨의 변화가 있을 때 호출)
    /// </summary>
    /// <param name="importance"></param>
    public void SearchFocus(bool importance = false)
    {
        if (importance)
        {
            if (searchFocusCor != null)
            {
                StopCoroutine(searchFocusCor);
                searchFocusCor = null;
            }
        }

        if (searchFocusCor == null)
        {
            searchFocusCor = SearchFocusCor();
            StartCoroutine(searchFocusCor);
        }
    }

    private void Rotate()
    {

        if (Focus.Target != null)
        {
            Vector2 dir = Focus.Target.position - transform.position;
            dir.Normalize();

            //var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.parent.rotation = Quaternion.FromToRotation(Vector3.right, dir);


            if(transform.parent.eulerAngles.z != 90f && transform.parent.eulerAngles.z != 270f)
            {
                if(transform.parent.eulerAngles.z > 90f && transform.parent.eulerAngles.z < 270f)
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
        else
        {
            DefaultRot();
            transform.parent.eulerAngles = Vector3.zero;
        }
    }

    private void DefaultRot()
    {
        if (defaultRight)
        {
            weaponModel.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            weaponModel.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    private IEnumerator SearchFocusCor()
    {
        while (true)
        {
            var _cooltime = WeaponData.GetSearchCooltime(level);
            while (_cooltime > 0f)
            {
                yield return null;

                if (GameManager.Instance == null ||
                    GameManager.Instance.timeScaleController == null)
                {
                    searchFocusCor = null;
                    yield break;
                }
                _cooltime -= GameManager.Instance.timeScaleController.GameTimeScaleUpdate;
            }

            if (GameManager.Instance == null || GameManager.Instance.monsterController == null)
            {
                searchFocusCor = null;
                yield break;
            }


            var CM = GameManager.Instance.monsterController;
            Transform target = null;
            var minRange = WeaponData.GetRange(level);

            if (Focus.Target != null)
            {
                var _minRange = (Focus.Target.position - transform.position).magnitude;
                if (_minRange > minRange)
                {
                    Focus.OffFocus();
                }
                else
                {
                    target = Focus.Target;
                    minRange = _minRange;
                }
            }

            if (gameObject.layer == LayerMask.NameToLayer("Character"))
            {
                for (int i = 0, icount = CM.FieldMonsterCount; i < icount; i++)
                {
                    if (i % 51 == 50)
                    {
                        yield return null;
                    }

                    var monster = CM.GetFieldMonster(i);
                    if (monster == null) continue;

                    var _rangeX2 = (monster.transform.position - transform.position).sqrMagnitude;

                    if (minRange * minRange > _rangeX2)
                    {
                        target = monster.transform;
                        minRange = Mathf.Sqrt(_rangeX2);
                    }
                }
            }
            else if (gameObject.layer == LayerMask.NameToLayer("Monster"))
            {

            }

            if (target != null)
            {
                Focus.OnFocus(target, focusSize);
            }

            yield return null;
        }
    }

    private IEnumerator ShootCor()
    {
        var cooltime = WeaponData.GetCooltime(level);

        while (cooltime > 0f)
        {
            yield return null;

            if (GameManager.Instance == null ||
                GameManager.Instance.timeScaleController == null)
            {
                shootCor = null;
                yield break;
            }
            cooltime -= GameManager.Instance.timeScaleController.GameTimeScaleUpdate;
        }

        if (Focus.Target != null)
        {
            if (bullets.Count > 0)
            {
                var bullet = bullets.Dequeue();
                bullet.Shoot(this);

                weaponAnim.OnAttack();

                var AC = GameManager.GetAudioController();
                AC?.PlaySFX("Bullet");
            }
        }

        shootCor = null;
    }
}
