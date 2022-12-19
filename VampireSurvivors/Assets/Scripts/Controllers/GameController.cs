using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Nodes;

public class GameController : MonoBehaviour
{
    [SerializeField] private Character player;
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private int stageLevel;
    [SerializeField] private int requireEXP;
    [SerializeField] private int currentEXP;
    [SerializeField] private NSkillTree skillTree;
    [SerializeField] private NSkillBookInfo skillBookInfo;

    public Character Player {
        get
        {
            return player;
        }
        set
        {
            if (player == value) return;

            player = value;
            player.Active();
        }
    }
    public int StageLevel => stageLevel;
    public int CurrentEXP
    {
        get => currentEXP;
        set
        {
            if (value < 0)
            {
                currentEXP = 0;
            }
            else currentEXP = value;

            Reward();
        }
    }
    public int RequireEXP => requireEXP;

    public NSkillBookInfo SkillBookInfo => skillBookInfo;
    public NSkillTree SkillTree => skillTree;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        while(NetManager.Instance == null)
        {
            yield return null;
        }

        while (NetManager.Instance.Server == null && NetManager.Instance.Client == null)
            yield return null;

        if (NetManager.Instance.Server != null)
        {
            var serverInitCor = ServerInitCor();
            while (serverInitCor.MoveNext())
            {
                yield return serverInitCor.Current;
            }
        }
        else
        {
            var clientInitCor = ClientInitCor();
            while (clientInitCor.MoveNext())
            {
                yield return clientInitCor.Current;
            }
        }
    }

    private IEnumerator ServerInitCor()
    {
        yield return null;
    }
    private IEnumerator ClientInitCor()
    {
        while (GameManager.Instance == null)
            yield return null;

        if (GameManager.Instance.gameController != null)
        {
            Destroy(gameObject);
            yield break;
        }

        stageLevel = 1;
        currentEXP = 0;

        while (GameManager.Instance.timeScaleController == null)
            yield return null;
        while (GameManager.Instance.uiController == null)
            yield return null;
        while (GameManager.Instance.skillController == null)
            yield return null;

        GameManager.Instance.uiController.SetEXP_Percent(0f);

        GameManager.Instance.timeScaleController.gameTimeScale = 0f;

        GameManager.Instance.gameController = this;

        joystick.Drag = (force) =>
        {
            if (Player != null)
            {
                //Player.Velocity = force;
                Player.Move(force);
            }
        };
        joystick.PointerUp = (e) =>
        {
            if (Player != null)
            {
                //Player.Velocity = Vector2.zero;
                Player.Move(Vector2.zero);
            }
        };

        if (Player != null)
        {
            Player.Active();
        }

        yield return new WaitForSeconds(1f);
        GameManager.Instance.timeScaleController.gameTimeScale = 1f;
    }


    public void GameEnd()
    {
        var TSC = GameManager.GetTimeScaleController();
        if(TSC != null)
        {
            TSC.gameTimeScale = 0f;
        }

        if(LoadSceneManager.Instance != null)
        {
            LoadSceneManager.Instance.LoadLobby();
        }
    }

    private void Reward()
    {
        if(currentEXP >= RequireEXP)
        {
            currentEXP = 0;
            var UC = GameManager.GetUIController();
            if (UC != null)
            {
                UC.SetEXP_Percent(0f);
            }
            LevelUP();
        }
        else
        {
            var UC = GameManager.GetUIController();
            if(UC != null)
            {
                UC.EXP_Percent = (float)CurrentEXP / RequireEXP;
            }
        }
    }

    private void LevelUP()
    {
        stageLevel += 1;

        OnStop();

        var UC = GameManager.GetUIController();
        if(UC != null && UC.SkillBook != null)
        {
            UC.SkillBook.gameObject.SetActive(true);
            UC.SkillBook.Active();
        }  
    }

    public void OnStop()
    {
        var TSC = GameManager.GetTimeScaleController();
        if (TSC != null)
        {
            TSC.gameTimeScale = 0f;
        }
    }
    public void OnReplay()
    {
        var TSC = GameManager.GetTimeScaleController();
        if (TSC != null)
        {
            TSC.gameTimeScale = 1f;
        }
    }
}
