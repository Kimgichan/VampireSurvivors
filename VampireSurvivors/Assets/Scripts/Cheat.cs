using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Numerics;


public class Cheat : MonoBehaviour
{
    [Button("°ñµå Àû¿ë")]
    private void GoldUpdate()
    {
        if (GameManager.Instance != null) {
            GameManager.Instance.Inventory.Gold = BigInteger.Parse(gold);
        }
    }
    [SerializeField] private string gold;
}
