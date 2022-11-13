using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFocus : Focus
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform target;
    [SerializeField] private Transform parent;
    [SerializeField] private Vector2 size;

    private IEnumerator activeCor;


    public Transform Target
    {
        get
        {
            if (gameObject.activeSelf)
            {
                return target;
            }
            return null;
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (target == null || (!target.gameObject.activeSelf) || GameManager.Instance == null || GameManager.Instance.timeScaleController == null)
        {
            OffFocus();
            return;
        }

        transform.position = target.position;
    }


    public void OnFocus(Transform target, Vector2 size)
    {
        gameObject.SetActive(true);
        this.target = target;
        parent = transform.parent;

        transform.parent = null;
        transform.position = target.position;
        transform.rotation = Quaternion.identity;

        this.size = size;
        TurnOnActive();
    }

    public void OffFocus()
    {
        gameObject.SetActive(false);
        transform.parent = parent;

        TurnOffActive();
    }

    protected override void TurnOnActive()
    {
        base.TurnOnActive();

        if (activeCor != null)
        {
            StopCoroutine(activeCor);
        }

        activeCor = ActiveCor();
        StartCoroutine(activeCor);
    }

    protected override void TurnOffActive()
    {
        base.TurnOffActive();

        if (activeCor != null)
        {
            StopCoroutine(activeCor);
            activeCor = null;
        }
    }
    private IEnumerator ActiveCor()
    {
        while (true)
        {
            if (Target == null || (!Target.gameObject.activeSelf))
            {
                OffFocus();
            }

            yield return null;

            var FC = GameManager.GetFocusController();
            if (FC == null) continue;

            spriteRenderer.size = size * FC.FocusScale;
        }
    }
}
