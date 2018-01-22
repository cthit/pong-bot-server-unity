using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Arena))]
public class Server : MonoBehaviour {

	public int port = 4242;

	public RectTransform clientList;
	public GameObject clientListItemPrefab;

	private TcpListener listener;

	private int idTicker = 0;

	private List<Client> pendingClients = new List<Client>();
	private List<Client> clients = new List<Client>();

	private Arena arena;

	private void HandleIncomingConnections() {
		try {
			while(listener.Pending()) {
				try {
					TcpClient tcpClient = listener.AcceptTcpClient();
					Client client = new Client(++idTicker, tcpClient);
					pendingClients.Add(client);
					client.SendMessage(string.Format("{{\"id\":{0}}}", client.ID));
				} catch(SocketException e) {
					Debug.LogError(e);
				}
			}
		} catch(InvalidOperationException e) {
			Debug.LogErrorFormat("Listener has not been initialized: {}", e);
		}
	}

	private void HandlePendingClients() {
		foreach(Client client in pendingClients) {
			try {
				String message = client.ReadMessage();
				if(message != null) {
					Debug.LogFormat("Client {0} greeted with {1}.", client.ID, message);

					Client.ClientInfo clientInfo = JsonUtility.FromJson<Client.ClientInfo>(message);
					client.info = clientInfo;

					clients.Add(client);
					pendingClients.Remove(client);
					HandlePendingClients();
					UpdateClientList(); // TODO: move me somewhere more sensible
					break;
				}
			} catch(Exception e) {
				Debug.LogWarning(e);
				Debug.LogWarningFormat("Kicking client {0}", client.ID);
				pendingClients.Remove(client); // Remove offending client
				client.tcpClient.Close();
				HandlePendingClients(); // Try again
				break;
			}
		}
	}

	private void UpdateClientList() {
		foreach(Transform child in clientList) {
			GameObject.Destroy(child.gameObject);
		}

		for(int i = 0; i < clients.Count; i++) {
			GameObject listItem = GameObject.Instantiate(clientListItemPrefab, clientList);
			listItem.transform.Find("Name Field").GetComponent<Text>().text = clients[i].info.name;
		}
	}
	
	public void StartGame() {
		List<PongActor> actors = new List<PongActor>();
		actors.AddRange(clients.Select(x => (PongActor)x));
		arena.StartGame(actors);
	}

	// Use this for initialization
	void Start () {
		this.listener = new TcpListener(port);
		this.listener.Start();
		this.arena = GetComponent<Arena>();
	}
	
	// Update is called once per frame
	void Update () {
		HandleIncomingConnections();
		HandlePendingClients();
	}

	void OnDestroy() {
		foreach(Client client in clients) {
			client.tcpClient.Close();
		}
		foreach(Client client in pendingClients) {
			client.tcpClient.Close();
		}
		listener.Stop();
	}
}
