using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClientTester : MonoBehaviour
{
    public static ClientTester instance;
    [SerializeField] private TCPClient_VampireSurvivors client;
    [SerializeField] private TextMeshProUGUI enterInfo;
    [SerializeField] private TextMeshProUGUI cancelInfo;

    private void Start()
    {
        instance = this;
    }
    public void SendEnterRoom()
    {
        client.SendData_EnterRoom(new NetNodes.Client.EnterRoom()
        {
            player ="AABB",
            stage = "A_0"
        });
    }
    public void SendCancelRoom()
    {
        client.SendData_CancelRoom(new NetNodes.Client.CancelRoom()
        {
            player = "AABB"
        });
    }

    public void RecvEnterRoom()
    {
        RecvEnterRoom(new NetNodes.Server.EnterRoom() { players = new List<string>() { "AABB"}});
    }
    public void RecvCancelRoom()
    {
        RecvCancelRoom(new NetNodes.Server.CancelRoom() { ok = true });
    }

    public void RecvEnterRoom(NetNodes.Server.EnterRoom enterRoom)
    {
        enterInfo.text = JsonUtility.ToJson(enterRoom, true);
    }
    public void RecvCancelRoom(NetNodes.Server.CancelRoom cancelRoom)
    {
        cancelInfo.text = JsonUtility.ToJson(cancelRoom, true);
    }
}
