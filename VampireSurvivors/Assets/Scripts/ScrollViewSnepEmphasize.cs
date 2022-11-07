using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewSnepEmphasize : MonoBehaviour
{
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;
    [SerializeField] private List<RectTransform> slots;
    [SerializeField] private RectTransform pivot;
    [SerializeField] private int updateFrame;
    [SerializeField] private float emphasizeRange;

    private IEnumerator resizeCor;

    public void TurnOnResizeUpdate()
    {
        if(resizeCor != null)
        {
            StopCoroutine(resizeCor);
        }
        resizeCor = ResizeCor();

        StartCoroutine(resizeCor);
    }

    public void TurnOffResizeUpdate()
    {
        if(resizeCor != null)
        {
            StopCoroutine(resizeCor);
            resizeCor = null;
        }
    }
    private IEnumerator ResizeCor()
    {
        var i = 0;
        while (true)
        {
            yield return null;
            var range = emphasizeRange * (Screen.width / canvasScaler.referenceResolution.x) * 0.5f;
            for (int f = 0; f< updateFrame; f++)
            {
                var slot = slots[i];
                if(RectTransformUtility.ScreenPointToLocalPointInRectangle(pivot, slot.position, null, out Vector2 localPoint))
                {
                    var _x = Mathf.Abs(localPoint.x);
                    Vector3 scale;
                    if (_x <= range)
                    {
                        scale = Vector3.one * Mathf.Lerp(maxScale, minScale, _x / range);
                    }
                    else scale = Vector3.one * minScale;

                    scale.z = 1f;

                    slot.localScale = scale;
                }


                i += 1;
                if(i >= slots.Count)
                {
                    i = 0;
                    break;
                }
            }
        }
    }
}
