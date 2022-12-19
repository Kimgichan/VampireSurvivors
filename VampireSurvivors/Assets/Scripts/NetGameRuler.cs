using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


/// <summary>
/// GameManagerÀÇ ¿ªÇÒ
/// </summary>
public class NetGameRuler : MonoBehaviour
{
    [ReadOnly][SerializeField] private Nodes.Room room;
    [SerializeField] private NetGameAgent gameAgent;
    [SerializeField] private NetMonsterControllerAgent monsterControllerAgent;
    [SerializeField] private float cooltime;
    public Nodes.Room Room => room;
    public NetGameAgent GameAgent => gameAgent;
    public NetMonsterControllerAgent MonsterControllerAgent => monsterControllerAgent;

    private IEnumerator Start()
    {
        while(NetManager.Instance == null || NetManager.Instance.Server == null)
        {
            yield return null;
        }

        while(room == null)
        {
            room = NetManager.Instance.Server.Pop();
            yield return null;
        }

        while (gameAgent == null)
        {
            yield return null;
        }
        gameAgent.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        StartCoroutine(NetCor());
    }

    private IEnumerator NetCor()
    {
        var wait = new WaitForSeconds(cooltime);
        var completePlayers = new Queue<Nodes.VampireSurvivorsClientInfo>();
        var loadingPlayers = new Queue<Nodes.VampireSurvivorsClientInfo>();

        while (true)
        {
            yield return wait;

            if (room == null) continue;
            if (!room.isRun) room.isRun = true;

            for(int i = 0, icount = room.RoomSize; i<icount; i++)
            {
                var player = room.GetPlayer(i);
                var clientInfo = NetManager.Instance.Server.GetClientInfo(player);

                if (clientInfo == null) continue;
                if (clientInfo.client.IsConntected())
                {
                    if (room.GetPercent(i) < 1f)
                    {
                        loadingPlayers.Enqueue(clientInfo);
                    }
                    else
                    {
                        completePlayers.Enqueue(clientInfo);
                    }
                }
                else
                {
                    room.SetPercent(i, 0f);
                }
            }

            if(loadingPlayers.Count > 0)
            {
                var sendData = room.GetReady();
                while(loadingPlayers.Count > 0)
                {
                    var player = loadingPlayers.Dequeue();
                    player.client.SendData_Ready(sendData);
                }
            }

            if(completePlayers.Count > 0)
            {
                var sendData = new NetNodes.Server.GameTick();
                monsterControllerAgent.GetMonstersInfo(out sendData.monstersInfo);

                while(completePlayers.Count > 0)
                {
                    var player = completePlayers.Dequeue();
                    player.client.SendData_GameTick(sendData);
                }
            }
        }
    }
}
