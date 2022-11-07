using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    [SerializeField] private Character player;
    [SerializeField] private VirtualJoystick joystick;

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


    // Start is called before the first frame update
    private IEnumerator Start()
    {
        while (GameManager.Instance == null)
            yield return null;

        if(GameManager.Instance.gameController != null)
        {
            Destroy(gameObject);
            yield break;
        }

        while (GameManager.Instance.timeScaleController == null)
            yield return null;

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

        if(Player != null)
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
}
