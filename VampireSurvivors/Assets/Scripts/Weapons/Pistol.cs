using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    /// <summary>
    /// ����� ����� Ȥ�� �ٸ� ������� ����(��ȣ�ۿ�), ������ ���� ���� Ư�� ȿ�� ��ġ ���� ����
    /// EquipSlotBag�� null���̾ ����� �۵��� �� �ְ� �ۼ�(��ž ���� ���)
    /// </summary>
    [SerializeField] protected EquipSlotBag equipSlotBag;
    [SerializeField] protected Transform weaponModel;
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected WeaponAnimator weaponAnim;
    [SerializeField] protected TargetFocus focus;
    [SerializeField] protected Vector2 focusSize;
    [SerializeField] protected Bullet prefab;
    [SerializeField] protected int bulletCapacity;
    [SerializeField] protected bool defaultRight;

    protected IEnumerator shootCor;
    protected IEnumerator searchFocusCor;
    protected Queue<Bullet> bullets;

    public TargetFocus Focus => focus;

    public Vector3 AttackPoint => attackPoint.position;
    public Transform AttackPointTr => attackPoint;

    protected void Start()
    {
        bullets = new Queue<Bullet>(bulletCapacity);
        for (int i = 0; i < bulletCapacity; i++)
        {
            var bullet = Instantiate(prefab, transform);
            bullet.transform.localPosition = Vector3.zero;
            bullets.Enqueue(bullet);
        }
    }


    protected void FixedUpdate()
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

    protected void OnDisable()
    {
        shootCor = null;
        searchFocusCor = null;
    }

    public void Enqueue(Bullet bullet)
    {
        bullets.Enqueue(bullet);
    }

    /// <summary>
    /// importance = true => Ž�� �ʱ�ȭ(=���� ������ ��ȭ�� ���� �� ȣ��)
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

    protected void Rotate()
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

    protected IEnumerator SearchFocusCor()
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
                    if (i % GameManager.Instance.FrameSearchCount + 1 == GameManager.Instance.FrameSearchCount)
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

    protected IEnumerator ShootCor()
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
                bullet.Shot(this);

                weaponAnim.OnAttack();

                var AC = AudioManager.GetAudioController();
                AC?.PlaySFX("Bullet");
            }
        }

        shootCor = null;
    }
}
