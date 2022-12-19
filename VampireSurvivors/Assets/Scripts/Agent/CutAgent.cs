using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutAgent : MonoBehaviour
{
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private RectTransform rectTr;
    [SerializeField] private float time;
    [SerializeField] private float outTimePercent;
    [SerializeField] private float inTimePercent;
    [SerializeField] private float maxWidth;
    [SerializeField] private float minWidth;

    private bool complete;
    private IEnumerator cutCor;

    public bool Complete => complete;

    private void Start()
    {
        maxWidth = maxWidth * Screen.width / canvasScaler.referenceResolution.x;
        minWidth = minWidth * Screen.width / canvasScaler.referenceResolution.x;
        //StartCoroutine(CutCor());
    }


    public void OnCutIn()
    {
        if(cutCor!= null)
        {
            StopCoroutine(cutCor);
        }

        cutCor = CutInCor();
        StartCoroutine(cutCor);
    }


    public void OnCutOut()
    {
        if(cutCor != null)
        {
            StopCoroutine(cutCor);
        }

        cutCor = CutOutCor();
        StartCoroutine(cutCor);
    }


    private IEnumerator CutInCor()
    {
        complete = false;

        var maxTime = time * inTimePercent / (outTimePercent + inTimePercent);
        var currentTime = 0f;
        var size = rectTr.sizeDelta;

        var AC = AudioManager.GetAudioController();
        if(AC!= null)
        {
            AC.PlaySFX("Transition");
        }

        while (currentTime < maxTime)
        {
            yield return null;

            size.x = Mathf.Lerp(minWidth, maxWidth, currentTime / maxTime);
            rectTr.sizeDelta = size;
            currentTime += Time.deltaTime;
        }

        size.x = maxWidth;
        rectTr.sizeDelta = size;
        cutCor = null;
        complete = true;
    }

    private IEnumerator CutOutCor()
    {
        complete = false;

        var maxTime = time * outTimePercent / (outTimePercent + inTimePercent);
        var currentTime = 0f;
        var size = rectTr.sizeDelta;

        var AC = AudioManager.GetAudioController();
        if (AC != null)
        {
            AC.PlaySFX("Transition");
        }

        while (currentTime < maxTime)
        {
            yield return null;

            size.x = Mathf.Lerp(maxWidth, minWidth, currentTime / maxTime);
            rectTr.sizeDelta = size;
            currentTime += Time.deltaTime;
        }

        size.x = minWidth;
        rectTr.sizeDelta = size;
        cutCor = null;
        complete = true;
    }
}
