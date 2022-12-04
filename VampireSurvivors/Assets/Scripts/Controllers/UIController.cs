using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldTxt;

    [SerializeField] private ProgressBar hpBar;
    [SerializeField] private ProgressBar expBar;

    [SerializeField] private SkillBook skillBook;


    public SkillBook SkillBook => skillBook;

    /// <summary>
    /// 0~1f
    /// </summary>
    public float HP_Percent
    {
        set
        {
            if (hpBar != null)
            {
                if (value > 1f) value = 1f;
                hpBar.FillAmount = Mathf.Ceil(value * 100f) / 100f;
            }
        }
    }

    public float EXP_Percent
    {
        set
        {
            if (expBar != null)
            {
                if (value > 1f) value = 1f;
                expBar.FillAmount = Mathf.Ceil(value * 100f) / 100f;
            }
        }
    }

    public BigInteger Gold
    {
        set
        {
            if(value < 0)
            {
                goldTxt.text = "0 G";
            }
            goldTxt.text = $"{string.Format("{0:#,0}", value.ToString())} G";
        }
    }

    private IEnumerator Start()
    {
        while (GameManager.Instance == null)
            yield return null;

        if(GameManager.Instance.uiController != null)
        {
            Destroy(gameObject);
            yield break;
        }

        GameManager.Instance.uiController = this;

        if(GameManager.Instance.Inventory != null)
        {
            Gold = GameManager.Instance.Inventory.Gold;
        }
    }

    public void SetHP_Percent(float percent)
    {
        if (hpBar != null)
        {
            if (percent > 1f) percent = 1f;
            hpBar.SetFillAmount(Mathf.Ceil(percent * 100f) / 100f);
        }
    }
    public void SetEXP_Percent(float percent)
    {
        if (expBar != null)
        {
            if (percent > 1f) percent = 1f;
            expBar.SetFillAmount(Mathf.Ceil(percent * 100f) / 100f);
        }
    }
}
