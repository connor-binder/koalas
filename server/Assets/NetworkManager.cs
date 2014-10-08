using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	private const string typeName = "UniqueGameName";
	private const string gameName = "RoomName";
	
	private HostData[] hostList;

	public GameObject PlayerPrefab;
		
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}

	private void StartServer()
	{
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}

	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}

	void OnServerInitialized()
	{
		Vector3 startPoint = new Vector3(0f, 5f, 0f);
		SpawnPlayer(startPoint);
	}
	
	void OnConnectedToServer()
	{
		Vector3 startPoint = new Vector3(5f, 5f, 5f);
		SpawnPlayer(startPoint);
	}
	
	private void SpawnPlayer(Vector3 startPoint)
	{
		Network.Instantiate(PlayerPrefab, startPoint, Quaternion.identity, 0);
	}

	void OnGUI()
	{
		if (!Network.isClient && !Network.isServer)
		{
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				StartServer();
			
			if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
				RefreshHostList();
			
			if (hostList != null)
			{
				for (int i = 0; i < hostList.Length; i++)
				{
					if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
						JoinServer(hostList[i]);
				}
			}
		}
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}
}
