using System.Collections;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshPro tmp;
    private IEnumerator showCor;
    private Vector3 pivot;

    public int Damage
    {
        get
        {
            return int.Parse(tmp.text);
        }

        set
        {
            tmp.text = value.ToString();
        }
    }

    public Color TextColor
    {
        get => tmp.color;
        set => tmp.color = value;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void ReturnPopup()
    {
        var DTC = GameManager.GetDamageTextController();
        if(DTC != null)
        {
            DTC.TurnOffPopup();
        }
    }

    public void ShowDamage(float time, Vector3 pos)
    {
        gameObject.SetActive(true);

        TurnOffShow();
        showCor = ShowCor(time);
        pivot = pos;
        transform.position = pivot;
        StartCoroutine(showCor);
    }

    private void TurnOffShow()
    {
        if (showCor != null)
        {
            StopCoroutine(showCor);
            showCor = null;
        }
    }

    private IEnumerator ShowCor(float time)
    {
        var DTC = GameManager.GetDamageTextController();
        var destTime = time * (DTC == null ? 0.3334f * 0.5f : DTC.PercentageOpen_1);
        var currentTime = 0f;
        var destScale = 0.5f;
        while(currentTime < destTime)
        {
            yield return null;

            var TSC = GameManager.GetTimeScaleController();
            if (TSC == null) continue;

            DTC = GameManager.GetDamageTextController();
            if (DTC == null) continue;

            currentTime += TSC.GameTimeScaleUpdate;

            float progress = currentTime / destTime;

            var pos = pivot;
            pos.y += DTC.OpenAxisY * progress;
            transform.position = pos;

            var scale = Vector3.one * destScale * progress * DTC.DestScale;
            scale.z = 1f;
            transform.localScale = scale;
        }

        currentTime = 0f;
        DTC = GameManager.GetDamageTextController();
        destTime = time * (DTC == null ? 0.3334f * 0.5f : DTC.PercentageOpen_2);

        while (currentTime < destTime)
        {
            #region
            yield return null;

            var TSC = GameManager.GetTimeScaleController();
            if (TSC == null) continue;

            DTC = GameManager.GetDamageTextController();
            if (DTC == null) continue;

            currentTime += TSC.GameTimeScaleUpdate;

            float progress = currentTime / destTime;
            #endregion

            var pos = pivot;
            pos.y += DTC.OpenAxisY;
            pos.y += -DTC.OpenAxisY * progress;
            transform.position = pos;

            var scale = Vector3.one * destScale * DTC.DestScale * (progress * 0.5f + 0.5f);
            scale.z = 1f;
            transform.localScale = scale;
        }

        currentTime = 0f;
        DTC = GameManager.GetDamageTextController();
        destTime = time * (DTC == null ? 0.3333f : DTC.PercentageStay);
        while (currentTime < destTime)
        {
            yield return null;

            var TSC = GameManager.GetTimeScaleController();
            if (TSC == null) continue;

            currentTime += TSC.GameTimeScaleUpdate;
        }

        currentTime = 0f;
        DTC = GameManager.GetDamageTextController();
        destTime = time * (DTC == null ? 0.3333f : DTC.PercentageClose);
        while (currentTime < destTime)
        {
            #region
            yield return null;

            var TSC = GameManager.GetTimeScaleController();
            if (TSC == null) continue;

            DTC = GameManager.GetDamageTextController();
            if (DTC == null) continue;

            currentTime += TSC.GameTimeScaleUpdate;

            float progress = currentTime / destTime;
            #endregion

            var pos = pivot;
            pos.y += DTC.CloseAxisY * progress;
            transform.position = pos;

            var scale = Vector3.one * DTC.DestScale * (1 - progress);
            scale.z = 1f;
            transform.localScale = scale;
        }

        showCor = null;
        ReturnPopup();
    }
}
