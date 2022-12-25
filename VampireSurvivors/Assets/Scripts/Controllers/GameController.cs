using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using Nodes;

public class GameController : MonoBehaviour
{
    [SerializeField] private VirtualJoystick joystick;
    public VirtualJoystick Joystick => joystick;

    [ReadOnly] public Character targetPlayer;
    [SerializeField] private float cooltime;


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

        float timer = cooltime;
        joystick.Drag = (force) =>
        {
            timer += Time.deltaTime;
            if (timer > cooltime)
            {
                targetPlayer?.MoveInput(force);
                timer = 0f;
            }
        };
        joystick.PointerUp = (e) =>
        {
            targetPlayer?.MoveInput(Vector2.zero);
            timer = cooltime;
        };
    }


    #region Æó±â

    public Character Player {
        get
        {
            return null;
        }
        set
        {
            
        }
    }
    public int StageLevel => 1;
    public int CurrentEXP
    {
        get => 0;
        set
        {
            
        }
    }

    public NSkillBookInfo SkillBookInfo => null;
    public NSkillTree SkillTree => null;

    public void OnReplay()
    {

    }
    #endregion
}
