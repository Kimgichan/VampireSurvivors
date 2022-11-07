using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    [SerializeField] private Pistol weapon;
    [SerializeField] private Collider2D collider2;
    [SerializeField] private Rigidbody2D rigid2D;
    [SerializeField] private Vector2 dir;
    [SerializeField] private Vector3 scale;
    [SerializeField] private float speed;

    private bool isStart;
    private IEnumerator shootCor;

    private void Start()
    {
        collider2.enabled = false;
        gameObject.SetActive(false);
        isStart = true;
    }


    public void Shoot(Pistol weapon)
    {
        if (weapon == null) return;

        this.weapon = weapon;
        dir = (weapon.Focus.Target.transform.position - weapon.AttackPoint).normalized;

        gameObject.SetActive(true);
        transform.parent = null;
        transform.localScale = scale;
        gameObject.transform.position = weapon.AttackPoint;


        if (shootCor != null)
        {
            StopCoroutine(shootCor);
        }

        collider2.enabled = true;
        shootCor = ShootCor();
        StartCoroutine(shootCor);
    }



    private IEnumerator ShootCor()
    {
        while (!isStart)
        {
            yield return null;
        }

        while (true)
        {

            var pos = transform.position;
            pos.z = pos.y * 0.001f;
            transform.position = pos;

            yield return null;

            if(weapon == null)
            {
                Destroy(gameObject);
                yield break;
            }

            if (!weapon.gameObject.activeInHierarchy)
            {
                transform.parent = weapon.transform;
                gameObject.SetActive(false);
                weapon.Enqueue(this);
                yield break;
            }


            if (GameManager.Instance == null || GameManager.Instance.timeScaleController == null)
            {
                continue;
            }

            var TSC = GameManager.Instance.timeScaleController;
            rigid2D.velocity = dir * TSC.gameTimeScale * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (weapon.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            var monster = collision.gameObject.GetComponent<Monster>();
            if (monster != null)
            {
                monster.OnHit(weapon.GetDamage(), dir * speed);
            }
            else
            {
                var character = collision.gameObject.GetComponent<Character>();
                if (character != null) return;
            }
        }
        else if (weapon.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {

        }


        TurnOff();
    }

    private void TurnOff()
    {
        if (weapon == null)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.parent = weapon.transform;
            gameObject.SetActive(false);
            weapon.Enqueue(this);
        }
    }
}
