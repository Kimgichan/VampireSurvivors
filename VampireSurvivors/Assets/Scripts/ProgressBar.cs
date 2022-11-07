using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image progress;
    [SerializeField] private float destValue;
    [SerializeField] private Color defaultColor;

    [SerializeField] private float time;

    private IEnumerator progressCor;

    /// <summary>
    /// 0~1f
    /// </summary>
    public float FillAmount
    {
        get
        {
            return destValue;
        }
        set
        {
            if(destValue != value)
            {
                destValue = value;
                if(progressCor != null)
                {
                    StopCoroutine(progressCor);
                }
                progressCor = ProgressCor();
                StartCoroutine(progressCor);
            }
        }
    }

    private void OnEnable()
    {
        if(progressCor != null)
        {
            StartCoroutine(progressCor);
        }
    }

    /// <summary>
    /// 0~1f
    /// </summary>
    /// <param name="percent"></param>
    public void SetFillAmount(float amount)
    {
        destValue = amount;
        TurnOffProgress();
    }

    private void TurnOffProgress()
    {
        if(progressCor != null)
        {
            StopCoroutine(progressCor);
            progressCor = null;
        }

        progress.color = defaultColor;
        progress.fillAmount = destValue;
    }

    private IEnumerator ProgressCor()
    {
        progress.color = Color.white;
        var dest = destValue - progress.fillAmount;
        var start = progress.fillAmount;
        var _time = 0f;

        while(_time < time)
        {
            yield return null;
            var TSC = GameManager.GetTimeScaleController();
            if (TSC != null) {
                _time += TSC.GameTimeScaleUpdate;
            }

            progress.fillAmount = start + dest * _time / time;
        }

        progress.fillAmount = destValue;

        progress.color = defaultColor;
        progressCor = null;
    }
}
