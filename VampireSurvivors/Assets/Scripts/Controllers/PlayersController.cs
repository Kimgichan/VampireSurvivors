using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PlayersController : MonoBehaviour
{
    [SerializeField] private Character prefab;
    [ReadOnly] [SerializeField] private List<Character> players;
    [SerializeField] private Transform map;
    [ReadOnly] [SerializeField] private NetNodes.Server.PlayersInfo currentInfo;
    [SerializeField] private float lerpForce;

    private IEnumerator Start()
    {
        while (GameManager.Instance == null)
            yield return null;

        if(GameManager.Instance.playersController != null)
        {
            Destroy(gameObject);
            yield break;
        }

        GameManager.Instance.playersController = this;
    }

    public void SetPlayersInfo(in NetNodes.Server.PlayersInfo info)
    {
        currentInfo = info;

        if(players.Count <= 0)
        {
            Init(info);
        }
        
        for(int i = 0, icount = info.players.Length; i<icount; i++)
        {
            players[i].CurrentHP = info.players[i].currentHP;
            players[i].OriginalHP = info.players[i].originalHP;
        }
    }

    private void FixedUpdate()
    {
        for(int i = 0, icount = players.Count; i<icount; i++)
        {
            Vector3 pos = Vector2.Lerp(players[i].transform.localPosition, currentInfo.players[i].pos, lerpForce);
            pos.z = pos.y * 0.05f;
            players[i].transform.localPosition = pos;
        }
    }

    private void Init(in NetNodes.Server.PlayersInfo info)
    {
        for(int i = 0, icount = info.players.Length; i<icount; i++)
        {
            var player = Instantiate(prefab, map);
            players.Add(player);
            player.id = i;

            players[i].transform.localPosition = info.players[i].pos;
            if (GameManager.Instance.player != "" && GameManager.Instance.player == info.players[i].name)
            {
                var GC = GameManager.GetGameController();
                if(GC != null)
                {
                    GC.targetPlayer = player;
                }
            }
        }
    }
}
