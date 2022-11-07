using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Object/StageData", order = int.MaxValue)]
public class StageData : ScriptableObject
{
    [SerializeField] [TextArea] private string content;
}
