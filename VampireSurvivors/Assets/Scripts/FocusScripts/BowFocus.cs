using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowFocus : Focus
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform parent;
    [SerializeField] private float scale;

    private IEnumerator actionCor;

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
        if(Target == null || (!Target.gameObject.activeSelf))
        {
            OffFocus();
            return;
        }

        transform.position = target.position;
    }

    public void OnFocus(Transform target, float scale)
    {
        if (target == null) return;

        gameObject.SetActive(true);
        this.target = target;
        parent = transform.parent;

        transform.parent = null;
        transform.position = target.position;

        this.scale = scale;
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

        if(actionCor != null)
        {
            StopCoroutine(actionCor);
        }

        actionCor = ActionCor();
        StartCoroutine(actionCor);
    }

    protected override void TurnOffActive()
    {
        base.TurnOffActive();

        if(actionCor != null)
        {
            StopCoroutine(actionCor);
            actionCor = null;
        }
    }

    private IEnumerator ActionCor()
    {
        while (true)
        {
            if(Target == null || (!Target.gameObject.activeSelf))
            {
                OffFocus();
            }

            yield return null;

            var FC = GameManager.GetFocusController();
            if (FC == null) continue;

            transform.localScale = Vector3.one * scale * FC.FocusScale;
        }
    }
}
