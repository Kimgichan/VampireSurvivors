using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Client = NetNodes.Client;
using Server = NetNodes.Server;

namespace Enums
{
    public enum WeaponRating
    {
        Normal,
        Rare,
        Epic,
        Legendary,
    }

    public enum Creature
    {
        Character,
        Monster,
    }
}

namespace NetEnums
{
    public enum Data
    {
        #region Client
        Login_Client,
        Logout_Client,
        EnterRoom_Client,
        CancelRoom_Client,
        #endregion

        #region Server
        Login_Server,
        Logout_Server,
        EnterRoom_Server,
        CancelRoom_Server,
        #endregion
    }

    public enum RoomEnterResult 
    {
        Success,
        Full,
        Multiple,
    }
}
