using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetManager : MonoBehaviour
{
    private static NetManager instance;
    [SerializeField] private TCPClient_VampireSurvivors client;
    [SerializeField] private TCPServer_VampireSurvivors server;

    public static NetManager Instance => instance;
    public TCPClient_VampireSurvivors Client => client;
    public TCPServer_VampireSurvivors Server => server;

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
