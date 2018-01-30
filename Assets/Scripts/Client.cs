using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System;
using UnityEngine;
using System.Threading;

public class Client : PongActor {

	const float NETWORK_SEND_DELAY = 0.1f;

	[System.Serializable]
	public class ClientInfo {
		public string name;
	}	
	
	private int id;
	public ClientInfo info;
	public TcpClient tcpClient;
	private StreamReader reader;
	private StreamWriter writer;

	private Paddle.Direction latestDecision = Paddle.Direction.None;
	private float latestNetworkSend = 0.0f;

	private Queue<string> receiveQueue = new Queue<string>();

	private Thread messageReader;

	public int ID {
		get {
			return id;
		}
	}

	public bool IsConnected {
		get {
			return tcpClient != null;
		}
	}

	public Client(int id, TcpClient tcpClient) {
		this.id = id;
		this.info = null;
		this.tcpClient = tcpClient;
		this.reader = new StreamReader(tcpClient.GetStream());
		this.writer = new StreamWriter(tcpClient.GetStream());
		this.latestNetworkSend = Time.fixedTime;

		// Always write instantly to socket on writer.Write()
		this.writer.AutoFlush = true;

		this.messageReader = new Thread(MessageReader);
		messageReader.Start();
	}

	private void MessageReader() {
		try {
			while(true) {
				string message = reader.ReadLine();
				lock(receiveQueue) {
					receiveQueue.Enqueue(message);
				}
			}
		} catch(IOException e) {
			Debug.LogError(e);
		}
	}
	
	public Paddle.Direction MakeDecision(Arena.State state) {
		if(IsConnected) {
			try {
				if(Time.fixedTime - latestNetworkSend >= NETWORK_SEND_DELAY) {
					writer.WriteLine(state.ToJson());
					latestNetworkSend = Time.fixedTime;
				}

				string message = ReadMessage();

				if(message != null) {
					try {
						latestDecision = Paddle.DirectionFromString(message);
					} catch(ArgumentException e) {
						Debug.LogWarning(e);
					}
				}
			} catch(IOException e) {
				Debug.LogWarning(e);
				Debug.LogWarningFormat("Disconnecting client {0}.", id);
				this.tcpClient = null;
				this.writer = null;
				this.reader = null;
			}
		}

		return latestDecision;
	}

	public string ReadMessage() {
		if(IsConnected) {
			try {
				lock (receiveQueue) {
					return receiveQueue.Dequeue();
				}
			} catch(InvalidOperationException) {
				// Queue was empty
			}
		}
		return null;
	}

	public void SendMessage(string message) {
		writer.WriteLine(message);
	}
}
