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
        /// ���� ������ ������ ���̻� �ȵ�
        /// </summary>
        Full,

        /// <summary>
        /// ���� ���� ���Կ� ��
        /// </summary>
        Left,

        /// <summary>
        /// ������ ���� ���Կ� ��
        /// </summary>
        Right,

        /// <summary>
        /// ���� ���Կ� ����ִ� ������ ������ Max�� ��� 
        /// </summary>
        MaxLevel,
    }

    public enum Creature
    {
        Character,
        Monster,
    }
}
