using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBox : MonoBehaviour
{
    [SerializeField] private float time;
    /// <summary>
    /// 0~1f
    /// </summary>
    [SerializeField] private float maxTimePercent;
    /// <summary>
    /// 0~1f
    /// </summary>
    [SerializeField] private float destTimePercent;

    [Space]
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;
    [SerializeField] private float destScale;

    private IEnumerator bounceCor;

    private void OnEnable()
    {
        if(bounceCor != null)
        {
            StartCoroutine(bounceCor);
        }
    }

    public void TurnOnBounce()
    {
        if(bounceCor != null)
        {
            StopCoroutine(bounceCor);
        }

        bounceCor = BounceCor();
        StartCoroutine(bounceCor);
    }

    public void TurnOffBounce()
    {
        if(bounceCor != null)
        {
            StopCoroutine(bounceCor);
            bounceCor = null;
        }
    }

    public void TurnOnInverseBounce()
    {
        if (bounceCor != null)
        {
            StopCoroutine(bounceCor);
        }

        bounceCor = InverseBounceCor();
        StartCoroutine(bounceCor);
    }

    private IEnumerator BounceCor()
    {
        yield return null;

        var _maxTime = time * maxTimePercent / (maxTimePercent + destTimePercent);
        var _time = 0f;
        while(_time <= _maxTime)
        {
            transform.localScale = Vector3.Lerp(Vector3.one * minScale, Vector3.one * maxScale, _time / _maxTime);


            yield return null;

            var TSC = GameManager.GetTimeScaleController();
            if(TSC != null)
            {
                _time += TSC.UITimeScaleUpdate;
            }
            else
            {
                _time += Time.deltaTime;
            }
        }
        transform.localScale = Vector3.one * maxScale;
        yield return null;

        _maxTime = time * destTimePercent / (maxTimePercent + destTimePercent);
        _time = 0f;
        while(_time <= _maxTime)
        {
            transform.localScale = Vector3.Lerp(Vector3.one * maxScale, Vector3.one * destScale, _time / _maxTime);

            yield return null;

            var TSC = GameManager.GetTimeScaleController();
            if (TSC != null)
            {
                _time += TSC.UITimeScaleUpdate;
            }
            else
            {
                _time += Time.deltaTime;
            }
        }
        transform.localScale = Vector3.one * destScale;

        bounceCor = null;
    }
    private IEnumerator InverseBounceCor()
    {
        yield return null;

        var _maxTime = time * destTimePercent / (maxTimePercent + destTimePercent);
        var _time = 0f;
        while (_time <= _maxTime)
        {
            transform.localScale = Vector3.Lerp(Vector3.one * destScale, Vector3.one * maxScale, _time / _maxTime);

            yield return null;

            var TSC = GameManager.GetTimeScaleController();
            if (TSC != null)
            {
                _time += TSC.UITimeScaleUpdate;
            }
            else
            {
                _time += Time.deltaTime;
            }
        }
        transform.localScale = Vector3.one * maxScale;

        yield return null;


        _maxTime = time * maxTimePercent / (maxTimePercent + destTimePercent);
        _time = 0f;
        while (_time <= _maxTime)
        {
            transform.localScale = Vector3.Lerp(Vector3.one * maxScale, Vector3.one * minScale, _time / _maxTime);


            yield return null;

            var TSC = GameManager.GetTimeScaleController();
            if (TSC != null)
            {
                _time += TSC.UITimeScaleUpdate;
            }
            else
            {
                _time += Time.deltaTime;
            }
        }
        transform.localScale = Vector3.one * minScale;



        bounceCor = null;
    }
}
