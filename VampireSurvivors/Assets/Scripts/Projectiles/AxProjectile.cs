using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxProjectile : Projectile
{
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform rotateTr;
    [SerializeField] private Ax weapon;
    [SerializeField] private Collider2D collider2;
    [SerializeField] private Rigidbody2D rigid2D;
    [SerializeField] private float attackTimePercent;
    [SerializeField] private float reloadTimePercent;
    [SerializeField] private float rotSpeed;
    [SerializeField] private float pushForce;
    [SerializeField] private float pullingForce;
    [SerializeField] private float force;
    [SerializeField] private Vector2 dir;
    [SerializeField] private float height;

    private HashSet<Creature> hitCreatures;

    private IEnumerator actionCor;

    private void Start()
    {
        hitCreatures = new HashSet<Creature>();
        collider2.enabled = false;
    }

    public void Shot(Ax weapon)
    {
        if (weapon == null) return;

        hitCreatures.Clear();
        this.weapon = weapon;
        gameObject.SetActive(true);

        transform.parent = null;

        collider2.enabled = true;

        if(actionCor != null)
        {
            StopCoroutine(actionCor);
        }

        actionCor = UpdateCor();
        StartCoroutine(actionCor);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(weapon.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            var monster = collision.gameObject.GetComponent<Monster>();
            if(monster != null)
            {
                if (!hitCreatures.Contains(monster))
                {
                    hitCreatures.Add(monster);
                    monster.OnHit(weapon.GetDamage(), dir * force);
                }
            }
        }
        else if(weapon.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            var character = collision.gameObject.GetComponent<Character>();
            if(character != null)
            {
                if (!hitCreatures.Contains(character))
                {
                    hitCreatures.Add(character);
                    character.OnHit(weapon.GetDamage());
                }
            }
        }
    }

    private void TurnOff()
    {
        trailRenderer.Clear();
        collider2.enabled = false;
        hitCreatures.Clear();
        if (actionCor != null)
        {
            StopCoroutine(actionCor);
            actionCor = null;
        }

        if(weapon == null)
        {
            Destroy(gameObject);
        }
        else
        {
            weapon.Enqueue(this);
        }
    }

    private IEnumerator UpdateCor()
    {
        //var moveForce = weapon.Focus.TargetPos - (Vector2)transform.position;
        dir = (weapon.Focus.TargetPos - (Vector2)weapon.transform.position).normalized;
        force = pushForce;
        var currentTime = 0f;
        var maxTime = weapon.WeaponData.GetCooltime(weapon.Level)*attackTimePercent / (attackTimePercent + reloadTimePercent);
        //var startPos = (Vector2)transform.position;
        var rotAngleForce = Vector3.zero;

        #region 공격 업데이트
        if (dir.x < 0f)
        {
            rotAngleForce.z = rotSpeed;
        }
        else
        {
            rotAngleForce.z = -rotSpeed;
        }

        while(currentTime < maxTime)
        {
            yield return null;

            #region
            var TSC = GameManager.GetTimeScaleController();
            var _rotAngleForce = rotAngleForce;
            if(TSC != null)
            {
                currentTime += TSC.GameTimeScaleUpdate;
                _rotAngleForce *= TSC.GameTimeScaleUpdate;
            }
            else
            {
                currentTime += Time.deltaTime;
                _rotAngleForce *= Time.deltaTime;
            }
            #endregion
            //transform.position = startPos + moveForce * (currentTime / maxTime);
            transform.position = Vector2.Lerp(weapon.transform.position, weapon.Focus.TargetPos, currentTime / maxTime);
            rotateTr.localEulerAngles += _rotAngleForce;
        }
        transform.position = weapon.Focus.TargetPos;
        weapon.Focus.OffFocus();
        yield return null;
        #endregion

        currentTime = 0f;
        maxTime = weapon.WeaponData.GetCooltime(weapon.Level) * reloadTimePercent / (attackTimePercent + reloadTimePercent);

        rotAngleForce.z *= -1f;
        dir *= -1f;
        force = pullingForce;
        hitCreatures.Clear();
        while (currentTime < maxTime)
        {
            #region
            yield return null;

            var TSC = GameManager.GetTimeScaleController();
            var _rotAngleForce = rotAngleForce;
            if (TSC != null)
            {
                currentTime += TSC.GameTimeScaleUpdate;
                _rotAngleForce *= TSC.GameTimeScaleUpdate;
            }
            else
            {
                currentTime += Time.deltaTime;
                _rotAngleForce *= Time.deltaTime;
            }
            #endregion

            rotateTr.localEulerAngles += _rotAngleForce;

            var pos = Vector2.Lerp(weapon.Focus.TargetPos, weapon.transform.position, currentTime / maxTime);

            pos.y += -height*(4f * Mathf.Pow(currentTime / maxTime - 0.5f, 2f) - 1f);
            transform.position = pos;
        }

        transform.position = weapon.transform.position;
        TurnOff();
    }
}
