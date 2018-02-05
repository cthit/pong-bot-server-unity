using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Arena))]
[RequireComponent(typeof(Server))]
public class DIFFAOrganizer : MonoBehaviour, GameOrganizer {

	private Arena arena;
	private Server server;
	private bool restart = false;

	void OnEnable() {
		arena = GetComponent<Arena>();
		server = GetComponent<Server>();
		RestartGame();
	}

	private void RestartGame() {
		if(arena.GameStarted) {
			arena.StopGame();
		}

		List<Client> clients = server.Clients;

		if(clients.Count == 0) {
			return;
		}

		
		List<PongActor> actors = clients.Cast<PongActor>().ToList();
		while(actors.Count < 4) {
			actors.Add(new DumbActor());
		}

		arena.StartGame(actors, RestartGame);
	}

	public void OnClientConnected(Client client) {
		restart = true;;
	}

	public void OnClientDisconnected(Client client) {
		restart = true;
	}

	void Update() {
		if(restart) {
			restart = false;
			RestartGame();
		}
	}
}
