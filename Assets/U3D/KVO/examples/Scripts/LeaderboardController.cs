using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class LeaderboardController : MonoBehaviour 
{
	public LeaderboardRow rowPrefab;
	public RectTransform leaderboardContainer;
	public LeaderboardModel model;
	// Use this for initialization
	void Start () 
	{
		model.leaderboard.RegisterObserver(UpdateLeaderboard);
	}
	void UpdateLeaderboard(ReadOnlyCollection<LeaderboardModel.TLeaderboardEntry> oldLeaderboard, ReadOnlyCollection<LeaderboardModel.TLeaderboardEntry> leaderboard)
	{
		while(leaderboardContainer.childCount > 0)
		{
			Transform i = leaderboardContainer.GetChild(0);
			DestroyImmediate(i.gameObject);
		}
		
		int pos= 1;
		foreach(LeaderboardModel.TLeaderboardEntry i in leaderboard)
		{
			LeaderboardRow row= Instantiate(rowPrefab) as LeaderboardRow;
			row.Set(pos, i.name, i.score);
			pos++;
			row.transform.SetParent(leaderboardContainer);
		}
	}
}
