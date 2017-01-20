using UnityEngine;
using System.Collections.Generic;

public class LobbyModel : MonoBehaviour 
{
	public enum TState { Open = 0, Private = 1, Playing = 2, Full = 3 }
	public class Room
	{
		public U3D.KVO.ReadOnlyValueObserving<TState> state { get { return m_state; } }
		U3D.KVO.ValueObserving<TState> m_state = new U3D.KVO.ValueObserving<TState>();
		public string name { get; private set; }
		public U3D.KVO.ReadOnlyValueObserving<int> players { get { return m_players; } }
		U3D.KVO.ValueObserving<int> m_players = new U3D.KVO.ValueObserving<int> ();
		public int maxPlayers { get; private set; }
		public string map { get; private set; }
		public U3D.KVO.ReadOnlyValueObserving<int> ping { get { return m_ping; } }
		U3D.KVO.ValueObserving<int> m_ping = new U3D.KVO.ValueObserving<int>();
		
		public Room()
		{
			TState s= (TState)Random.Range(0, System.Enum.GetNames(typeof(TState)).Length);
			this.m_state.set = s;
			int maxPlayers = Random.Range(4, 20);
			this.maxPlayers = maxPlayers;
			this.name = "Room " + Random.Range(0, 100).ToString();
			this.map = "Map " + Random.Range(0, 100).ToString();
			this.m_players.set = s == TState.Full ? maxPlayers : Random.Range(0, maxPlayers-1);
			this.m_ping.set = Random.Range(20, 300);
		}
		public void Randomize()
		{
			TState newState = this.m_state.get;
			int modifyState = Random.Range(0, 3);
			// 0 -> stay, 1 -> move forward, 2 -> move backwards
			switch(newState)
			{
			case TState.Open:
				if(modifyState!=0) newState = TState.Playing;
				break;
			case TState.Private:
				if(modifyState!=0) newState = TState.Playing;
				break;
			case TState.Playing:
				if(modifyState== 1) newState = TState.Full;
				else if(modifyState== 2) newState = Random.Range(0,2) == 0 ? TState.Open : TState.Private;
				break;
			case TState.Full:
				if(modifyState!=0) newState = TState.Playing;
				break;
			}
			this.m_state.set = newState;
			this.m_players.set = newState == TState.Full ? maxPlayers : Random.Range(0, maxPlayers -1);
			this.m_ping.set = this.m_ping.get + Random.Range(-50, 51);
		}
	}
	public int nbOfRegisters = 10;
	[RangeAttribute(1, 10)]
	public float updateEverySeconds = 3;
	
	public U3D.KVO.ReadOnlyListObserving<Room> lobby { get { return m_lobby; } } 
	U3D.KVO.ListObserving<Room> m_lobby = new U3D.KVO.ListObserving<Room>();  
	
	List<Room> m_currentLobby = new List<Room>(); 
	void Start () 
	{
		for(int i= 0; i< nbOfRegisters; i++)
		{
			m_currentLobby.Add(new Room());
		}
		m_lobby.set= m_currentLobby;
	}

	float m_timeSinceLastUpdate = float.MaxValue;
	void Update () 
	{
		m_timeSinceLastUpdate+= Time.deltaTime;		
		if(m_timeSinceLastUpdate > updateEverySeconds)
		{
			m_timeSinceLastUpdate = 0;

			foreach(Room i in m_currentLobby)
			{
				i.Randomize();
			}
			int removeAdd= Random.Range(0, 3);
			// 0 nothing, 1 add, 2 remove
			if(removeAdd == 1 && m_currentLobby.Count < nbOfRegisters * 2)
			{
				m_currentLobby.Add(new Room());
				m_lobby.set= m_currentLobby;
			}
			else if(removeAdd == 2 && m_currentLobby.Count > 0)
			{
				m_currentLobby.RemoveAt(Random.Range(0, m_currentLobby.Count));
				m_lobby.set= m_currentLobby;
			}
		}
	}
}
