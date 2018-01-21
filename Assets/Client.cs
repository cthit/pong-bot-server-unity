using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;

public class Client {

	[DataContract]
	public class ClientInfo {
		[DataMember]
		public string name;
	}	
	
	private int id;
	public ClientInfo info;
	public TcpClient tcpClient;
	private StreamReader reader;

	public int ID {
		get {
			return id;
		}
	}

	public Client(int id, TcpClient tcpClient) {
		this.id = id;
		this.info = null;
		this.tcpClient = tcpClient;
		this.reader = new StreamReader(tcpClient.GetStream());
	}

	public string ReadMessage() {
		return reader.ReadLine();
	}
}
