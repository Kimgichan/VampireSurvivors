using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Focus : MonoBehaviour
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
        if (target == null || GameManager.Instance == null || GameManager.Instance.timeScaleController == null)
        {
            OffFocus();
            return;
        }

        var TC = GameManager.Instance.timeScaleController;
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

    private void TurnOnActive()
    {
        if(activeCor != null)
        {
            StopCoroutine(activeCor);
        }

        GameManager.GetFocusController()?.AddFocus(this);

        activeCor = ActiveCor();
        StartCoroutine(activeCor);
    }

    private void TurnOffActive()
    {
        if (activeCor != null)
        {
            StopCoroutine(activeCor);
            activeCor = null;
        }

        GameManager.GetFocusController()?.RemoveFocus(this);
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
