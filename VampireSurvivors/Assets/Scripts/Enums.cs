using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enums
{
    public enum WeaponRating
    {
        Normal,
        Rare,
        Epic,
        Legendary,
    }

    public enum WeaponSlotState
    {
        None,
        /// <summary>
        /// 무기 슬롯이 꽉차서 더이상 안들어감
        /// </summary>
        Full,

        /// <summary>
        /// 왼쪽 무기 슬롯에 들어감
        /// </summary>
        Left,

        /// <summary>
        /// 오른쪽 무기 슬롯에 들어감
        /// </summary>
        Right,

        /// <summary>
        /// 무기 슬롯에 들어있는 무기의 레벨이 Max일 경우 
        /// </summary>
        MaxLevel,
    }

    public enum Creature
    {
        Character,
        Monster,
    }
}
