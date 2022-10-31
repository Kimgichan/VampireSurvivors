using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public GameController gameController;
    public SoundController soundController;
    public TimeScaleController timeScaleController;
    public MonsterController monsterController;
    public FocusController focusController;
    public AudioController audioController;
    public DamageTextController damageTextController;

    [SerializeField] private List<int> stageLevels;
    public int selectStage;
    

    public static GameManager Instance => instance;
    

    // Start is called before the first frame update
    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetCurrengStageLevel()
    {
        return stageLevels[selectStage];
    }

    public static FocusController GetFocusController()
    {
        if(Instance == null || Instance.focusController == null)
        {
            return null;
        }
        return Instance.focusController;
    }

    public static TimeScaleController GetTimeScaleController()
    {
        if(Instance == null || Instance.timeScaleController == null)
        {
            return null;
        }
        return Instance.timeScaleController;
    }

    public static AudioController GetAudioController()
    {
        if(Instance == null || Instance.audioController == null)
        {
            return null;
        }
        return Instance.audioController;
    }

    public static DamageTextController GetDamageTextController()
    {
        if(Instance == null || Instance.damageTextController == null)
        {
            return null;
        }
        return Instance.damageTextController;
    }
}
