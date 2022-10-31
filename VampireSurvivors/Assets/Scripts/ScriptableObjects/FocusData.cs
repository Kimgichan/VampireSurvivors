using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FocusData", menuName = "Scriptable Object/FocusData", order = int.MaxValue)]
public class FocusData : ScriptableObject
{
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;

    public float MinScale
    {
        get
        {
            if (maxScale < 0f) return 0f;
            if (minScale > maxScale) return maxScale;
            return minScale;
        }
    }
    public float MaxScale
    {
        get
        {
            if (maxScale < 0f) return 0f;
            return maxScale;
        }
    }
    
}
