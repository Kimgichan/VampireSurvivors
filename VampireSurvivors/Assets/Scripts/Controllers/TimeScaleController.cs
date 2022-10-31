using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
    public float gameTimeScale;
    public float uiTimeScale;

    public float GameTimeScaleUpdate => Time.deltaTime * gameTimeScale;
    public float GameTimeScaleFixed => Time.fixedDeltaTime * gameTimeScale;
    public float UITimeScaleUpdate => Time.deltaTime * uiTimeScale;
    public float UITimeScaleFixed => Time.fixedDeltaTime * uiTimeScale;


    private IEnumerator Start()
    {
        while (GameManager.Instance == null)
            yield return null;

        if (GameManager.Instance.timeScaleController != null)
        {
            Destroy(gameObject);
            yield break;
        }

        GameManager.Instance.timeScaleController = this;
    }
}
