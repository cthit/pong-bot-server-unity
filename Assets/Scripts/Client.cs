﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using UnityEngine;

public class Client : PongActor
{

    const float NETWORK_SEND_DELAY = 0.05f;

    [System.Serializable]
    public class ClientInfo
    {
        public string name;
    }

    private int id;
    public ClientInfo info;
    public TcpClient tcpClient;
    private Server server;
    private StreamReader reader;
    private StreamWriter writer;

    private Paddle.Direction latestDecision = Paddle.Direction.None;
    private float latestNetworkSend = 0.0f;

    private Queue<string> receiveQueue = new Queue<string>();

    private Thread messageReader;

    public int ID
    {
        get
        {
            return id;
        }
    }

    public bool IsConnected
    {
        get
        {
            return tcpClient != null;
        }
    }

    public Client(int id, TcpClient tcpClient, Server server)
    {
        this.id = id;
        this.info = null;
        this.tcpClient = tcpClient;
        this.server = server;
        this.reader = new StreamReader(tcpClient.GetStream());
        this.writer = new StreamWriter(tcpClient.GetStream());
        this.latestNetworkSend = Time.fixedTime;

        // Always write instantly to socket on writer.Write()
        this.writer.AutoFlush = true;

        this.messageReader = new Thread(MessageReader);
        messageReader.Start();
    }

    private void MessageReader()
    {
        try
        {
            while (true)
            {
                string message = reader.ReadLine();
                lock (receiveQueue)
                {
                    receiveQueue.Enqueue(message);
                }
            }
        }
        catch (IOException e)
        {
            Debug.LogError(e);
        }
    }

    public Paddle.Direction MakeDecision(Arena.State state)
    {
        if (IsConnected)
        {
            try
            {
                if (Time.fixedTime - latestNetworkSend >= NETWORK_SEND_DELAY)
                {
                    writer.WriteLine(string.Format("{{\"event\":\"game_state\",\"game_state\":{0}}}",
                        state.ToJson()));
                    latestNetworkSend = Time.fixedTime;
                }

                string message = ReadMessage();

                if (message != null)
                {
                    try
                    {
                        latestDecision = Paddle.DirectionFromString(message);
                    }
                    catch (ArgumentException e)
                    {
                        Debug.LogWarning(e);
                    }
                }
            }
            catch (IOException e)
            {
                Debug.LogWarning(e);
                Debug.LogWarningFormat("Disconnecting client {0}.", id);
                this.tcpClient = null;
                this.writer = null;
                this.reader = null;
                server.gameObject.SendMessage("OnClientDisconnected", this);
            }
        }

        return latestDecision;
    }


    public void OnGameStart(Arena.State state)
    {
        StringBuilder idList = new StringBuilder();
        for (int i = 0; i < state.paddles.Count; i++)
        {
            if (state.paddles[i].Actor is Client)
            {
                Client client = (Client)state.paddles[i].Actor;
                idList.Append(client.ID);
            }
            else
            {
                idList.Append(0);
            }

            if (i < state.paddles.Count - 1)
            {
                idList.Append(',');
            }
        }

        SendMessage(string.Format("{{\"event\":\"start_game\",\"player_ids\":[{0}]}}", idList.ToString()));
    }
    public void OnGameEnd(Arena.State state)
    {
        if (IsConnected)
        {
            // TODO: Maybe send winner?
            SendMessage(string.Format("{{\"event\":\"end_game\"}}"));
        }
    }

    public string ReadMessage()
    {
        if (IsConnected)
        {
            try
            {
                lock (receiveQueue)
                {
                    return receiveQueue.Dequeue();
                }
            }
            catch (InvalidOperationException)
            {
                // Queue was empty
            }
        }
        return null;
    }

    public void SendMessage(string message)
    {
        writer.WriteLine(message);
    }
}
