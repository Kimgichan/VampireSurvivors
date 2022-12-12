using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSlot : MonoBehaviour
{
    //[SerializeField] private RectTransform pivot;
    [SerializeField] private RectTransform rectTr;

    public RectTransform RectTr => rectTr;

    void Start()
    {
        //StartCoroutine(Cor());
    }

    //private IEnumerator Cor()
    //{
    //    var delay = new WaitForSeconds(0.5f);
    //    while (true)
    //    {
    //        yield return delay;

    //        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(pivot, tr.position, null, out Vector2 localPoint))
    //        {
    //            Debug.Log(localPoint.x);
    //        }
    //    }
    //}
}
