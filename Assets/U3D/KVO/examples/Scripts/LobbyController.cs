using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;


public class LobbyController : MonoBehaviour 
{
	public LobbyRoom roomPrefab;
	public RectTransform lobbyContainer;
	public LobbyModel model;
	// Use this for initialization
	void Start () 
	{
		model.lobby.RegisterObserver(UpdateLobby);
	}
	void UpdateLobby(ReadOnlyCollection<LobbyModel.Room> oldLobby, ReadOnlyCollection<LobbyModel.Room> lobby)
	{
		while(lobbyContainer.childCount > 0)
		{
			Transform c = lobbyContainer.GetChild(0);
			DestroyImmediate(c.gameObject);
		}
		
		int pos= 1;
		foreach(LobbyModel.Room i in lobby)
		{
			LobbyRoom row= Instantiate(roomPrefab) as LobbyRoom;
			row.Set(pos, i);
			pos++;
			row.transform.SetParent(lobbyContainer);
		}
	}
}
