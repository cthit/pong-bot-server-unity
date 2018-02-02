using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Server))]
public class ServerEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        Server server = (Server)target;
        
        //myTarget.experience = EditorGUILayout.IntField("Experience", myTarget.experience);
        EditorGUILayout.IntField("Connected Clients", server.ClientCount + server.PendingClientCount);
		EditorGUILayout.Space();
		server.port = EditorGUILayout.IntField("Port", Mathf.Max(Mathf.Min(server.port, 65536), 1000));
		server.clientList = (RectTransform)EditorGUILayout.ObjectField(
			"Client List",
			server.clientList,
			typeof(RectTransform), true);
		server.clientListItemPrefab = (GameObject)EditorGUILayout.ObjectField(
			"Client List Item Prefab",
			server.clientListItemPrefab,
			typeof(GameObject), false);
    }
}