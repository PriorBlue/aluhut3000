using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LikesIncrease : MonoBehaviour {
	public int PostingValue = 1;

	public void Increase () {
		Game.Instance.Player.Follower.set = Game.Instance.Player.Follower.get + PostingValue * Game.Instance.Player.LikeMultiplier.get;
	}
}
