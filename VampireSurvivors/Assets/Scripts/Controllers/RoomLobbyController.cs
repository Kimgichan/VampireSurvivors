using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLobbyController : MonoBehaviour
{
    [SerializeField] private PlayReadySlot prefab;
    [SerializeField] private Transform content;
    [SerializeField] private List<PlayReadySlot> players;
    private IEnumerator Start()
    {
        while (GameManager.Instance == null)
            yield return null;

        if(GameManager.Instance.roomLobbyController != null)
        {
            Destroy(gameObject);
            yield break;
        }

        GameManager.Instance.roomLobbyController = this;
    }

    public void SetReady(in NetNodes.Server.Ready ready)
    {
        var gap = ready.players.Count - players.Count;
        if(gap > 0)
        {
            for(int i = 0, icount = gap; i<icount; i++)
            {
                var newSlot = Instantiate(prefab, content);
                players.Add(newSlot);
            }
        }
        else if(gap < 0)
        {
            for(int i = ready.players.Count, icount = players.Count; i<icount; i++)
            {
                players[i].gameObject.SetActive(false);
            }
        }

        for(int i = 0, icount = ready.players.Count; i<icount; i++)
        {
            players[i].gameObject.SetActive(true);
            players[i].Player = ready.players[i];
            players[i].Percent = ready.percents[i];
        }
    }
}
