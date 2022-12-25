using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NetEnums;

using Client = NetNodes.Client;
using Server = NetNodes.Server;

public class TCPClient_VampireSurvivors : TCPClient
{
    private void OnApplicationQuit()
    {
        Close();
    }

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
    public void SendData_PlayerMoveInput(Client.PlayerMoveInput input)
    {
        SendData($"{(int)Data.PlayerMoveInput_Client}/{JsonUtility.ToJson(input)}");
    }

    public void SendData_Ready(Client.Ready ready)
    {
        SendData($"{(int)Data.Ready_Client}/{JsonUtility.ToJson(ready)}");
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
                        RecvData_CancelRoom();
                    }
                    break;
                case Data.Chat_Server:
                    {
                        RecvData_Chat(JsonUtility.FromJson<Server.Chat>(split[1]));
                    }
                    break;
                case Data.Ready_Server:
                    {
                        RecvData_Ready(JsonUtility.FromJson<Server.Ready>(split[1]));
                    }
                    break;
                case Data.GameTick_Server:
                    {
                        RecvData_GameTick(JsonUtility.FromJson<Server.GameTick>(split[1]));
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
        var LC = GameManager.GetLobbyController();
        if(LC != null)
        {
            LC.EnterRoom(enterRoom);
        }
    }
    private void RecvData_CancelRoom()
    {
        var LC = GameManager.GetLobbyController();
        if(LC != null)
        {
            LC.CancelRoom();
        }
    }
    private void RecvData_Chat(Server.Chat chat)
    {
        var LC = GameManager.GetLobbyController();
        if (LC == null) return;

        LC.AddChat(chat.Player, chat.MSG);
    }
    private void RecvData_Ready(Server.Ready ready)
    {
        var LC = GameManager.GetLobbyController();
        if(LC != null)
        {
            if (GameManager.Instance.player != "" && LoadSceneManager.Instance != null)
            {
                LoadSceneManager.Instance.LoadRoomLobby();
            }
        }

        var RLC = GameManager.GetRoomLobbyController();
        if(RLC != null)
        {
            RLC.SetReady(ready);
        }
    }
    private void RecvData_GameTick(Server.GameTick gameTick)
    {
        if(LoadSceneManager.Instance != null)
        {
            LoadSceneManager.Instance.OpenStage();
        }

        var MC = GameManager.GetMonsterController();
        if(MC != null)
        {
            MC.SetMonstersInfo(gameTick.monstersInfo);
        }

        var PC = GameManager.GetPlayersController();
        if(PC != null)
        {
            PC.SetPlayersInfo(gameTick.playersInfo);
        }
    }
}
