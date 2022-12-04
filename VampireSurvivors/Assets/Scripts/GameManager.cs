using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Nodes;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [SerializeField] private List<GameObject> notMultipleObjs;
    public GameController gameController;
    public TimeScaleController timeScaleController;
    public MonsterController monsterController;
    public FocusController focusController;
    public DamageTextController damageTextController;
    public UIController uiController;
    public LobbyController lobbyController;
    public SkillController skillController;

    [SerializeField] private List<int> stageLevels;
    public int selectStage;

    [SerializeField] private int frameSearchCount;

    [SerializeField] private WeaponDatabase weaponDatabase;

    [SerializeField] private NInventory inventory;
    

    public static GameManager Instance => instance;

    public int FrameSearchCount => frameSearchCount;
    public WeaponDatabase WeaponDatabase => weaponDatabase;

    public NInventory Inventory => inventory;

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

        foreach(var obj in notMultipleObjs)
        {
            obj.SetActive(true);
        }

        Application.targetFrameRate = 60;

        InventorySetting();
    }

    private void InventorySetting()
    {
        inventory.Init();

        inventory.Gold = 0;
        inventory.Crystal = 0;
        inventory.Dust = 0;
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

    public static DamageTextController GetDamageTextController()
    {
        if(Instance == null || Instance.damageTextController == null)
        {
            return null;
        }
        return Instance.damageTextController;
    }

    public static UIController GetUIController()
    {
        if(Instance == null || Instance.uiController == null)
        {
            return null;
        }
        return Instance.uiController;
    }

    public static LobbyController GetLobbyController()
    {
        if(Instance == null || Instance.lobbyController == null)
        {
            return null;
        }
        return Instance.lobbyController;
    }

    public static GameController GetGameController()
    {
        if (Instance == null || Instance.gameController == null)
        {
            return null;
        }
        return Instance.gameController;
    }

    public static SkillController GetSkillController()
    {
        if(Instance == null || Instance.skillController == null)
        {
            return null;
        }
        return Instance.skillController;
    }

    public static MonsterController GetMonsterController()
    {
        if(Instance == null || Instance.monsterController == null)
        {
            return null;
        }
        return Instance.monsterController;
    }

    public static WeaponData GetWeaponData(string name)
    {
        if(Instance != null)
        {
            Instance.weaponDatabase.GetWeaponData(name);
        }

        return null;
    }

    public void GMReset()
    {
        gameController = null;
        timeScaleController = null;
        monsterController = null;
        focusController = null;
        damageTextController = null;
        uiController = null;
        lobbyController = null;
    }
}
