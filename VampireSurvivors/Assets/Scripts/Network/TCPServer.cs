using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System;
using UnityEngine;
using NaughtyAttributes;

public class TCPServer : MonoBehaviour
{
    [SerializeField] private string IP;
    [SerializeField] private int port;
    [SerializeField] private float cooltime;

    [SerializeField] private int capacity;
    [ReadOnly] [SerializeField] private int waitClientCount;
    [ReadOnly] [SerializeField] private int connectClientCount;
    List<ServerClient> waitClients;
    List<ServerClient> connectClients;

    TcpListener server;
    private IEnumerator networkCor;

    void Start()
    {
        Init();
        waitClients = new List<ServerClient>(capacity);
        connectClients = new List<ServerClient>(capacity);

        for(int i = 0; i<capacity; i++)
        {
            waitClients.Add(CreateServerClient());
            connectClients.Add(null);
        }

        waitClientCount = capacity;
        connectClientCount = 0;

        try
        {
            server = new TcpListener(IPAddress.Parse(IP), port);
            server.Start();
            StartListening();

            networkCor = NetworkCor();
            StartCoroutine(networkCor);
        }
        catch
        {

        }
    }

    private void OnEnable()
    {
        if(networkCor != null)
        {
            StartCoroutine(networkCor);
        }
    }
    protected virtual void Init() { }
    protected virtual ServerClient CreateServerClient()
    {
        return new ServerClient();
    }
    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    private void AcceptTcpClient(IAsyncResult ar)
    {
        if(waitClientCount > 0)
        {
            var client = waitClients[--waitClientCount];
            waitClients[waitClientCount] = null;

            client.Open(ar, this);
            connectClients[connectClientCount++] = client;
        }

        StartListening();
    }

    private IEnumerator NetworkCor()
    {
        var wait = new WaitForSeconds(cooltime);
        while (true)
        {
            yield return wait;

            for(int i = 0; i<connectClientCount; i++)
            {
                var client = connectClients[i];

                if (!client.IsConntected())
                {
                    client.Close();

                    connectClients[i] = connectClients[connectClientCount - 1];
                    connectClients[--connectClientCount] = null;
                    waitClients[waitClientCount++] = client;
                    i -= 1;
                    continue;
                }

                client.Active();
            }
        }
    }
}

public class ServerClient 
{
    private TcpClient tcp;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    public void Open(IAsyncResult ar, TCPServer server)
    {
        Close();

        TcpListener listener = (TcpListener)ar.AsyncState;
        tcp = listener.EndAcceptTcpClient(ar);

        stream = tcp.GetStream();
        writer = new StreamWriter(stream);
        reader = new StreamReader(stream);

        Setting(server);
    }

    protected virtual void Setting(TCPServer server) { } 
    public bool IsConntected()
    {
        try
        {
            if (tcp != null && tcp.Client != null && tcp.Client.Connected)
            {
                if (tcp.Client.Poll(0, SelectMode.SelectRead))
                    return !(tcp.Client.Receive(new byte[1], SocketFlags.Peek) == 0);

                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public void Close()
    {
        if(tcp != null)
        {
            tcp.Close();
            tcp = null;
        }
        if(stream != null)
        {
            stream.Close();
            stream = null;
        }
        if(writer != null)
        {
            writer.Close();
            writer = null;
        }
        if(reader != null)
        {
            reader.Close();
            reader = null;
        }

        Reset();
    }
    protected virtual void Reset() { }

    public void Active()
    {
        if(stream != null && stream.DataAvailable)
        {
            string data = reader.ReadLine();
            RecvData(data);
        }
    }

    protected virtual void RecvData(string data) {}
    public void SendData(string data)
    {
        if(writer != null)
        {
            writer.WriteLine(data);
            writer.Flush();
        }
    }
}
