﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

using TMPro;

using UnityEngine;

[RequireComponent(typeof(Arena))]
public class Server : MonoBehaviour
{

    public int port = 4242;

    public RectTransform clientList;
    public GameObject clientListItemPrefab;

    private TcpListener listener;

    private int idTicker = 0;

    private List<Client> pendingClients = new List<Client>();
    private List<Client> clients = new List<Client>();

    public List<Client> Clients { get { return new List<Client>(clients); } }
    public int ClientCount { get { return clients.Count; } }
    public int PendingClientCount { get { return clients.Count; } }

    private Arena arena;

    private void HandleIncomingConnections()
    {
        try
        {
            while (listener.Pending())
            {
                try
                {
                    TcpClient tcpClient = listener.AcceptTcpClient();
                    Client client = new Client(++idTicker, tcpClient, this);
                    pendingClients.Add(client);
                    Debug.LogFormat("New client {0} connected", client.ID);
                    client.SendMessage(string.Format("{{\"id\":{0}}}", client.ID));
                }
                catch (SocketException e)
                {
                    Debug.LogError(e);
                }
            }
        }
        catch (InvalidOperationException e)
        {
            Debug.LogErrorFormat("Listener has not been initialized: {}", e);
        }
    }

    private void HandlePendingClients()
    {
        foreach (Client client in pendingClients)
        {
            try
            {
                String message = client.ReadMessage();
                if (message != null)
                {
                    Debug.LogFormat("Client {0} sent greeting: \"{1}\".", client.ID, message);

                    Client.ClientInfo clientInfo = JsonUtility.FromJson<Client.ClientInfo>(message);
                    client.info = clientInfo;

                    clients.Add(client);
                    pendingClients.Remove(client);
                    HandlePendingClients();
                    UpdateClientList(); // TODO: move me somewhere more sensible

                    gameObject.SendMessage("OnClientConnected", client);
                    break;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                Debug.LogWarningFormat("Kicking client {0}", client.ID);
                pendingClients.Remove(client); // Remove offending client
                client.tcpClient.Close();
                HandlePendingClients(); // Try again
                break;
            }
        }
    }

    private void UpdateClientList()
    {
        foreach (Transform child in clientList)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < clients.Count; i++)
        {
            GameObject listItem = GameObject.Instantiate(clientListItemPrefab, clientList);
            listItem.transform.Find("Name Field").GetComponent<TextMeshProUGUI>().text = clients[i].info.name;
            listItem.transform.Translate(0, -40 * i, 0);
        }
    }

    public void OnClientDisconnected(Client client)
    {
        if (clients.Remove(client))
        {
            UpdateClientList();
        }
        else
        {
            Debug.LogError("OnClientDisconnected called on non-existing client");
        }
    }

    public void StartGame()
    {
        List<PongActor> actors = new List<PongActor>();
        actors.AddRange(clients.Select(x => (PongActor)x));
        arena.StartGame(actors, null);
    }

    // Use this for initialization
    void Start()
    {
        this.listener = new TcpListener(port);
        this.listener.Start();
        this.arena = GetComponent<Arena>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleIncomingConnections();
        HandlePendingClients();
    }

    void OnDestroy()
    {
        foreach (Client client in clients)
        {
            client.tcpClient.Close();
        }
        foreach (Client client in pendingClients)
        {
            client.tcpClient.Close();
        }
        listener.Stop();
    }
}
