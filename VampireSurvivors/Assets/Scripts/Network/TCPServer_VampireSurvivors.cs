using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nodes;
using NetEnums;

using Client = NetNodes.Client;
using Server = NetNodes.Server;


public class TCPServer_VampireSurvivors : TCPServer
{
    private Dictionary<string, VampireSurvivorsClientInfo> clientTable;

    /// <summary>
    /// 자료구조의 Queue 아님
    /// </summary>
    private Dictionary<string, LinkedList<Room>> queueRooms;

    protected override void Init()
    {
        clientTable = new Dictionary<string, VampireSurvivorsClientInfo>();
        queueRooms = new Dictionary<string, LinkedList<Room>>();
    }
    protected override ServerClient CreateServerClient()
    {
        return new ServerClient_VampireSurvivors();
    }

    public void Login(ServerClient_VampireSurvivors client)
    {
        if (!clientTable.ContainsKey(client.Player))
        {
            clientTable.Add(client.Player, new VampireSurvivorsClientInfo() { client = client, room = null });

            client.SendData_Login(new Server.Login() { ok = true, msg = client.Player });

            return;
        }

        var info = clientTable[client.Player];
        if(info.client == null)
        {
            info.client = client;
            client.SendData_Login(new Server.Login() { ok = true, msg = client.Player });
        }
        else if (info.client.IsConntected())
        {
            client.SendData_Login(new Server.Login() { ok = false, msg = "이미 사용중인 닉네임입니다." });
        }
        else
        {
            info.client.Close();
            info.client = client;
            client.SendData_Login(new Server.Login() { ok = true, msg = client.Player });
        }
    }
    public void Logout(ServerClient_VampireSurvivors client)
    {
        if (client.Player == null || client.Player == "")
        {
            client.SendData_Logout();
            return;
        }

        RemovePlayer(client);
        clientTable.Remove(client.Player);

        client.SendData_Logout();
    }

    private void RemovePlayer(ServerClient_VampireSurvivors client)
    {
        if (clientTable.TryGetValue(client.Player, out VampireSurvivorsClientInfo info))
        {
            if (info.room != null)
            {
                info.room.RemovePlayer(client.Player);

                if (info.room.Count <= 0)
                {
                    if(queueRooms.TryGetValue(info.room.stage, out LinkedList<Room> rooms))
                    {
                        for(var pivot = rooms.First; pivot != null; pivot = pivot.Next)
                        {
                            if(pivot.Value == info.room)
                            {
                                rooms.Remove(pivot);
                                break;
                            }
                        }
                    }
                }

                info.room = null;
            }
        }
    }

    public void Chat(Client.Chat chat)
    {
        if (chat.Player == "") return;

        var echo = new Server.Chat(chat.Player, chat.MSG);
        foreach(var client in clientTable)
        {
            client.Value.client.SendData_Chat(echo);
        }
    }
}

public class ServerClient_VampireSurvivors : ServerClient
{
    private TCPServer_VampireSurvivors server;
    private string player;

    public string Player => player;

    protected override void Setting(TCPServer server)
    {
        var _server = server as TCPServer_VampireSurvivors;
        if (_server == null)
        {
            Close();
            return;
        }

        this.server = _server; 
    }

    protected override void Reset()
    {
        if (server == null) return;

        server.Logout(this);
    }

    #region Send
    public void SendData_Login(Server.Login login)
    {
        SendData($"{(int)Data.Login_Server}/{JsonUtility.ToJson(login)}");
    }
    public void SendData_Logout()
    {
        SendData($"{(int)Data.Logout_Server}/");
    }

    public void SendData_EnterRoom(Server.EnterRoom enterRoom)
    {
        SendData($"{(int)Data.EnterRoom_Server}/{JsonUtility.ToJson(enterRoom)}");
    }
    public void SendData_CancelRoom(Server.CancelRoom cancelRoom)
    {
        SendData($"{(int)Data.CancelRoom_Server}/{JsonUtility.ToJson(cancelRoom)}");
    }
    public void SendData_Chat(Server.Chat chat)
    {
        SendData($"{(int)Data.Chat_Server}/{JsonUtility.ToJson(chat)}");
    }
    #endregion

    #region Recv
    protected override void RecvData(string data)
    {
        if (server == null)
        {
            return;
        }

        try
        {
            var split = data.Split('/');

            var type = (Data)int.Parse(split[0]);
            switch (type)
            {
                case Data.Login_Client:
                    {
                        RecvData_Login(JsonUtility.FromJson<Client.Login>(split[1]));
                    }break;
                case Data.Logout_Client:
                    {
                        RecvData_Logout();
                    }break;
                case Data.EnterRoom_Client: 
                    {
                        RecvData_EnterRoom(JsonUtility.FromJson<Client.EnterRoom>(split[1]));
                    }break;
                case Data.CancelRoom_Client:
                    {
                        RecvData_CancelRoom(JsonUtility.FromJson<Client.CancelRoom>(split[1]));
                    }break;
                case Data.Chat_Client:
                    {
                        RecvData_Chat(JsonUtility.FromJson<Client.Chat>(split[1]));
                    }break;
            }
        }
        catch
        {

        }
    }
    private void RecvData_Login(Client.Login login)
    {
        player = login.player;
        server.Login(this);
    }
    private void RecvData_Logout()
    {
        server.Logout(this);
    }
    private void RecvData_EnterRoom(Client.EnterRoom enterRoom)
    {

    }
    private void RecvData_CancelRoom(Client.CancelRoom cancelRoom)
    {

    }
    private void RecvData_Chat(Client.Chat chat)
    {
        server.Chat(chat);
    }
    #endregion
}