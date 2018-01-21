using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using UnityEngine;
using UnityEngine.UI;
public class PongMaster : MonoBehaviour {

	public int port = 4242;

	public GameObject ballPrefab;
	public GameObject paddlePrefab;

	public RectTransform clientList;
	public GameObject clientListItemPrefab;

	private TcpListener listener;

	private int idTicker = 0;

	private List<Client> pendingClients = new List<Client>();
	private List<Client> clients = new List<Client>();

	private void HandleIncomingConnections() {
		try {
			while(listener.Pending()) {
				try {
					TcpClient tcpClient = listener.AcceptTcpClient();
					pendingClients.Add(new Client(++idTicker, tcpClient));
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
					Debug.Log(message);
					clients.Add(client);
					pendingClients.Remove(client);
					HandlePendingClients();
					UpdateClientList(); // TODO: move me somewhere more sensible
					break;
				}
			} catch(ObjectDisposedException e) {
				Debug.LogWarning(e);
				pendingClients.Remove(client); // Remove offending client
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

	// Use this for initialization
	void Start () {
		listener = new TcpListener(port);
		listener.Start();
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
