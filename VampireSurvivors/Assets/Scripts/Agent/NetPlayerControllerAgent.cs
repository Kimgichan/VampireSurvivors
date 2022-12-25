using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


public class NetPlayerControllerAgent : MonoBehaviour
{
    [SerializeField] private NetGameRuler gameRuler;
    [SerializeField] private Character prefab;
    [SerializeField] private List<Character> players;
    [SerializeField] private List<Transform> spawnPosList;
    [SerializeField] private Transform map;

    public int PlayerCount => players.Count;
    public Character GetPlayer(int id) => players[id];

    private void AgentReset()
    {
        for(int i = 0, icount = players.Count; i<icount; i++)
        {
            Destroy(players[i].gameObject);
        }
        players.Clear();
    }

    private void OnEnable()
    {
        Setting();
    }

    // 방 인원 수 만큼 플레이어 생성, 닉네임 할당.
    private void Setting()
    {
        AgentReset();

        if (gameRuler == null) return;
        if (gameRuler.Room == null) return;

        for(int i = 0, icount = gameRuler.Room.RoomSize; i<icount; i++)
        {
            var player = Instantiate(prefab, map);
            player.id = i;
            player.transform.localPosition = spawnPosList[i].localPosition;
            players.Add(player);
        }
    }

    public void GetPlayersInfo(out NetNodes.Server.PlayersInfo info)
    {
        info = new NetNodes.Server.PlayersInfo();
        info.players = new NetNodes.Server.PlayerInfo[players.Count];

        for(int i = 0, icount = players.Count; i<icount; i++)
        {
            var player = players[i];
            if(player == null)
            {
                info.players[i].id = -1;
                continue;
            }

            info.players[i].id = player.id;
            info.players[i].pos = player.transform.localPosition;
            info.players[i].currentHP = player.CurrentHP;
            info.players[i].originalHP = player.OriginalHP;
            info.players[i].name = gameRuler.Room.GetPlayer(i);
        }
    }

    public void SetPlayerMoveInput(in NetNodes.Client.PlayerMoveInput input)
    {
        if (input.id < 0) return;
        players[input.id].MoveInput(input.force);
    }
}
