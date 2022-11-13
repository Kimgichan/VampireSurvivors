using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowTracing : MonoBehaviour
{
    [SerializeField] Bow bow;

    public void OnShot()
    {
        if(bow != null)
        {
            bow.OnShot();
        }
    }
}
