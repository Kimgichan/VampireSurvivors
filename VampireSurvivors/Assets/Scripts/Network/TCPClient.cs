using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System;
using UnityEngine;
using TMPro;
using NetEnums;

using Client = NetNodes.Client;
using Server = NetNodes.Server;


public class TCPClient : MonoBehaviour
{
    [SerializeField] private string hostIP;
    [SerializeField] private int port;

    [SerializeField] private float connectTimeout;

    private Thread thread;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    private IEnumerator connectTimerCor;
    private bool isConnect = false;

    public bool IsConnect => isConnect;

    void Start()
    {
        ConnectToServer();
    }
    private void OnDestroy()
    {
        Close();
    }

    private void Update()
    {
        if(connectTimerCor != null && thread == null)
        {
            StopCoroutine(connectTimerCor);
            connectTimerCor = null;
        }

        if (!isConnect) return;

        if (!IsConntected())
        {
            Close();
        }

        if (stream.DataAvailable)
        {
            try
            {
                string data = reader.ReadLine();
                if (data != null)
                {
                    RecvData(data);
                }
            }
            catch
            {

            }
        }
    }


    public void ConnectToServer()
    {
        Close();

        connectTimerCor = ConnectTimerCor();
        StartCoroutine(connectTimerCor);
        thread = new Thread(ConnectThread);
        thread.Start();
    }

    private void ConnectThread()
    {
        try
        {
            socket = new TcpClient(hostIP, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            isConnect = true;
            thread = null;
        }
        catch
        {
            isConnect = false;
            thread = null;
        }
    }

    protected void SendData(string data)
    {
        //if (connectCor == null) return;

        //writer.WriteLine(data);
        //writer.Flush();
        if (IsConnect)
        {
            try
            {
                writer.WriteLine(data);
            }
            catch
            {
                Debug.Log($"{data} 전송 실패");
            }
        }
    }


    protected virtual void RecvData(string data)
    {
    }


    private void Close()
    {
        if (thread != null)
        {
            thread.Abort();
            thread = null;
        }

        if (stream != null)
        {
            stream.Close();
            stream = null;
        }

        if (writer != null)
        {
            writer.Close();
            writer = null;
        }

        if (reader != null)
        {
            reader.Close();
            reader = null;
        }

        if (socket != null)
        {
            socket.Close();
            socket = null;
        }

        isConnect = false;

        if(connectTimerCor != null)
        {
            StopCoroutine(connectTimerCor);
            connectTimerCor = null;
        }
    }

    private bool IsConntected()
    {
        try
        {
            if (socket != null && socket.Client != null && socket.Client.Connected)
            {
                if (socket.Client.Poll(0, SelectMode.SelectRead))
                    return !(socket.Client.Receive(new byte[1], SocketFlags.Peek) == 0);

                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    private IEnumerator ConnectTimerCor()
    {
        var wait = new WaitForSeconds(connectTimeout);
        yield return wait;

        if (!IsConnect)
        {
            Close();
        }
    }
}
