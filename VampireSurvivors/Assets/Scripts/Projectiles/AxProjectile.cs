using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxProjectile : Projectile
{
    [SerializeField] private Ax weapon;
    [SerializeField] private Collider2D collider2;
    [SerializeField] private Rigidbody2D rigid2D;
    [SerializeField] private Vector2 destination;
    [SerializeField] private Vector3 scale;
    [SerializeField] private float speed;

    private HashSet<Creature> hitCreatures;

    private IEnumerator actionCor;

    private void Start()
    {
        hitCreatures = new HashSet<Creature>();
    }

    public void Shoot(Ax weapon)
    {
        if (weapon == null) return;

        this.weapon = weapon;
        gameObject.SetActive(true);

        transform.parent = null;

        collider2.enabled = true;
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
                    monster.OnHit(weapon.GetDamage(), ((Vector2)(transform.position - monster.transform.position)).normalized * speed);
                }
            }
        }
        else if(weapon.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {

        }
    }

    private void TurnOff()
    {

    }
}
