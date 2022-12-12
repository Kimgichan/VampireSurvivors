using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NetEnums;

using Client = NetNodes.Client;
using Server = NetNodes.Server;

public class TCPClient_VampireSurvivors : TCPClient
{
    public void SendData_Login(Client.Login login)
    {
        SendData($"{(int)Data.Login_Client}/{JsonUtility.ToJson(login)}");
    }
    public void SendData_Logout()
    {
        SendData($"{(int)Data.Logout_Client}/");
    }
    public void SendData_EnterRoom(Client.EnterRoom enterRoom)
    {
        SendData($"{(int)Data.EnterRoom_Client}/{JsonUtility.ToJson(enterRoom)}");
    }
    public void SendData_CancelRoom(Client.CancelRoom cancelRoom)
    {
        SendData($"{(int)Data.CancelRoom_Client}/{JsonUtility.ToJson(cancelRoom)}");
    }

    public void SendData_Chat(Client.Chat chat)
    {
        SendData($"{(int)Data.Chat_Client}/{JsonUtility.ToJson(chat)}");
    }


    protected override void RecvData(string data)
    {
        try
        {
            var split = data.Split('/');

            var type = (Data)int.Parse(split[0]);
            switch (type)
            {
                case Data.Login_Server:
                    {
                        RecvData_Login(JsonUtility.FromJson<Server.Login>(split[1]));
                    }break;
                case Data.Logout_Server:
                    {
                        RecvData_Logout();
                    }break;
                case Data.EnterRoom_Server:
                    {
                        RecvData_EnterRoom(JsonUtility.FromJson<Server.EnterRoom>(split[1]));
                    }
                    break;
                case Data.CancelRoom_Server:
                    {
                        RecvData_CancelRoom(JsonUtility.FromJson<Server.CancelRoom>(split[1]));
                    }
                    break;
                case Data.Chat_Server:
                    {
                        RecvData_Chat(JsonUtility.FromJson<Server.Chat>(split[1]));
                    }
                    break;
            }
        }
        catch
        {

        }
    }

    private void RecvData_Login(Server.Login login)
    {
        if (login.ok)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.player = login.msg;
                if (LoadSceneManager.Instance != null)
                    LoadSceneManager.Instance.LoadLobby();
            }
        }
        else
        {
            var tc = GameManager.GetTitleController();
            if(tc != null)
            {
                tc.Error = login.msg;
                tc.LoginReset();
            }
        }
    }
    private void RecvData_Logout()
    {

    }
    private void RecvData_EnterRoom(Server.EnterRoom enterRoom)
    {
        ClientTester.instance?.RecvEnterRoom(enterRoom);
    }
    private void RecvData_CancelRoom(Server.CancelRoom cancelRoom)
    {
        ClientTester.instance?.RecvCancelRoom(cancelRoom);
    }
    private void RecvData_Chat(Server.Chat chat)
    {
        var LC = GameManager.GetLobbyController();
        if (LC == null) return;

        LC.AddChat(chat.Player, chat.MSG);
    }
}
