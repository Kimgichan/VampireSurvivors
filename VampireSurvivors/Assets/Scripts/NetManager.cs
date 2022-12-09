using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetManager : MonoBehaviour
{
    private static NetManager instance;
    [SerializeField] private TCPClient_VampireSurvivors client;

    public TCPClient_VampireSurvivors Client => client;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
