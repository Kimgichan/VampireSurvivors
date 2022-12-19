using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameControllerÀÇ ¿ªÇÒ
/// </summary>
public class NetGameAgent : MonoBehaviour
{
    [SerializeField] private NetGameRuler gameRuler;
    [SerializeField] private int stageLevel;
    [SerializeField] private int requireEXP;
    [SerializeField] private int currentEXP;

    public int StageLevel => stageLevel;
    public int RequireEXP => requireEXP;
    public int CurrentEXP
    {
        get => currentEXP;
        set
        {
            if (value < 0) currentEXP = 0;
            else currentEXP = value;
        }
    }

    public void LevelUP()
    {

    }

    private IEnumerator RecessCor()
    {
        yield return null;
    }
}
