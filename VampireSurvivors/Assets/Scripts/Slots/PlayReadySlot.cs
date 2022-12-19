using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayReadySlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI player;
    [SerializeField] private Slider percent;
    [SerializeField] private float minPercent;
    [SerializeField] private float maxPercent;

    public string Player
    {
        set
        {
            if (player != null)
                player.text = value;
        }
    }

    /// <summary>
    /// range : 0~1f
    /// </summary>
    public float Percent
    {
        set
        {
            percent.value = Mathf.Lerp(minPercent, maxPercent, value);
        }
    }
}
