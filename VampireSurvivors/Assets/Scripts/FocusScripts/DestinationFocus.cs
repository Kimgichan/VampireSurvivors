using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationFocus : Focus
{
    [SerializeField] private Transform parent;
    [SerializeField] private float scale;
    [SerializeField] private Vector2 destination;

    private IEnumerator actionCor;


    public Vector2 TargetPos => destination;

    private void Start()
    {
        gameObject.SetActive(false);
    }


    public void OnFocus(Vector2 destination, float scale)
    {
        gameObject.SetActive(true);
        this.scale = scale;
        this.destination = destination;

        parent = transform.parent;
        transform.parent = null;
        transform.position = this.destination;
        TurnOnActive();
    }

    public void OffFocus()
    {
        transform.parent = parent;
        gameObject.SetActive(false);
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

        if (actionCor != null)
        {
            StopCoroutine(actionCor);
            actionCor = null;
        }
    }

    private IEnumerator ActionCor()
    {
        while (true)
        {
            yield return null;

            var FC = GameManager.GetFocusController();
            if (FC == null) continue;

            transform.localScale = Vector3.one * (scale * FC.FocusScale);
        }
    }
}
