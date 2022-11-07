using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class DamageTextController : MonoBehaviour
{
    [SerializeField] private DamageText prefab;
    [SerializeField] private int capacity;
    [SerializeField] private float zOrder;
    [SerializeField] private float showTime;
    [SerializeField] private float destScale;
    [SerializeField] private float openAxisY;
    [SerializeField] private float closeAxisY;
    [SerializeField] private Vector2 noiseMin;
    [SerializeField] private Vector2 noiseMax;
    [SerializeField] private float timePercentageOpen_1;
    [SerializeField] private float timePercentageOpen_2;
    [SerializeField] private float timePercentageStay;
    [SerializeField] private float timePercentageClose;

    [SerializeField] private Color monster;
    [SerializeField] private Color character;

    private Queue<DamageText> disableObjs;
    private Queue<DamageText> enableObjs;

    public float DestScale => destScale;
    public float OpenAxisY => openAxisY;
    public float CloseAxisY => closeAxisY;
    public Vector2 NoiseMin => noiseMin;
    public Vector2 NoiseMax => noiseMax;

    public float PercentageOpen_1 => timePercentageOpen_1;
    public float PercentageOpen_2 => timePercentageOpen_2;
    public float PercentageStay => timePercentageStay;
    public float PercentageClose => timePercentageClose;

    private IEnumerator Start()
    {
        while (GameManager.Instance == null)
            yield return null;

        if(GameManager.Instance.damageTextController != null)
        {
            Destroy(gameObject);
            yield break;
        }

        GameManager.Instance.damageTextController = this;
        disableObjs = new Queue<DamageText>(capacity);
        enableObjs = new Queue<DamageText>(capacity);

        for(int i = 0; i<capacity; i++)
        {
            var damageTxt = Instantiate(prefab, transform);
            disableObjs.Enqueue(damageTxt);
        }
    }

    public void TurnOnPopup(int damage, Vector2 pos, Enums.Creature creature = Enums.Creature.Monster)
    {
        DamageText damageTxt;
        if (disableObjs.Count > 0)
            damageTxt = disableObjs.Dequeue();
        else 
            damageTxt = enableObjs.Dequeue();


        pos.x += Random.Range(noiseMin.x, noiseMax.x);
        pos.y += Random.Range(noiseMin.y, noiseMax.y);

        damageTxt.ShowDamage(showTime, new Vector3(pos.x, pos.y, zOrder));
        damageTxt.Damage = damage;

        if(creature == Enums.Creature.Character)
        {
            damageTxt.TextColor = character;
        }
        else if(creature == Enums.Creature.Monster)
        {
            damageTxt.TextColor = monster;
        }

        enableObjs.Enqueue(damageTxt);
    }

    public void TurnOffPopup()
    {
        var damageTxt = enableObjs.Dequeue();
        damageTxt.gameObject.SetActive(false);
        disableObjs.Enqueue(damageTxt);
    }
}
