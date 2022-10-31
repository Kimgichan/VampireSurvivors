using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    [SerializeField] private Character player;
    [SerializeField] private VirtualJoystick joystick;

    public Character Player => player;


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

        GameManager.Instance.gameController = this;

        joystick.Drag = (dir) =>
        {
            if(Player != null)
            {
                Player.Velocity = dir;
            }
        };
        joystick.PointerUp = (e) =>
        {
            if(Player != null)
            {
                Player.Velocity = Vector2.zero;
            }
        };
    }
}
