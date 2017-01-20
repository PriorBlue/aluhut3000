using UnityEngine;
using UnityEngine.UI;
using System;

public class LobbyRoom : MonoBehaviour 
{
	public Text statusText;
	public Text nameText;
	public Text playersText;
	public Text mapText;
	public Text pingText;
	public Image background;
	public Color evenColor, oddColor;
	
	Guid stateGuid, playersGuid, pingGuid;
	
	LobbyModel.Room m_room;
	public void Set(int position, LobbyModel.Room r)
	{
		m_room= r;
		background.color = position % 2 == 0 ? evenColor : oddColor;
		
		nameText.text = r.name;
		mapText.text = r.map;
		
		stateGuid = r.state.RegisterObserver((s) => statusText.text = s.ToString());
		playersGuid= r.players.RegisterObserver((p) => { playersText.text = string.Format("{0}/{1}", p, m_room.maxPlayers); });
		pingGuid= r.ping.RegisterObserver((p) => pingText.text = p.ToString());
	}
	
	void OnDestroy()
	{
		if (m_room != null) 
		{
			m_room.state.RemoveObserver (stateGuid);
			m_room.players.RemoveObserver (playersGuid);
			m_room.ping.RemoveObserver (pingGuid);
		}
	}
}
