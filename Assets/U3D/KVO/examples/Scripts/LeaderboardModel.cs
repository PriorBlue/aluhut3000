using UnityEngine;
using System.Collections.Generic;

public class LeaderboardModel : MonoBehaviour 
{
	public class TLeaderboardEntry
	{
		public string name;
		public int score;
	}
	public int nbOfRegisters = 10;
	[RangeAttribute(1, 10)]
	public float updateEverySeconds = 3;
	
	public U3D.KVO.ReadOnlyListObserving<TLeaderboardEntry> leaderboard { get { return m_leaderboard; } } 
	U3D.KVO.ListObserving<TLeaderboardEntry> m_leaderboard = new U3D.KVO.ListObserving<TLeaderboardEntry>();  
	
	List<TLeaderboardEntry> m_currentLeaderboard = new List<TLeaderboardEntry>(); 
	void Start () 
	{
		for(int i= 0; i< nbOfRegisters * 2; i++)
		{
			m_currentLeaderboard.Add(new TLeaderboardEntry()
			{
				name = "Player " + (i + 1).ToString(),
				score = 0
			});
		}
	}
	
	class InvertedComparer : IComparer<float>
    {
        public int Compare(float x, float y)
        {
            return y.CompareTo(x);
        }
    }
	float m_timeSinceLastUpdate = float.MaxValue;
	void Update () 
	{
		m_timeSinceLastUpdate+= Time.deltaTime;		
		if(m_timeSinceLastUpdate > updateEverySeconds)
		{
			m_timeSinceLastUpdate = 0;

			// randomizing and sorting
			SortedList<float, TLeaderboardEntry> sortedList = new SortedList<float, TLeaderboardEntry>(new InvertedComparer());
			foreach(TLeaderboardEntry i in m_currentLeaderboard)
			{
				i.score += Random.Range(0, 5);
				float scoreForSorting = i.score;
				while(sortedList.ContainsKey(scoreForSorting)) scoreForSorting+= 0.1f / m_currentLeaderboard.Count;
				sortedList.Add(scoreForSorting, i); 
			}
			
			List<TLeaderboardEntry> list= new List<TLeaderboardEntry>();
			foreach(TLeaderboardEntry i in sortedList.Values)
			{
				list.Add(i);
				if(list.Count== nbOfRegisters) break;
			}
			
			m_leaderboard.set= list;
		}
	}
}
