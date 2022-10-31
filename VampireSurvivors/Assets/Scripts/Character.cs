using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigid2D;
    [SerializeField] private int currentHP;
    [SerializeField] private float moveSpeed;

    public int OriginalHP
    {
        get
        {
            return 50;
        }
    }

    public Vector2 Velocity
    {
        get
        {
            return rigid2D.velocity;
        }
        set
        {
            if(GameManager.Instance == null || GameManager.Instance.timeScaleController == null)
            {
                rigid2D.velocity = Vector2.zero;
                return;
            }
            rigid2D.velocity = value * GameManager.Instance.timeScaleController.gameTimeScale * moveSpeed;
        }
    }

    private void FixedUpdate()
    {
        var pos = transform.position;
        pos.z = pos.y * 0.001f;
        transform.position = pos;
    }

    public void OnHit(int damage)
    {

    }
}
